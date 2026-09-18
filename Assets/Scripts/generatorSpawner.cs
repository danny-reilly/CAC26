using UnityEngine;

public class generatorSpawner : MonoBehaviour
{
    public GameObject imageGenerator;
    bool restarted = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        newGen(imageGenerator, true);

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
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

            print(imageGenerator.GetComponent<neuralNetwork>().score);
            newGen(imageGenerator, false);

            foreach (Transform gen in transform)
            {
                if(gen.name == "old")
                {
                    Destroy(gen.gameObject);
                }
            }

            restarted = false;
        }
    }

    void newGen(GameObject best, bool start)
    {
        for (int i = 0; i < 50; i++)
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
            }

            

            newGen.transform.position = new Vector3(-10 + i * 3.5f, 0, 0);
            newGen.transform.parent = transform;
            
            if(!start)
            {
                newGen.GetComponent<neuralNetwork>().mutateWeights(0.1f);
                newGen.GetComponent<neuralNetwork>().runNeuralNetwork();
            }
            
        }
    }
}
