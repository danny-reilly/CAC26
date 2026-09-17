using UnityEngine;

public class neuralNetwork : MonoBehaviour
{
    //how many neurons in each layer
    int[] layerSizes = {4, 8, 6, 5, 8, 4};
    //values of neurons (not including output)
    int[][] neurons;
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
        //initialize neurons with x layers 
        neurons = new int[layerSizes.Length][];
        for(int i = 0; i < layerSizes.Length; i++)
        {
            //initialize each layer with y neurons
            neurons[i] = new int[layerSizes[i]];
            i++;
        }
        


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
                    print(weights[layerNum][neuronNum][weightNum]);
                }
            }
        }


 


        //display neurons and lines
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
