using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AdaptivePerformance.Provider;
using UnityEngine.UIElements;


public class runNetwork : MonoBehaviour
{

    int[] inputsInt;
    float[] inputs;
    public int charNum; //number of chars (num = 10, num&let = 62)
    static float[] goals; 
    int imgLabel;
    [HideInInspector]
    public List<int> topChars;
    neuralNetwork nn;
    DrawDigits dd;
    imgGenerator ig;
    public guessWord gw;
    public Tutorial tut;

    VisualElement uiRoot;
    public TMP_Text leaderboardText;
    public string startingLeaderboardText;
    public void Start()
    {
        startingLeaderboardText = leaderboardText.text; ////////////////////////////////////////////////////////////
    
        uiRoot = GameObject.Find("UIDocument").GetComponent<UIDocument>().rootVisualElement;

        ig = new imgGenerator();        



        int[] empty = new int[0];
        float[] goals = new float[26];
        float[] inputs = new float[784];
        nn = new neuralNetwork(inputs, empty, goals, 0);

        /////// load nn
        dd = GameObject.Find("Canvas").GetComponent<DrawDigits>();
        nn.loadInitialization();
        nn.runNeuralNetwork();
        updateText();
    }


    void Update()
    {
        nn.changeInputs(intsToFloats(dd.centeredGridArray));
        if (!dd.convertArray.All(i => i == 0) && !gw.guessingWord && !tut.givingTutorial)
        {
            nn.runNeuralNetwork();
            updateText(); 
        } else if(tut.givingTutorial)
        {
            dd.erase();
            if (gw.guessingWord)
            {
                gw.guessList.Clear();
                gw.guessWordToggle();
            }
            leaderboardText.text = "No pressing buttons\nduring the tutorial!";
        } else if(gw.guessingWord)
        {
            nn.runNeuralNetwork();
            updateText();
            leaderboardText.text = gw.finalWord.Length <= 0 ? startingLeaderboardText : $"Was your word: \"{gw.finalWord}\" ?";
        } else
        {
            nn.runNeuralNetwork();
            updateText(); 
            leaderboardText.text = "No Guesses While\nWriting Word";
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
        string tempLeaderboard = "";
        topChars = Enumerable.Range(0, charNum)
            .OrderBy(index => -nn.getOutput()[index]) // Change to OrderByDescending for high-to-low
            .ToList();
        for (int i = 0; i < 5; i++)
        {
            //uiRoot.Q<Label>(i + "").text =
            if(Mathf.Round(10000 * nn.getOutput()[topChars[i]]) <= 0|| gw.guessingWord)
            {
                continue;
            }
            if(i != 0) {
                tempLeaderboard += $"<color=#173717>{GetCharFromLabelLet(topChars[i])} ({Mathf.Round(10000 * nn.getOutput()[topChars[i]]) / 100}% Certainty)</color>\n";
            } else
            {
                tempLeaderboard += $"<size=55>{GetCharFromLabelLet(topChars[i])} ({Mathf.Round(10000 * nn.getOutput()[topChars[i]]) / 100}% Certainty)\n</size>";
            }
            
            
        }
        leaderboardText.text = tempLeaderboard;
    }

    char GetCharFromLabelLet(int label)
    {
        return (char)('A' + label); //retrun A-Z
    }
}