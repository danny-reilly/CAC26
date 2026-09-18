using UnityEngine;

public class neuralNetwork : MonoBehaviour
{
    //how many neurons in each layer
    const int imgSize = 10;
    int[] layerSizes = {4, 800, 800, imgSize*imgSize*3};
    //values of neurons (not including output)
    public float[][] neurons;
    //connections of neuron layers
    public float[][][] weights;

    public Transform point1;
    public Transform point2;

    public GameObject neuron;
    public GameObject lr;
    public GameObject renders;

    private LineRenderer line;

    public float score;

    //neurons[x][y], x = layer num, y = which neuron in layer
    //weights[x][y][z], x = layer num, y = which neuron in layer, z = what neuron in the next layer its connected to

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
    }

    public void initialize()
    {
        print("running initialization");
        ///////////////////////////////////////////////////////////////// initialization /////////////////////////////////////////////////////////////////
        //initialize neurons with x layers 
        neurons = new float[layerSizes.Length][];
        for (int i = 0; i < layerSizes.Length; i++)
        {
            //initialize each layer with y neurons
            neurons[i] = new float[layerSizes[i]];
        }

        neurons[0] = new float[] { 0.5f, -0.53f, 0.84f, -0.2f };



        //initialize weights with x layers and set them randomly
        weights = new float[layerSizes.Length - 1][][];
        for (int layerNum = 0; layerNum < layerSizes.Length - 1; layerNum++)
        {
            //intitialize each layer with y neurons
            weights[layerNum] = new float[layerSizes[layerNum]][];

            //initialize each neuron with z weights
            //z = amount of neurons in next layer
            for (int neuronNum = 0; neuronNum < layerSizes[layerNum]; neuronNum++)
            {
                weights[layerNum][neuronNum] = new float[layerSizes[layerNum + 1]];



                //set weights to random value
                for (int weightNum = 0; weightNum < layerSizes[layerNum + 1]; weightNum++)
                {
                    //(uses Xavier/Glorot initialization to scale for different layer sizes)
                    float maxWeight = Mathf.Sqrt(600.0f / (layerSizes[layerNum] + layerSizes[layerNum + 1]));
                    weights[layerNum][neuronNum][weightNum] = Random.Range(-maxWeight, maxWeight);
                    //print(weights[layerNum][neuronNum][weightNum]);
                }
            }
        }




        ///////////////////////////////////////////////////////////////// display /////////////////////////////////////////////////////////////////
        /*/display neurons and lines
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
        }*/
    }




    public void runNeuralNetwork()
    {
        score = 0;
        //print(transform.name);
        for (int i = 1; i < neurons.Length; i++)
        {
            System.Array.Clear(neurons[i], 0, neurons[i].Length);
        }


        ///////////////////////////////////////////////////////////////// processing /////////////////////////////////////////////////////////////////
        for (int layerNumb = 0; layerNumb < layerSizes.Length - 1; layerNumb++)
        {
            //for each neuron in the next layer
            for (int neuronNumb = 0; neuronNumb < layerSizes[layerNumb + 1]; neuronNumb++)
            {
                for (int prevNueronNumb = 0; prevNueronNumb < layerSizes[layerNumb]; prevNueronNumb++)
                {
                    neurons[layerNumb + 1][neuronNumb] += (neurons[layerNumb][prevNueronNumb] * weights[layerNumb][prevNueronNumb][neuronNumb]);
                }

                if(true)//layerNumb == layerSizes.Length - 2)
                {
                    neurons[layerNumb + 1][neuronNumb] = 1 / (1 + Mathf.Pow(2.718281828459045f, -neurons[layerNumb + 1][neuronNumb]));
                }
                //print($"({layerNumb + 1}, {neuronNumb}): {neurons[layerNumb + 1][neuronNumb]}");
            }
        }


        ///////////////////////////////////////////////////////////////// creating image ////////////////////////////////////////////////////////////////

        Texture2D texture = new Texture2D(imgSize, imgSize, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        Color[] pixels = new Color[imgSize*imgSize];
        for (int i = 0; i < imgSize * imgSize; i++)
        {
            pixels[i] = new Color(returnOutput(i * 3), returnOutput(1 + i * 3), returnOutput(2 + i * 3));
        }
        texture.SetPixels(pixels);
        texture.Apply();
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1.0f);

        //print(score);

        /*display neurons and lines
        foreach(Transform item in renders.transform)
        {
            Destroy(item.gameObject);
        }

        for (int x = 0; x < layerSizes.Length; x++)
        {
            for (int y = 0; y < layerSizes[x]; y++)
            {
                //print(x + ", " + y);
                GameObject newNeuron = Instantiate(neuron);
                newNeuron.transform.parent = renders.transform;
                newNeuron.transform.position = new Vector2(-8 + 16 * ((float)x / layerSizes.Length), -5 + 10 * ((float)(y + 1) / (layerSizes[x] + 1)));
                newNeuron.GetComponent<SpriteRenderer>().color = new Color(neurons[x][y], neurons[x][y], neurons[x][y]);

                if (x != layerSizes.Length - 1)
                {
                    for (int z = 0; z < layerSizes[x + 1]; z++)
                    {
                        drawLine(getPos(x, y), getPos(x + 1, z), weights[x][y][z]);
                    }
                }

            }
        }*/
    }


    public void mutateWeights(float mutateChance)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for(int j = 0; j < weights[i].Length; j++)
            {
                for(int k = 0; k < weights[i][j].Length; k++)
                {
                    if(Random.Range(0.0f, 100.0f) < mutateChance)
                    {
                        int mutateType = Random.Range(0, 3);
                        if(mutateType == 0)
                        {
                            float maxWeight = Mathf.Sqrt(600.0f / (layerSizes[i] + layerSizes[i + 1]));
                            weights[i][j][k] = Random.Range(-maxWeight, maxWeight);
                        } else if(mutateType == 1)
                        {
                            weights[i][j][k] *= -1;
                        } else
                        {
                            weights[i][j][k] += Random.Range(-0.02f, 0.02f);
                        }
                    }
                }
            }
        }
    }


    float returnOutput(int num)
    {
        score += neurons[layerSizes.Length - 1][num];
        return neurons[layerSizes.Length - 1][num];
    }

    void drawLine(Vector3 start, Vector3 end, float color)
    {
        GameObject newLine = Instantiate(lr);
        newLine.transform.parent = renders.transform;
        Vector3[] pos = new Vector3[] {start, end};
        lr.GetComponent<LineRenderer>().SetPositions(pos);
        lr.GetComponent<LineRenderer>().startColor = new Color(color, color, color);
        lr.GetComponent<LineRenderer>().endColor = new Color(color, color, color);

    }

    Vector3 getPos(int x, int y)
    {
        return new Vector3(-8 + 16 * ((float)x / layerSizes.Length), -5 + 10 * ((float)(y + 1) / (layerSizes[x] + 1)), 0);
    }


    public float[][][] cloneWeights()
    {
        float[][][] clone = new float[weights.Length][][];
        for (int i = 0; i < weights.Length; i++)
        {
            clone[i] = new float[weights[i].Length][];
            for (int j = 0; j < weights[i].Length; j++)
            {
                clone[i][j] = (float[])weights[i][j].Clone();
            }
        }
        return clone;
    }

    public float[][] cloneNeurons()
    {
        float[][] clone = new float[neurons.Length][];
        for (int i = 0; i < neurons.Length; i++)
        {
            clone[i] = (float[])neurons[i].Clone();
        }
        return clone;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
