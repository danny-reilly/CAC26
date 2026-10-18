using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;


public class training : MonoBehaviour
{
    int[] inputsInt; //= { 0.3f, -0.8f, 0.4f, 1f, 0.02f, 0.52f };
    float[] inputs;
    [SerializeField] int[] hiddenLayers = { 20, 25, 20, 15 };
    static float[] goals; //= {0.67f};
    int imgLabel;
    List<int> topChars;
    int gen = 0;

    [SerializeField] float learnRate = 0.2f;

    NN nn;
    MNISTReadTest mrt = new MNISTReadTest();

    VisualElement uiRoot; 
    public void Start()
    {

        uiRoot = GameObject.Find("UIDocument").GetComponent<UIDocument>().rootVisualElement;

        /////// train nn

        //get an image from mnist and take data from the image
        mrt.Init();
        mrt.GetLabelAndVals(ref imgLabel, ref inputsInt, 0);

        //set goals and inputs
        goals = goals09(imgLabel);
        inputs = intsToFloats(inputsInt);

        nn = new NN(inputs, hiddenLayers, goals, learnRate);
        nn.trainingInitialization();

        /////// load nn
        /*nn.loadInitialization();
        nn.runNeuralNetwork();
        print("output: " + nn.getOutput()[0]);*/
    }

    void Update()
    {
        print("Gen " + gen);
        gen++;
        nn.runNeuralNetwork();
        //print("Gen " + a + ": " + nn.getOutput()[0]);
        nn.backprop();

        topChars = Enumerable.Range(0, 10)
            .OrderBy(index => -nn.getOutput()[index]) // Change to OrderByDescending for high-to-low
            .ToList();
        for (int i = 0; i < 10; i++)
        {
            uiRoot.Q<Label>(i + "").text =
            topChars[i] + ": " + Mathf.Round(10000 * nn.getOutput()[topChars[i]])/100 + "%";
        }
        //nn.saveNN();
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
}