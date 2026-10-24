using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.InputSystem.Interactions;

public class neuralNetwork
{
    //how many neurons in each layer
    int[] layerSizes;
    float[] inputs;
    //values of neurons (not including output)
    float[][] neurons;
    //connections of neuron layers
    float[][][] weights;
    //values of biases
    float[][] biases;

    //neuron values before sigmoid/ReLU
    float[][] neuronsPreActivation;
    //Derivatives for calculating weight/bias changes
    float[][] neuronDerivs;

    //storage for weight/bias changes (added after each batch of images)
    float[][][] weightChanges;
    float[][] biasChanges;

    //intended outputs
    float[] goals;


    //how fast gradient decent happens (if it's too high or low, it will go towards infinity or not move and never reach the expected value)
    float learnRate;

    public int[] shuffledMNISTIndex;


    //neurons[x][y], x = layer num, y = which neuron in layer
    //weights[x][y][z], x = layer num, y = which neuron in layer, z = what neuron in the next layer its connected to

    public neuralNetwork(float[] inputs, int[] hiddenLayers, float[] goals, float learnRate)
    {
        this.inputs = inputs;
        this.goals = goals;
        this.learnRate = learnRate;

        layerSizes = new int[hiddenLayers.Length + 2];
        layerSizes[0] = inputs.Length;
        for (int i = 1; i < hiddenLayers.Length + 1; i++)
        {
            layerSizes[i] = hiddenLayers[i - 1];
        }
        layerSizes[hiddenLayers.Length + 1] = goals.Length;
    }

    ////////////////////initialization for when weights and biases aren't trained yet
    public void trainingInitialization()
    {
        //initialize neurons and activations with x layers 
        neurons = new float[layerSizes.Length][];
        neuronsPreActivation = new float[layerSizes.Length][];
        neuronDerivs = new float[layerSizes.Length][];
        for (int layerNum = 0; layerNum < layerSizes.Length; layerNum++)
        {
            //initialize each layer with y neurons
            neurons[layerNum] = new float[layerSizes[layerNum]];
            neuronsPreActivation[layerNum] = new float[layerSizes[layerNum]];
            neuronDerivs[layerNum] = new float[layerSizes[layerNum]];
        }

        for (int i = 0; i < neurons[0].Length; i++)
        {
            neurons[0][i] = inputs[i];
        }



        //initialize weights with x layers and set them randomly
        weights = new float[layerSizes.Length - 1][][];
        weightChanges = new float[layerSizes.Length - 1][][];
        for (int layerNum = 0; layerNum < layerSizes.Length - 1; layerNum++)
        {
            //intitialize each layer with y neurons
            weights[layerNum] = new float[layerSizes[layerNum]][];
            weightChanges[layerNum] = new float[layerSizes[layerNum]][];

            //initialize each neuron with z weights
            //z = amount of neurons in next layer
            for (int neuronNum = 0; neuronNum < layerSizes[layerNum]; neuronNum++)
            {
                weights[layerNum][neuronNum] = new float[layerSizes[layerNum + 1]];
                weightChanges[layerNum][neuronNum] = new float[layerSizes[layerNum + 1]];


                //set weights to random value
                for (int weightNum = 0; weightNum < layerSizes[layerNum + 1]; weightNum++)
                {
                    //(uses He initialization to scale for different layer sizes)
                    float maxWeight = Mathf.Sqrt(6.0f / layerSizes[layerNum]);
                    weights[layerNum][neuronNum][weightNum] = Random.Range(-maxWeight, maxWeight);
                    //print(weights[layerNum][neuronNum][weightNum]);
                }
            }
        }

        //initialize biases
        biases = new float[layerSizes.Length - 1][];
        biasChanges = new float[layerSizes.Length - 1][];
        for (int layerNum = 0; layerNum < layerSizes.Length - 1; layerNum++)
        {
            //intitialize each layer with y neurons
            biases[layerNum] = new float[layerSizes[layerNum + 1]];
            biasChanges[layerNum] = new float[layerSizes[layerNum + 1]];

            //initialize each neuron with z weights
            //z = amount of neurons in next layer
            for (int neuronNum = 0; neuronNum < layerSizes[layerNum + 1]; neuronNum++)
            {
                biases[layerNum][neuronNum] = 0;
            }
        }
    }
    ///////////////// end trainInit







