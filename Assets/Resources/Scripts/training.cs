using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AdaptivePerformance.Provider;
using UnityEngine.UIElements;


public class training : MonoBehaviour
{
    const int maxEMNIST = 697932;
    const int maxTestEMNIST = 116323;
    const int maxMNIST = 60000;
    const int maxTestMNIST = 10000;
    const int maxEMNISTLet = 124800;
    const int maxTestEMNISTLet = 20800;
    int maxTrainImg, maxTestImg;
    int[] inputsInt;
    float[] inputs;
    [SerializeField] int[] hiddenLayers;
    public int charNum; //number of chars (num = 10, num&let = 62)
    static float[] goals; 
    int imgLabel;

    [SerializeField] bool train;
    [SerializeField] bool loadTrain;
    [SerializeField] float learnRate = 0.01f;
    [SerializeField] int batchSize = 100;
    [SerializeField] int epochNum = 20;
    [HideInInspector]
    public neuralNetwork nn;
    MNISTRead mr;
    imgGenerator ig;

    VisualElement uiRoot;
    public void Start()
    {
        if(charNum == 10)
        {
            maxTestImg = maxTestMNIST;
            maxTrainImg = maxMNIST;
        } else if(charNum == 62 || charNum == 47)
        {
            maxTestImg = maxTestEMNIST;
            maxTrainImg = maxEMNIST;
        } else if(charNum == 26)
        {
            maxTestImg = maxTestEMNISTLet;
            maxTrainImg = maxEMNISTLet;
        }
        
        uiRoot = GameObject.Find("UIDocument").GetComponent<UIDocument>().rootVisualElement;

        ig = new imgGenerator();        

        //get an image from mnist and take data from the image
        mr = new MNISTRead();
        mr.Init();
        mr.GetLabelAndVals(ref imgLabel, ref inputsInt, 0, true);

        //set goals and inputs
        goals = goalsToFloats(imgLabel);
        inputs = intsToFloats(inputsInt);


        nn = new neuralNetwork(inputs, hiddenLayers, goals, learnRate);

        if (train)
        {
            //train
            if(loadTrain)
            {
                nn.loadInitialization();
            } else
            {
                nn.trainingInitialization();
            }

            StartCoroutine(MainTrain());
        } else
        {
            /////// load nn
            nn.loadInitialization();
            nn.runNeuralNetwork();
            //updateText();
        }
    }


    IEnumerator MainTrain()
    {
        for (int epoch = 0; epoch < epochNum; epoch++)
        {
            nn.shuffle(maxTrainImg);
            for (int i = 0; i < maxTrainImg / batchSize; i++)
            {
                if (i % 50 == 0)
                {
                    StartCoroutine(testNetwork(epoch));
                }

                for (int j = 0; j < batchSize; j++)
                {

                    //change goals and inputs to match new image
                    mr.GetLabelAndVals(ref imgLabel, ref inputsInt, nn.shuffledMNISTIndex[i * batchSize + j], true);
                    if (charNum == 26)
                    {
                        imgLabel--;
                    }
                    nn.changeGoals(goalsToFloats(imgLabel));
                    nn.changeInputs(intsToFloats(inputsInt));

                    //run and backprop
                    nn.runNeuralNetwork();
                    nn.backprop();
                }
                nn.applyBackprop(batchSize);
                Debug.Log("batch " + i + "/" + (maxTrainImg / batchSize) + " complete (" + (epoch + 1) + "/" + epochNum + ")");
                yield return null;
            }

            nn.saveNN(epoch);
            StartCoroutine(testNetwork(epoch));
            yield return null;
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


    public void updateText()
    {
       
    }




    IEnumerator testNetwork(int epoch) {
        int correctNum = 0;
        for (int i = 0; i < maxTestEMNISTLet; i+=10)
        {
            mr.GetLabelAndVals(ref imgLabel, ref inputsInt, i, false);
            if(charNum == 26)
            {
                imgLabel--;
            }
            nn.changeInputs(intsToFloats(inputsInt));
            nn.runNeuralNetwork();


            float[] outputs = nn.getOutput();
            int best = -1;
            float bestValue = -Mathf.Infinity;
            for(int j = 0; j < charNum; j++)
            {
                if (outputs[j] > bestValue)
                {
                    bestValue = outputs[j];
                    best = j;
                }
            }

            //if guess was correct (capital or lowercase)
            if(best == imgLabel || (imgLabel < 36 && best == imgLabel + 26) || (imgLabel >= 36 && best == imgLabel - 26))
            {
                //Debug.Log("correct (" + i + "/" + maxTestImg + ")");
                correctNum++;
            } else
            {
                //Debug.Log("incorrect (" + i + "/" + maxTestImg + "), (" + outputs[imgLabel] + ")");
            }



            /*Texture2D texture = ig.createImg(28, inputsInt);
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 28 / 10f);
            updateText();

            yield return null;*/
        }
        Debug.Log(correctNum + "/" + (maxTestEMNISTLet/10) + "correct for #" + epoch + ", (" + ((float)100*correctNum / (maxTestEMNISTLet/10)) + "%)");
        yield return null;
    }


    IEnumerator TestDataset()
    {
        int[] dataset = new int[47];

        for (int i = 0; i < maxEMNIST-10; i++)
        {
            mr.GetLabelAndVals(ref imgLabel, ref inputsInt, i, false);
            dataset[imgLabel]++;

            if(i%1000 == 0)
            {
                Debug.Log("loading " + i);
                yield return null;
            }
        }

        for (int i = 0; i < charNum; i++)
        {
            Debug.Log(mr.GetCharFromLabelLet(i) + ": " + dataset[i] + ", " + ((float)dataset[i]/maxEMNIST*100) + "%");
            yield return null;
        }

    }

}