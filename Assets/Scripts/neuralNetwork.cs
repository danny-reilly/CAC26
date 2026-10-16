using UnityEngine;

public class neuralNetwork : MonoBehaviour
{
    static float[] inputs = { 0.3f, -0.8f, 0.4f, 1f, 0.02f, 0.52f };
    static int[] hiddenLayers = { 20, 25, 20, 15 };
    static float[] goals = { 0.4f, 0.2f, 0.3f, 0.4f };
    static float learnRate = 0.2f;

    NN nn = new NN(inputs, hiddenLayers, goals, learnRate);

    public void Start()
    {

        nn.initialize();

        for (int a = 0; a < 100; a++)
        {

            nn.runNeuralNetwork();
            print("Gen " + a + ": " + nn.getOutput()[0]);
            nn.backprop();
        }

    }
}

public class NN
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

    float[][] neuronsPreActivation;
    float[][] neuronDerivs;

    float[] goals;


    //how fast gradient decent happens (if it's too high or low, it will go towards infinity or not move and never reach the expected value)
    float learnRate;

    //neurons[x][y], x = layer num, y = which neuron in layer
    //weights[x][y][z], x = layer num, y = which neuron in layer, z = what neuron in the next layer its connected to

    public NN(float[] inputs, int[] hiddenLayers, float[] goals, float learnRate)
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

    public void initialize()
    {
        ///////////////////////////////////////////////////////////////// initialization /////////////////////////////////////////////////////////////////
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
                    //(uses He initialization to scale for different layer sizes)
                    float maxWeight = Mathf.Sqrt(6.0f / layerSizes[layerNum]);
                    weights[layerNum][neuronNum][weightNum] = Random.Range(-maxWeight, maxWeight);
                    //print(weights[layerNum][neuronNum][weightNum]);
                }
            }
        }

        //initialize biases
        biases = new float[layerSizes.Length - 1][];
        for (int layerNum = 0; layerNum < layerSizes.Length - 1; layerNum++)
        {
            //intitialize each layer with y neurons
            biases[layerNum] = new float[layerSizes[layerNum + 1]];

            //initialize each neuron with z weights
            //z = amount of neurons in next layer
            for (int neuronNum = 0; neuronNum < layerSizes[layerNum + 1]; neuronNum++)
            {
                biases[layerNum][neuronNum] = 0;
            }
        }
    }




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
                    weights[l][j][k] -= learnRate
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
                biases[l][j] -= learnRate
                                * activationDerivative(neuronsPreActivation[l + 1][j], (l == layerSizes.Length - 2))
                                * neuronDerivs[l + 1][j];
            }
        }


    }

    public float activationDerivative(float x, bool sigmoid)
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
}
