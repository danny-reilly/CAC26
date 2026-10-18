using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;
using System.Collections;


public class training : MonoBehaviour
{
    int[] inputsInt; //= { 0.3f, -0.8f, 0.4f, 1f, 0.02f, 0.52f };
    float[] inputs;
    [SerializeField] int[] hiddenLayers = { 20, 25, 20, 15 };
    static float[] goals; //= {0.67f};
    int imgLabel;
    List<int> topChars;

    [SerializeField] bool train = true;
    [SerializeField] float learnRate = 0.2f;
    [SerializeField] int batchSize = 100;
    [SerializeField] int epochNum = 20;

    neuralNetwork nn;
    MNISTReadTest mrt;

    VisualElement uiRoot; 
    public void Start()
    {

        uiRoot = GameObject.Find("UIDocument").GetComponent<UIDocument>().rootVisualElement;

        /////// train nn
        
        //get an image from mnist and take data from the image
        mrt = new MNISTReadTest();
        mrt.Init();
        mrt.GetLabelAndVals(ref imgLabel, ref inputsInt, 0);

        //set goals and inputs
        goals = goals09(imgLabel);
        inputs = intsToFloats(inputsInt);

        nn = new neuralNetwork(inputs, hiddenLayers, goals, learnRate);

        if(train)
        {
            //train
            nn.trainingInitialization();
            StartCoroutine(mainTrain());
        } else
        {
            /////// load nn
            nn.loadInitialization();
            nn.runNeuralNetwork();
            updateText();
        }
    }


    IEnumerator mainTrain()
    {
        for (int epoch = 0; epoch < epochNum; epoch++)
        {
            nn.shuffle();
            for (int i = 0; i < 60000 / batchSize; i++)
            {
                for (int j = 0; j < batchSize; j++)
                {
                    //change goals and inputs to match new image
                    mrt.GetLabelAndVals(ref imgLabel, ref inputsInt, nn.shuffledMNISTIndex[i * batchSize + j]);
                    nn.changeGoals(goals09(imgLabel));
                    nn.changeInputs(intsToFloats(inputsInt));

                    //run and backprop
                    nn.runNeuralNetwork();
                    nn.backprop();
                }
                nn.applyBackprop(batchSize);
                Debug.Log("batch " + i + "/" + 60000 / batchSize + " complete (" + (epoch+1) + "/" + epochNum + ")");
                yield return null;
            }
        }

        nn.saveNN();
    }




    void Update()
    {
        /*nn.runNeuralNetwork();
        //print("Gen " + a + ": " + nn.getOutput()[0]);
        nn.backprop();
        nn.applyBackprop(1);
        //nn.saveNN();*/
    }

    float[] goals09(int num)
    {
        float[] goals = new float[10];
        for (int i = 0; i < 10; i++)
        {
            goals[i] = (i == num) ? 1.0f : 0.0f;
        }
        return goals;
    }

    //converts MNIST 0-255 data to 0-1 floats
    float[] intsToFloats(int[] list)
    {
        float[] newList = new float[list.Length];
        for(int i = 0; i < list.Length; i++)
        {
            newList[i] = list[i]/255f;
        }
        return newList;
    }


    void updateText()
    {
        topChars = Enumerable.Range(0, 10)
            .OrderBy(index => -nn.getOutput()[index]) // Change to OrderByDescending for high-to-low
            .ToList();
        for (int i = 0; i < 10; i++)
        {
            uiRoot.Q<Label>(i + "").text =
            topChars[i] + ": " + Mathf.Round(10000 * nn.getOutput()[topChars[i]]) / 100 + "%";
        }
    }
}