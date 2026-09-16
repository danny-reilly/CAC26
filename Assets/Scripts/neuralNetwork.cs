using UnityEngine;

public class neuralNetwork : MonoBehaviour
{
    //how many neurons in each layer
    int[] layerSizes = {4, 8, 8, 5, 8, 5};
    //values of neurons (not including output)
    int[][] neurons;
    //connections of neuron layers
    int[][][] weights;

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
        


        //initialize weights with x layers
        weights = new int[layerSizes.Length-1][][];
        for(int layerNum = 0; layerNum < layerSizes.Length-1; layerNum++)
        {
            //intitialize each layer with y neurons
            weights[layerNum] = new int[layerSizes[layerNum]][];

            //initialize each neuron with z weights
            //z = amount of neurons in next layer
            for (int neuronNum = 0; neuronNum < layerSizes[layerNum]; neuronNum++)
            {
                weights[layerNum][neuronNum] = new int[layerSizes[layerNum+1]];
            }
        }

        for(int x = 0; x < weights.Length; x++)
        {
            for (int y = 0; y < weights[x].Length; y++)
            {
                for (int z = 0; z < weights[x][y].Length; z++)
                {
                    print(x + ", " + y + ", " + z);
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
