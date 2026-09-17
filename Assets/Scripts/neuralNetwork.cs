using UnityEngine;

public class neuralNetwork : MonoBehaviour
{
    //how many neurons in each layer
    int[] layerSizes = {4, 8000, 6000, 5000, 8000, 49152};
    //values of neurons (not including output)
    float[][] neurons;
    //connections of neuron layers
    float[][][] weights;

    public Transform point1;
    public Transform point2;

    public GameObject neuron;
    public GameObject lr;

    private LineRenderer line;

    //neurons[x][y], x = layer num, y = which neuron in layer
    //weights[x][y][z], x = layer num, y = which neuron in layer, z = what neuron in the next layer its connected to

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        ///////////////////////////////////////////////////////////////// initialization /////////////////////////////////////////////////////////////////
        //initialize neurons with x layers 
        neurons = new float[layerSizes.Length][];
        for(int i = 0; i < layerSizes.Length; i++)
        {
            //initialize each layer with y neurons
            neurons[i] = new float[layerSizes[i]];
        }

        neurons[0] = new float[] {0.5f, -0.53f, 0.84f, -0.2f};



        //initialize weights with x layers and set them randomly
        weights = new float[layerSizes.Length-1][][];
        for(int layerNum = 0; layerNum < layerSizes.Length-1; layerNum++)
        {
            //intitialize each layer with y neurons
            weights[layerNum] = new float[layerSizes[layerNum]][];

            //initialize each neuron with z weights
            //z = amount of neurons in next layer
            for (int neuronNum = 0; neuronNum < layerSizes[layerNum]; neuronNum++)
            {
                weights[layerNum][neuronNum] = new float[layerSizes[layerNum+1]];



                //set weights to random value
                for(int weightNum = 0; weightNum < layerSizes[layerNum+1]; weightNum++)
                {
                    //(uses Xavier/Glorot initialization to scale for different layer sizes)
                    float maxWeight = Mathf.Sqrt(6.0f / (layerSizes[layerNum] + layerSizes[layerNum + 1]));
                    weights[layerNum][neuronNum][weightNum] = Random.Range(maxWeight, -maxWeight);
                    //print(weights[layerNum][neuronNum][weightNum]);
                }
            }
        }




        ///////////////////////////////////////////////////////////////// display /////////////////////////////////////////////////////////////////
        /*display neurons and lines
        for (int x = 0; x < layerSizes.Length; x++)
        {
            for(int y = 0; y < layerSizes[x] ;y++)
            {
                //print(x + ", " + y);
                GameObject newNeuron = Instantiate(neuron);
                newNeuron.transform.position = new Vector2(-8 + 16 * ((float)x / layerSizes.Length),  -5 +   10 * ((float)(y+1) / (layerSizes[x]+1)) );

                if (x != layerSizes.Length -1)
                {
                    for (int z = 0; z < layerSizes[x + 1]; z++)
                    {
                        drawLine(getPos(x, y), getPos(x + 1, z), weights[x][y][z]);
                    }
                }

            }
        }


        *///////////////////////////////////////////////////////////////// processing /////////////////////////////////////////////////////////////////
        for(int layerNumb = 0; layerNumb < layerSizes.Length-1; layerNumb++)
        {
            //for each neuron in the next layer
            for(int neuronNumb = 0; neuronNumb < layerSizes[layerNumb+1]; neuronNumb++)
            {
                for(int prevNueronNumb = 0; prevNueronNumb < layerSizes[layerNumb]; prevNueronNumb++)
                {
                    neurons[layerNumb + 1][neuronNumb] += (neurons[layerNumb][prevNueronNumb] * weights[layerNumb][prevNueronNumb][neuronNumb]);
                }

                neurons[layerNumb + 1][neuronNumb] = 1 / (1 + Mathf.Pow(2.718281828459045f, -neurons[layerNumb + 1][neuronNumb]));
                //print($"({layerNumb + 1}, {neuronNumb}): {neurons[layerNumb + 1][neuronNumb]}");
            }
        }


        ///////////////////////////////////////////////////////////////// creating image ////////////////////////////////////////////////////////////////
        
        Texture2D texture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        Color[] pixels = new Color[16384];
        for(int i = 0; i < 16384; i++)
        {
            pixels[i] = new Color(returnOutput(i * 3), returnOutput(1 + i * 3), returnOutput(2 + i * 3));
        }
        texture.SetPixels(pixels);
        texture.Apply();
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 10.0f);

    }




    float returnOutput(int num)
    {
        return neurons[layerSizes.Length - 1][num];
    }

    void drawLine(Vector3 start, Vector3 end, float color)
    {
        GameObject newLine = Instantiate(lr);
        Vector3[] pos = new Vector3[] {start, end};
        lr.GetComponent<LineRenderer>().SetPositions(pos);
        lr.GetComponent<LineRenderer>().startColor = new Color(color, color, color);
        lr.GetComponent<LineRenderer>().endColor = new Color(color, color, color);

    }

    Vector3 getPos(int x, int y)
    {
        return new Vector3(-8 + 16 * ((float)x / layerSizes.Length), -5 + 10 * ((float)(y + 1) / (layerSizes[x] + 1)), 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
