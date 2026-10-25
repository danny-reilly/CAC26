using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Tutorial : MonoBehaviour
{

    public TextMeshProUGUI speechText;
    string text;
    string[] voicelines = 
    {
        "Woah, didn't see you there! <Color=#555555>(Press Q to continue)</Color>", 
        "Hi, I'm John Albertson,  but my friends call me Al.", 
        "This is my AI studio, draw any letter and it will identify it with over 90% accuracy!",
        "Draw on this easel, and the chalkboard will show you what it thinks your letter is.",
        "Press 'E', or the Erase Button to clear the canvas.",
        "If you want it to guess a word you write, click on the 'Guess A Word' button.",
        ""
    };

    string[] expressions =
    {
        "Surprised",
        "Explain",
        "Explain2",
        "Point2",
        "Point2",
        "Point"
    };
    int voiceline = 0;
    int index = 0;
    float typeSpeed;
    bool typing;
    float charCd = 0; // cd = current dialogue?
    float switchCd = 0;
    string lastChar;
    bool  canSwitch;
    Animator theAnimator, bananimator, thatAnimator;
    public GameObject tutorialObj;
    public bool givingTutorial;
    public TMP_Text tutButtonText;
    public Button guessWordBtn, eraseBtn;
    public johnMovement john;
    AudioSource As;


    void Start()
    {
        As = GetComponent<AudioSource>();
        OnTutorialClick();
    }
    public void OnTutorialClick()
    {
        givingTutorial = !givingTutorial;
        tutorialObj.SetActive(givingTutorial);
        guessWordBtn.interactable = !givingTutorial;
        eraseBtn.interactable = !givingTutorial;
        if (givingTutorial)
        {
            tutButtonText.text = "End Tutorial";
            voiceline = 0;
            UpdateText(voicelines[voiceline], 0.05f);
        } else
        {
            tutButtonText.text = "Restart Tutorial";            
        }
    }
    
    void UpdateText(string newSpeech, float speed)
    {
        typing = true;
        speechText.text = "";
        text = "";
        index = 0;
        switchCd = 999;


        text = newSpeech;
        typeSpeed = speed;
        john.ChangeJohnSprite(expressions[voiceline]);
    }






    void Update()
    {
        if (givingTutorial)
        {
            switchCd -= Time.deltaTime;
            charCd -= Time.deltaTime;
            if (typing && charCd <= 0)
            {
                charCd = 0;
                NewChar();
            }
            if (!typing && canSwitch)
            {
                canSwitch = false;
                //either end tutorial or go to next voiceline
                if (voiceline >= voicelines.Length-1)
                {
                    OnTutorialClick();
                    john.ChangeJohnSprite("Explain");
                }
                else
                {
                    //update text here
                    UpdateText(voicelines[voiceline], 0.05f);
                }
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                canSwitch = true;
                if (switchCd > 0)
                {
                    Skip();
                }
            }

            john.talking = typing;
        }
    }


    void Skip()
    {
        speechText.text = text;
        typing = false;
        index = 0;
        switchCd = 0;
        canSwitch = false;
        voiceline += voiceline < voicelines.Length - 1 ? 1 : 0;
    }



    void NewChar()
    {

        if (index < text.Length)
        {
            if (text[index] == ' ')
            {
                charCd = 0;
            }
            else
            {
                As.pitch = Random.Range(0.7f, 0.9f);
                As.PlayOneShot(As.clip);
            }
            speechText.text += text[index];
            index++;
            charCd = typeSpeed;
            //take longer on ts
            if (text[index - 1] == '.' || text[index - 1] == '!' || text[index - 1] == '?' || text[index - 1] == '-')
            {
                charCd *= 4;
            } else if (text[index-1] == '<')
            {
                Skip();
            }
        }
        else
        {
            typing = false;
            index = 0;
            switchCd = 0;
            voiceline += voiceline<voicelines.Length-1 ? 1 : 0;
        }
    }
}
