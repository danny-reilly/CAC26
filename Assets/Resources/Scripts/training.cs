using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AdaptivePerformance.Provider;
using UnityEngine.UIElements;


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
    DrawDigits dd;
    imgGenerator ig;

    VisualElement uiRoot;
    public void Start()
    {

        uiRoot = GameObject.Find("UIDocument").GetComponent<UIDocument>().rootVisualElement;

        ig = new imgGenerator();        

        //get an image from mnist and take data from the image
        mrt = new MNISTReadTest();
        mrt.Init();
        mrt.GetLabelAndVals(ref imgLabel, ref inputsInt, 0);//GetComponent<imageGen>().imgNum);

        //set goals and inputs
        goals = goals09(imgLabel);
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
                //Debug.Log("batch " + i + "/" + 60000 / batchSize + " complete (" + (epoch + 1) + "/" + epochNum + ")");
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
                nn.changeInputs(intsToFloats(dd.convertArray));
            }
            nn.runNeuralNetwork();
            updateText();
        }
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
        for (int i = 0; i < list.Length; i++)
        {
            newList[i] = list[i] / 255f;
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




    IEnumerator testNetwork(int epoch) {
        int correctNum = 0;
        for (int i = 0; i < 10000; i++)
        {
            mrt.GetLabelAndVals(ref imgLabel, ref inputsInt, i);
            //imgLabel;
            nn.changeInputs(intsToFloats(inputsInt));
            nn.runNeuralNetwork();
            //Texture2D texture = ig.createImg(28, inputsInt);
            //GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 28 / 10f);
            //updateText();

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
                //Debug.Log("correct (" + i + "/10000)");
                correctNum++;
            } else
            {
                //Debug.Log("incorrect (" + i + "/10000), (correct = " + imgLabel + ")");
            }

        }
        Debug.Log(correctNum + "/10000 correct for #" + epoch);
        yield return null;
    }
}