    ///////////////////////initialization for when weights and biases are already trained
    public void loadInitialization()
    {

        //////// get data from .dat files
        string weightsPath = Application.dataPath + "/Resources/NNData/weights45.dat";
        string biasesPath = Application.dataPath + "/Resources/NNData/biases45.dat";
        string layersPath = Application.dataPath + "/Resources/NNData/layers45.dat";

        //set layers
        using (BinaryReader reader = new BinaryReader(File.Open(layersPath, FileMode.Open)))
        {
            layerSizes = new int[reader.BaseStream.Length / 4];
            int pos = 0;
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                int value = reader.ReadInt32(); // reads ints
                //Debug.Log("Layer size = " + value + " at layer " + pos);
                layerSizes[pos] = value;
                pos++;
            }
        }

        //initialize neurons and activations with x layers 
        neurons = new float[layerSizes.Length][];
        neuronsPreActivation = new float[layerSizes.Length][];
        neuronDerivs = new float[layerSizes.Length][];
        for (int layerNum = 0; layerNum < layerSizes.Length; layerNum++)
        {
            //initialize each layer with y neurons
            neurons[layerNum] = new float[layerSizes[layerNum]];
            neuronsPreActivation[layerNum] = new float[layerSizes[layerNum]];
            neuronDerivs[layerNum] = new float[layerSizes[layerNum]];
        }

        for (int i = 0; i < neurons[0].Length; i++)
        {
            neurons[0][i] = inputs[i];
        }



        //set biases
        biases = new float[layerSizes.Length - 1][];
        for (int layerNum = 0; layerNum < layerSizes.Length - 1; layerNum++)
        {
            //intitialize each layer with y neurons
            biases[layerNum] = new float[layerSizes[layerNum + 1]];
        }

