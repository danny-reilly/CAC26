using UnityEngine;
using UnityEngine.UIElements;

public class generatorSpawner : MonoBehaviour
{
    public GameObject imageGenerator;
    float cd;
    int genNum = 0;
    float mutateChance = 3f;
    float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 10f;
        newGen(imageGenerator, true);


        var uiDocument = GameObject.Find("UIDocument").GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        // Query your slider by its name or type
        Slider mySlider = root.Q<Slider>("mutateSlider");
        mySlider.RegisterValueChangedCallback(OnSliderValueChanged);

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(cd < 0)
        {
            cd = 0.001f;
            float score = 0;
            foreach(Transform gen in transform)
            {
                gen.GetComponent<neuralNetwork>().runNeuralNetwork();
                //print(gen.GetComponent<neuralNetwork>().score);
                if(gen.GetComponent<neuralNetwork>().score > score)
                {
                    score = gen.GetComponent<neuralNetwork>().score;
                }
            }



            foreach (Transform gen in transform)
            {
                gen.name = "old";
                if (gen.GetComponent<neuralNetwork>().score == score)
                {
                    imageGenerator = gen.gameObject;
                }
            }

            genNum++;
            GameObject.Find("UIDocument").GetComponent<UIDocument>().rootVisualElement.Q<Label>("genTxt").text = "Generation " + genNum + ": " + Mathf.Round(100*imageGenerator.GetComponent<neuralNetwork>().score/1875f) + "% Match";
            if(genNum % 100 == 0)
            {
                print("100 gens in " + timer/10 + "s");
                timer = 0;
            }

            newGen(imageGenerator, false);

            foreach (Transform gen in transform)
            {
                if(gen.name == "old")
                {
                    Destroy(gen.gameObject);
                }
            }

        } else
        {
            cd -= Time.deltaTime;
        }
    }

    void newGen(GameObject best, bool start)
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject newGen = Instantiate(best);
            neuralNetwork nn = newGen.GetComponent<neuralNetwork>();
            if(start)
            {
                nn.initialize();
            } else
            {
                nn.weights = best.GetComponent<neuralNetwork>().cloneWeights();
                nn.neurons = best.GetComponent<neuralNetwork>().cloneNeurons();
                nn.biases = best.GetComponent<neuralNetwork>().cloneBiases();
            }

            

            newGen.transform.position = new Vector3(4 + i * 13.5f, 0, 0);
            newGen.transform.parent = transform;
            
            if(!start)
            {
                //keep unmutated version of parent in next gen
                if(i != 0)
                {
                    newGen.GetComponent<neuralNetwork>().mutateWeights(mutateChance);
                }
                newGen.GetComponent<neuralNetwork>().runNeuralNetwork();
            }
            
        }
    }


    private void OnSliderValueChanged(ChangeEvent<float> evt)
    {
        mutateChance = evt.newValue;
        GameObject.Find("UIDocument").GetComponent<UIDocument>().rootVisualElement.Q<Label>("mutateTxt").text = "Mutate Chance: " + mutateChance;
    }

}
