using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class guessWord : MonoBehaviour
{
    public runNetwork runNetwork;
    public DrawDigits dd;
    public Tutorial tut;
    public List<char> guessList = new List<char>();
    public string finalWord;

    public bool guessingWord = false;
    public Button next, done, space;
    public TMP_Text toggleText;
    public GameObject rightWrongObj;
    johnMovement john;

    void Start()
    {
        john = GameObject.Find("John").GetComponent<johnMovement>();
    }

    public void onNextLetterClick()
    {
        guessList.Add((char)('A' + runNetwork.topChars[0]));
        dd.erase();
        toggleText.text = guessingWord ? "Guess a Word" : "Guess a Letter";

    }
    public void onDoneWWordClick()
    {
        dd.erase();
        finalWord = "";
        guessingWord = false;
        next.gameObject.SetActive(false);
        done.gameObject.SetActive(false);
        space.gameObject.SetActive(false);
        if(guessList.Count == 0) return;
        foreach(char letter in guessList)
        {
            finalWord += letter;
        }
        runNetwork.leaderboardText.text = $"Was your word: \"{finalWord}\" ?";
        rightWrongObj.SetActive(true);
        guessList.Clear();
        toggleText.text = guessingWord ? "Guess a Word" : "Guess a Letter";
    }

    public void AddSpace()
    {
        guessList.Add(' ');
        dd.erase();
    }

    public void guessWordToggle()
    {
        dd.erase();
        guessingWord = !guessingWord;
        next.gameObject.SetActive(guessingWord);
        done.gameObject.SetActive(guessingWord);
        space.gameObject.SetActive(guessingWord);
        toggleText.text = guessingWord ? "Guess a Letter" : "Guess a Word";
    }

    public void WordWasRightWrong(bool correct)
    {
        tut.tutorialObj.SetActive(true);
        tut.givingTutorial = false;
        tut.guessWordBtn.interactable = false;
        tut.eraseBtn.interactable = false;
        tut.voiceline = 0;
        tut.tutButtonText.text = "Restart Tutorial";
        rightWrongObj.SetActive(false);

        if(correct)
        {
            tut.UpdateText(tut.rightLines[tut.voiceline], 0.05f);
            john.ChangeJohnSprite("Happy");
            tut.correct = true;
            tut.wrong = false;
        } else
        {
            tut.UpdateText(tut.wrongLines[tut.voiceline], 0.05f);
            john.ChangeJohnSprite("Sad");
            tut.correct = false;
            tut.wrong = true;
        }
    }

}
