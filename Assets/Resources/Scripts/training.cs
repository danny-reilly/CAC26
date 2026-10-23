using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AdaptivePerformance.Provider;
using UnityEngine.UIElements;


public class training : MonoBehaviour
{
    const int maxEMNIST = 697932;
    const int maxTestEMNIST = 116323;
    const int maxMNIST = 60000;
    const int maxTestMNIST = 10000;
    int maxTrainImg, maxTestImg;
    int[] inputsInt;
    float[] inputs;
    [SerializeField] int[] hiddenLayers;
    [SerializeField] int charNum; //number of chars (num = 10, num&let = 62)
    static float[] goals; 
    int imgLabel;
    List<int> topChars;

    [SerializeField] bool train = true;
    [SerializeField] float learnRate = 0.01f;
    [SerializeField] int batchSize = 100;
    [SerializeField] int epochNum = 20;

    neuralNetwork nn;
    MNISTRead mr;
    DrawDigits dd;
    imgGenerator ig;

    VisualElement uiRoot;
    public void Start()
    {
        if(charNum == 10)
        {
            maxTestImg = maxTestMNIST;
            maxTrainImg = maxMNIST;
        } else if(charNum == 62)
        {
            maxTestImg = maxTestEMNIST;
            maxTrainImg = maxEMNIST;
        }
        
        uiRoot = GameObject.Find("UIDocument").GetComponent<UIDocument>().rootVisualElement;

        ig = new imgGenerator();        

        //get an image from mnist and take data from the image
        mr = new MNISTRead();
        mr.Init();
        mr.GetLabelAndVals(ref imgLabel, ref inputsInt, 0);

        //set goals and inputs
        goals = goalsToFloats(imgLabel);
        inputs = intsToFloats(inputsInt);


        nn = new neuralNetwork(inputs, hiddenLayers, goals, learnRate);

        if (train)
        {
            //train
            nn.trainingInitialization();
            StartCoroutine(mainTrain());
        } else
        {
            /////// load nn
            if (GameObject.Find("Canvas") != null)
            {
                dd = GameObject.Find("Canvas").GetComponent<DrawDigits>();
            }
            nn.loadInitialization();
            nn.runNeuralNetwork();
            updateText();
        }

        //for testing accuracy
        /*
        nn.loadInitialization();
        nn.runNeuralNetwork();
        StartCoroutine(testNetwork());
        */
    }


    IEnumerator mainTrain()
    {
        for (int epoch = 0; epoch < 200; epoch++)
        {
            nn.shuffle(maxTrainImg);
            Debug.Log("john fraction: " + (maxTrainImg / batchSize));
            for (int i = 0; i < maxTrainImg / batchSize; i++)
            {
                for (int j = 0; j < batchSize; j++)
                {
                    //change goals and inputs to match new image
                    mr.GetLabelAndVals(ref imgLabel, ref inputsInt, nn.shuffledMNISTIndex[i * batchSize + j]);
                    nn.changeGoals(goalsToFloats(imgLabel));
                    nn.changeInputs(intsToFloats(inputsInt));

                    //run and backprop
                    nn.runNeuralNetwork();
                    nn.backprop();
                }
                nn.applyBackprop(batchSize);
                //Debug.Log("batch " + i + "/" + (maxTrainImg / batchSize) + " complete (" + (epoch + 1) + "/" + epochNum + ")");
                yield return null;
            }

            nn.saveNN(epoch);
            StartCoroutine(testNetwork(epoch));
        }

    }




    void Update()
    {
        if (!train)
        {
            if (dd != null)
            {
                nn.changeInputs(intsToFloats(dd.centeredGridArray));
            }
            nn.runNeuralNetwork();
            updateText();
        }
    }

    float[] goalsToFloats(int num)
    {
        float[] goals = new float[charNum];
        for (int i = 0; i < charNum; i++)
        {
            goals[i] = (i == num) ? 1.0f : 0.0f;
        }
        return goals;
    }

    //converts MNIST 0-255 data to 0-1 floats
    float[] intsToFloats(int[] list)
    {
        float[] newList = new float[list.Length];
        for (int i = 0; i < list.Length; i++)
        {
            newList[i] = list[i] / 255f;
        }
        return newList;
    }


    void updateText()
    {
        topChars = Enumerable.Range(0, charNum)
            .OrderBy(index => -nn.getOutput()[index]) // Change to OrderByDescending for high-to-low
            .ToList();
        for (int i = 0; i < 10; i++)
        {
            Debug.Log(nn.getOutput()[61]);
            uiRoot.Q<Label>(i + "").text =
            mr.GetCharFromLabel(topChars[i]) + ": " + Mathf.Round(10000 * nn.getOutput()[topChars[i]]) / 100 + "%";
        }
    }




    IEnumerator testNetwork(int epoch) {
        int correctNum = 0;
        for (int i = 0; i < maxTestImg; i++)
        {
            mr.GetLabelAndVals(ref imgLabel, ref inputsInt, i);
            //imgLabel;
            nn.changeInputs(intsToFloats(inputsInt));
            nn.runNeuralNetwork();


            float[] outputs = nn.getOutput();
            int best = 0;
            float bestValue = 0;
            for(int j = 0; j < 10; j++)
            {
                if (outputs[j] > bestValue)
                {
                    bestValue = outputs[j];
                    best = j;
                }
            }

            //if guess was correct
            if(best == imgLabel)
            {
                Debug.Log("correct (" + i + "/10000)");
                correctNum++;
            } else
            {
                Debug.Log("incorrect (" + i + "/10000), (correct = " + imgLabel + ")");
            }
        }
        Debug.Log(correctNum + "/" + maxTestImg + "correct for #" + epoch + ", (" + ((float)correctNum /maxTestImg) + "%)");
        yield return null;
    }
}