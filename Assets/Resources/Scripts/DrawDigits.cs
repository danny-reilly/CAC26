using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;

public class DrawDigits : MonoBehaviour
{
    public GameObject digitCanvas;

    private InputAction mouseClick;

    Texture2D texture;

    public bool mouseDown;

    public int numRowsCols;
    public int drawRadius;
    public int[,] grid;
    public int[] convertArray;

    int lastPixelX, lastPixelY;
    imgGenerator imgGen = new imgGenerator();

    void Awake()
    {
        int[,] tempGrid = new int[numRowsCols, numRowsCols];
        grid = tempGrid;
        for(int i=0; i<numRowsCols; i++)
        {
            for(int j=0; j<numRowsCols; j++)
            {
                grid[i,j] = 0;
            }
        }

        mouseClick = new InputAction(binding: "<Mouse>/leftButton");
        mouseClick.performed += ctx => mouseClicked();
        mouseClick.canceled += ctx => mouseUnclicked();
    }

    private void OnEnable() => mouseClick.Enable();
    private void OnDisable() => mouseClick.Disable();


    // Update is called once per frame
    void Update()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
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
        if(mouseDown/* && (lastPixelX != pixelCoordX || lastPixelY != pixelCoordY)*/)
        {
            for(int i=-drawRadius; i<drawRadius; i++)
            {
                for(int j=-drawRadius; j<drawRadius; j++)
                {
                    if((i) * (i) + (j) * (j) <= (drawRadius-0.5)*(drawRadius-0.5)) // checks a circle around the cursor
                    {
                        if(pixelCoordX + i > 0 && pixelCoordX + i <= numRowsCols && pixelCoordY + j > 0 && pixelCoordY + j <= numRowsCols) // checks bounds to stop overflow
                        {
                            int distance = (int) (40*Mathf.Abs(pixelX - Mathf.Abs(pixelCoordX - i+2))) + (int) (40*Mathf.Abs(pixelY - Mathf.Abs(pixelCoordY - j+1)));

                            if(grid[pixelCoordY + j-1, pixelCoordX + i-1] < 255 - distance)
                            {
                                grid[pixelCoordY + j-1, pixelCoordX + i-1] = 255 - distance;   
                            }
                        }
                    }
                }
            }
        }

        convertArray = grid.Cast<int>().ToArray();

        texture = imgGen.createImg(28, convertArray);
        Rect rect = new Rect(0, 0, digitCanvas.GetComponent<Image>().rectTransform.rect.width,digitCanvas.GetComponent<Image>().rectTransform.rect.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        digitCanvas.GetComponent<Image>().sprite = Sprite.Create(texture, rect, pivot, 1);

        // used to check if the grid pos changed since last frame
        lastPixelX = pixelCoordX;
        lastPixelY = pixelCoordY;
    }

    

    void mouseClicked()
    {
        mouseDown = true;
    }

    void mouseUnclicked()
    {
        mouseDown = false;
    }
}