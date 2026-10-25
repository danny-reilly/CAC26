using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Tutorial : MonoBehaviour
{

    public TextMeshProUGUI speechText;
    string text;
    public string[] voicelines = {"Hi, I'm John,  but my friends call me Reis.", "line2"};
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
            tutButtonText.text = "Start Tutorial";            
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
    }






    void Update()
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
            //update text here
            UpdateText(voicelines[voiceline], 0.05f);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            canSwitch = true;
            if (switchCd > 0)
            {
                Skip();
            }
        }

    }


    void Skip()
    {
        speechText.text = text;
        typing = false;
        index = 0;
        switchCd = 0;
        canSwitch = false;
    }



    void NewChar()
    {

        if (index < text.Length)
        {
            if (text[index].ToString() == " ")
            {
                charCd = 0;
            }
            else
            {
                //when we have sfx
                //As.pitch = Random.Range(0.6f + pitch, 0.8f + pitch);
                //As.PlayOneShot(As.clip);
            }
            speechText.text += text[index];
            index++;
            charCd = typeSpeed;
            //take longer on ts
            if (text[index - 1].ToString() == "." || text[index - 1].ToString() == "!" || text[index - 1].ToString() == "?" || text[index - 1].ToString() == "-")
            {
                charCd *= 4;
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
