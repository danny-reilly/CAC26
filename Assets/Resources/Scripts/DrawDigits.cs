using System;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DrawDigits : MonoBehaviour
{
    public GameObject digitCanvas;

    private InputAction mouseClick;
    private InputAction resetCanvas;

    private Image drawingImage;
    private Image centerImage;

    Texture2D texture;

    public bool mouseDown;

    public int numRowsCols;
    public int drawRadius;
    public int[,] grid;
    public int[] convertArray;
    int[,] centeredGrid;
    int[,] fixedCenteredGrid;
    public int[] centeredGridArray;

    int lastPixelX, lastPixelY;
    imgGenerator imgGen = new imgGenerator();
    public runNetwork runNetwork;

    void Awake()
    {
        drawingImage = digitCanvas.GetComponent<Image>();
        centerImage = GameObject.Find("Image (1)").GetComponent<Image>();


        int[,] tempGrid = new int[numRowsCols, numRowsCols];
        grid = tempGrid;
        for (int i = 0; i < numRowsCols; i++)
        {
            for (int j = 0; j < numRowsCols; j++)
            {
                grid[i, j] = 0;
            }
        }

        mouseClick = new InputAction(binding: "<Mouse>/leftButton");
        resetCanvas = new InputAction(binding: "<Keyboard>/e");
        mouseClick.performed += ctx => mouseClicked();
        mouseClick.canceled += ctx => mouseUnclicked();
        resetCanvas.performed += ctx => erase();
    }

    private void OnEnable()
    {
        mouseClick.Enable();
        resetCanvas.Enable();
    }
    private void OnDisable()
    {
        mouseClick.Disable();
        resetCanvas.Disable();   
    }


    // Update is called once per frame
    void Update()
    {
        Vector2 mouseScreenPos = Mouse.current?.position.ReadValue() ?? Vector2.zero;
        Vector3 screenPos3D = new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPos3D);

        Vector2 mousePosition = new Vector2(worldPosition.x, worldPosition.y);
        Vector2 digitCanvasPos = new Vector2((digitCanvas.transform.position.x-960) * 0.00925925925f, (digitCanvas.transform.position.y-540)*0.00925925925f); // decimals convert positions to positions the mouse uses

        // worldspace coords centered on the image
        Vector2 mouseToCanvasRelative = new Vector2((mousePosition.x - digitCanvasPos.x) /0.00925925925f, (mousePosition.y - digitCanvasPos.y) /0.00925925925f);
        float pixelX = ((numRowsCols * (mouseToCanvasRelative.x/digitCanvas.GetComponent<Image>().rectTransform.rect.width) / digitCanvas.GetComponent<Image>().rectTransform.localScale.x) + (numRowsCols/2));
        float pixelY =  ((numRowsCols * (mouseToCanvasRelative.y/digitCanvas.GetComponent<Image>().rectTransform.rect.height) / digitCanvas.GetComponent<Image>().rectTransform.localScale.y) + (numRowsCols/2));
        int pixelCoordX = (int)pixelX;
        int pixelCoordY = (int)pixelY;


        //Vector2 pixelCoords = new Vector2(pixelCoordX, pixelCoordY);
        //print($"x: {pixelCoordX}, y: {pixelCoordY}");
        if (mouseDown/* && (lastPixelX != pixelCoordX || lastPixelY != pixelCoordY)*/)
        {
            for(int i=-drawRadius; i<drawRadius; i++)
            {
                for(int j=-drawRadius; j<drawRadius; j++)
                {
                    if((i) * (i) + (j) * (j) <= (drawRadius-0.5)*(drawRadius-0.5)) // checks a circle around the cursor
                    {
                        if(pixelCoordX + i > 0 && pixelCoordX + i <= numRowsCols && pixelCoordY + j > 0 && pixelCoordY + j <= numRowsCols) // checks bounds to stop overflow
                        {
                            int distance = (int) (150*Mathf.Abs(pixelX - Mathf.Abs(pixelCoordX - i+2))) + (int) (150*Mathf.Abs(pixelY - Mathf.Abs(pixelCoordY - j+1)));

                            if(grid[pixelCoordY + j-1, pixelCoordX + i-1] < 255)// - distance)
                            {
                                grid[pixelCoordY + j-1, pixelCoordX + i-1] = 255;// - distance;   
                            }
                        }
                    }
                }
            }
        }

        convertArray = grid.Cast<int>().ToArray();

        texture = imgGen.createImg(28, convertArray);
        Rect rect = new Rect(0, 0, drawingImage.rectTransform.rect.width,drawingImage.rectTransform.rect.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        drawingImage.sprite = Sprite.Create(texture, rect, pivot, 1);

        // used to check if the grid pos changed since last frame
        lastPixelX = pixelCoordX;
        lastPixelY = pixelCoordY;


        centerImg();
    }

    

    void mouseClicked()
    {
        mouseDown = true;
    }

    void mouseUnclicked()
    {
        mouseDown = false;
    }

    public void erase()
    {
        for (int i = 0; i < 28; i++) {
            for (int j = 0; j < 28; j++)
            {
                grid[i, j] = 0;
            }
        }
        runNetwork.leaderboardText.text = runNetwork.startingLeaderboardText;
    }

    void centerImg()
    {
        centeredGrid = new int[numRowsCols, numRowsCols];
        fixedCenteredGrid = new int[numRowsCols, numRowsCols];
        for (int i = 0; i < numRowsCols; i++)
        {
            for (int j = 0; j < numRowsCols; j++)
            {
                centeredGrid[i, j] = 0;
                fixedCenteredGrid[i,j] = 0;
            }
        }

        int minX = numRowsCols, minY = numRowsCols, maxX = -1, maxY = -1;

        for (int x = 0; x < numRowsCols; x++) {
            for (int y = 0; y < numRowsCols; y++)
            {
                if (grid[x, y] != 0)
                {
                    minX = Mathf.Min(minX, x);
                    minY = Mathf.Min(minY, y);
                    maxX = Mathf.Max(maxX, x);
                    maxY = Mathf.Max(maxY, y);
                }
            }
        }

        //if drawing not blank
        if(maxX != -1)
        {
            int centerX = (minX + maxX) / 2;
            int centerY = (minY + maxY) / 2;
            int width = (maxX - minX)+1;
            int height = (maxY - minY)+1;

            int maxCharSize = 20;

            float scale = Mathf.Min((float)maxCharSize/width, (float)maxCharSize/height);


            int newWidth = Mathf.Max(
            1, Mathf.RoundToInt(width * scale));

            int newHeight = Mathf.Max(
                1, Mathf.RoundToInt(height * scale));

            // Position the resized character in the center.
            int offsetX = (28 - newWidth) / 2;
            int offsetY = (28 - newHeight) / 2;

            // Resize using nearest-neighbor sampling.
            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    int sourceX = minX + Mathf.Min(
                        width - 1,
                        Mathf.FloorToInt((x + 0.5f) / scale));

                    int sourceY = minY + Mathf.Min(
                        height - 1,
                        Mathf.FloorToInt((y + 0.5f) / scale));

                    centeredGrid[offsetX + x, offsetY + y] =
                        grid[sourceX, sourceY];
                }
            }

        }
        /*for(int i=0; i<centeredGrid.GetLength(0); i++)
        {
            for(int j=0; j<centeredGrid.GetLength(0); j++)
            {
                fixedCenteredGrid[i, j] = centeredGrid[j, i];
            }
        }*/


        centeredGridArray = centeredGrid.Cast<int>().ToArray(); //fixedCenteredGrid.Cast<int>().ToArray();
        texture = imgGen.createImg(28, centeredGrid.Cast<int>().ToArray());
        Rect rect = new Rect(0, 0, centerImage.rectTransform.rect.width, centerImage.rectTransform.rect.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        centerImage.sprite = Sprite.Create(texture, rect, pivot, 1);
    }



}