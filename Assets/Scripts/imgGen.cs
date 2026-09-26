using UnityEngine;

public class imgGen : MonoBehaviour
{
    //how many neurons in each layer
    const int imgSize = 25;

    //neurons[x][y], x = layer num, y = which neuron in layer
    //weights[x][y][z], x = layer num, y = which neuron in layer, z = what neuron in the next layer its connected to

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Texture2D texture = new Texture2D(imgSize, imgSize, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        Color[] pixels = new Color[imgSize * imgSize];
        for (int i = 0; i < imgSize * imgSize; i++)
        {
            float c = ((float)i / 23) % 1;
            pixels[i] = new Color(c, c, c);
        }
        texture.SetPixels(pixels);
        texture.Apply();
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), imgSize / 10f);

    }

   
}
