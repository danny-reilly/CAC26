using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Tutorial : MonoBehaviour
{

    public TextMeshProUGUI speechText;
    string text;
    string[] voicelines = 
    {
        "Woah, didn't see you there!<Color=#555555>\n(Press Q to continue)</Color>", 
        "Hi, I'm John Albertson, but my friends call me Al.<Color=#555555>\n(Press Q to continue)</Color>", 
        "This is my AI studio, draw any letter and it will identify it with over 90% accuracy!<Color=#555555>\n(Press Q to continue)</Color>",
        "Draw on this canvas, and the chalkboard will show you what it thinks your letter is.<Color=#555555>\n(Press Q to continue)</Color>",
        "Press 'E', or the Erase Button to clear the canvas.<Color=#555555>\n(Press Q to continue)</Color>",
        "If you want it to guess a word or phrase you write, click on the 'Guess A Word' button.<Color=#555555>\n(Press Q to continue)</Color>",
        "After clicking the 'Guess A Word' button, draw a letter and press the 'Add Current Letter' button. If you want to add a space, click 'Add Space.'<Color=#555555>\n(Press Q to continue)</Color>",
        "Once you are all done with your word or phrase, click 'Done.'"
    };

    public string[] rightLines =
    {
        "Yes! I knew my AI studio could do it!"
    };

    public string[] wrongLines =
    {
        "Aw drat. I guess you can't win em all...<Color=#555555>\n(Press Q to continue)</Color>",
        "If you try again, maybe it'll get it right next time!"
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
    public int voiceline = 0;
    int index = 0;
    float typeSpeed;
    public bool typing;
    float charCd = 0; // cd = current dialogue?
    float switchCd = 0;
    string lastChar;
    bool  canSwitch;
    Animator theAnimator, bananimator, thatAnimator;
    public GameObject tutorialObj;
    public bool givingTutorial, correct, wrong;
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
        }
        else
        {
            voiceline = voicelines.Length;
            tutButtonText.text = "Restart Tutorial";
            typing = false;
            john.ChangeJohnSprite("Explain");
        }
    }
    
    public void UpdateText(string newSpeech, float speed)
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
            if (Input.anyKeyDown)
            {
                canSwitch = true;
                if (switchCd > 0)
                {
                    Skip();
                }
            }

            john.talking = typing;
        } else if (correct) {
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
                if (voiceline >= rightLines.Length-1)
                {
                    OnTutorialClick();
                    OnTutorialClick();

                }
                else
                {
                    //update text here
                    UpdateText(rightLines[voiceline], 0.05f);
                }
            }
            if (Input.anyKeyDown)
            {
                canSwitch = true;
                if (switchCd > 0)
                {
                    Skip();
                }
            }
            john.talking = typing;
        }else if (wrong) {
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
                if (voiceline >= wrongLines.Length-1)
                {
                    OnTutorialClick();
                    OnTutorialClick();
                }
                else
                {
                    //update text here
                    UpdateText(wrongLines[voiceline], 0.05f);
                }
            }
            if (Input.anyKeyDown)
            {
                canSwitch = true;
                if (switchCd > 0)
                {
                    Skip();
                }
            }
        }
        john.talking = typing;

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
