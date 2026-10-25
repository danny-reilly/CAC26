using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class guessWord : MonoBehaviour
{
    public runNetwork runNetwork;
    public DrawDigits dd;
    public List<char> guessList = new List<char>();
    public string finalWord;

    public bool guessingWord = false;
    public Button next, done;
    public TMP_Text toggleText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onNextLetterClick()
    {
        guessList.Add((char)('A' + runNetwork.topChars[0]));
        dd.erase();

    }
    public void onDoneWWordClick()
    {
        dd.erase();
        finalWord = "";
        guessingWord = false;
        next.gameObject.SetActive(false);
        done.gameObject.SetActive(false);
        if(guessList.Count == 0) return;
        foreach(char letter in guessList)
        {
            finalWord += letter;
        }
        runNetwork.leaderboardText.text = $"Was your word: \"{finalWord}\" ?";
        guessList.Clear();
    }

    public void guessWordToggle()
    {
        dd.erase();
        guessingWord = !guessingWord;
        next.gameObject.SetActive(guessingWord);
        done.gameObject.SetActive(guessingWord);
        toggleText.text = !guessingWord ? "Guess a Word" : "Guess a Letter";

    }
}