        using (BinaryReader reader = new BinaryReader(File.Open(biasesPath, FileMode.Open)))
        {
            int i = 0;
            int j = 0;
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                float value = reader.ReadSingle(); // reads ints
                biases[i][j] = value;
                j++;

                if (j >= layerSizes[i + 1])
                {
                    i++;
                    j = 0;
                }

            }
        }



        //set weights
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
            }
        }

        using (BinaryReader reader = new BinaryReader(File.Open(weightsPath, FileMode.Open)))
        {
            int i = 0;
            int j = 0;
            int k = 0;
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                float value = reader.ReadSingle(); // reads ints
                weights[i][j][k] = value;
                k++;

                if (k >= layerSizes[i + 1])
                {
                    j++;
                    k = 0;

                    if (j >= layerSizes[i])
                    {
                        i++;
                        j = 0;
                    }
                }

            }
        }

    }
    ///////////////////// end loadInit






    public void runNeuralNetwork()
    {
        //clears neuron list to recalculate neurons
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
                //add each neuron*weight from previous layer to new layer neuron
                for (int prevNueronNumb = 0; prevNueronNumb < layerSizes[layerNumb]; prevNueronNumb++)
                {
                    neurons[layerNumb + 1][neuronNumb] += (neurons[layerNumb][prevNueronNumb] * weights[layerNumb][prevNueronNumb][neuronNumb]);
                }

                //add bias to new layer neuron
                neurons[layerNumb + 1][neuronNumb] += biases[layerNumb][neuronNumb];
                neuronsPreActivation[layerNumb + 1][neuronNumb] = neurons[layerNumb + 1][neuronNumb];
                //sigmoid for last layer, ReLU for others
                if (layerNumb == layerSizes.Length - 2)
                {
                    neurons[layerNumb + 1][neuronNumb] = 1 / (1 + Mathf.Exp(-neurons[layerNumb + 1][neuronNumb]));
                }
                else
                {
                    neurons[layerNumb + 1][neuronNumb] = Mathf.Max(0, neurons[layerNumb + 1][neuronNumb]);
                }
            }
        }


    }


    public void backprop()
    {
        int lastLayer = layerSizes.Length - 1;

        //clear neuron derivatives
        for (int l = 0; l < neuronDerivs.Length; l++)
        {
            System.Array.Clear(neuronDerivs[l], 0, neuronDerivs[l].Length);
        }


        //set last layer neuron derivatives
        for (int j = 0; j < layerSizes[lastLayer]; j++)
        {
            neuronDerivs[lastLayer][j] = 2 * (neurons[lastLayer][j] - goals[j]);
        }

        //set all the neuron derivatives starting from 2nd to last and going back
        for (int l = lastLayer - 1; l >= 1; l--)
        {
            for (int j = 0; j < layerSizes[l]; j++)
            {
                for (int m = 0; m < layerSizes[l + 1]; m++)
                {
                    //print($"l: {l}, j {j}, {m} {weights[l+1][0][0]}");
                    neuronDerivs[l][j] += (activationDerivative(neuronsPreActivation[l + 1][m], ((l == layerSizes.Length - 2))))
                                            * (weights[l][j][m])
                                            * (neuronDerivs[l + 1][m]); ////////// weights[l+1][j][m]?
                }
            }
        }


        //set the values to change weights by
        for (int l = 0; l < lastLayer; l++)
        {
            for (int j = 0; j < layerSizes[l]; j++)
            {
                for (int k = 0; k < layerSizes[l + 1]; k++)
                {
                    //print($"l: {l}, j: {j}, k: {k} ---- {weights[l].Length}");
                    weightChanges[l][j][k] -= learnRate
                                        * neurons[l][j]
                                        * activationDerivative(neuronsPreActivation[l + 1][k], (l == layerSizes.Length - 2))
                                        * neuronDerivs[l + 1][k];
                }
            }
        }

        //set the values to change biases by
        for (int l = 0; l < lastLayer; l++)
        {
            for (int j = 0; j < layerSizes[l + 1]; j++)
            {
                biasChanges[l][j] -= learnRate
                                * activationDerivative(neuronsPreActivation[l + 1][j], (l == layerSizes.Length - 2))
                                * neuronDerivs[l + 1][j];
            }
        }


    }



    public void applyBackprop(int batchSize)
    {
        //change weights
        for (int l = 0; l < layerSizes.Length - 1; l++)
        {
            for (int j = 0; j < layerSizes[l]; j++)
            {
                for (int k = 0; k < layerSizes[l + 1]; k++)
                {
                    //print($"l: {l}, j: {j}, k: {k} ---- {weights[l].Length}");
                    weights[l][j][k] += weightChanges[l][j][k] / batchSize;
                    weightChanges[l][j][k] = 0;
                }
            }
        }

        //change biases
        for (int l = 0; l < layerSizes.Length - 1; l++)
        {
            for (int j = 0; j < layerSizes[l + 1]; j++)
            {
                biases[l][j] += biasChanges[l][j]/batchSize;
                biasChanges[l][j] = 0;
            }
        }
    }


    float activationDerivative(float x, bool sigmoid)
    {
        if (sigmoid)
        {
            float s = Mathf.Exp(-x);
            return s / Mathf.Pow(1 + s, 2);
        }
        else
        {
            return (x > 0) ? 1 : 0;
        }
    }

    public float[] getOutput()
    {
        return neurons[layerSizes.Length - 1];
    }


    public void saveNN(int id)
    {
        //save weights,
        //biases, layers, layer sizes

        string weightsPath = Application.dataPath + "/Resources/NNData/weights" + id + ".dat";
        string biasesPath = Application.dataPath + "/Resources/NNData/biases" + id + ".dat";
        string layersPath = Application.dataPath + "/Resources/NNData/layers" + id + ".dat";


        //weights
        using (BinaryWriter writer = new BinaryWriter(
            File.Open(weightsPath, FileMode.Create)))
        {
            for (int x = 0; x < weights.Length; x++)
            {
                for (int y = 0; y < weights[x].Length; y++)
                {
                    for (int z = 0; z < weights[x][y].Length; z++)
                    {
                        writer.Write(weights[x][y][z]);
                    }
                }
            }
        }

        //biases
        using (BinaryWriter writer = new BinaryWriter(
            File.Open(biasesPath, FileMode.Create)))
        {
            for (int x = 0; x < biases.Length; x++)
            {
                for (int y = 0; y < biases[x].Length; y++)
                {
                    writer.Write(biases[x][y]);
                }
            }
        }

        //layers
        using (BinaryWriter writer = new BinaryWriter(
            File.Open(layersPath, FileMode.Create)))
        {
            for (int x = 0; x < layerSizes.Length; x++)
            {
                writer.Write(layerSizes[x]);
            }
        }

        Debug.Log("Saved to: " + Application.dataPath + "/Resources/NNData");

    }


    public void shuffle(int MNISTLength)
    {
        if(shuffledMNISTIndex == null)
        {
            //creates an array [0, 1... MNISTLength-1]
            shuffledMNISTIndex = Enumerable.Range(0, MNISTLength).ToArray();
        }
  
        for(int i = 0; i < MNISTLength; i++)
        {
            //switches each 
            int j = Random.Range(0, MNISTLength);
            int temp = shuffledMNISTIndex[i];
            shuffledMNISTIndex[i] = shuffledMNISTIndex[j];
            shuffledMNISTIndex[j] = temp;
        }
    }

    public void changeInputs(float[] newInputs)
    {
        for (int i = 0; i < neurons[0].Length; i++)
        {
           inputs[i] = newInputs[i];
           neurons[0][i] = inputs[i];
        }
    }

    public void changeGoals(float[] newGoals)
    {
        for (int i = 0; i < goals.Length; i++)
        {
            goals[i] = newGoals[i];
        }
    }
}
