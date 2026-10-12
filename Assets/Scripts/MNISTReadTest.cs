using UnityEngine;
using System.Collections.Generic;

public class MNISTReadTest : MonoBehaviour
{

    // Of course, all of the below is a formatted CSV, and by default it is just a string

    // MNIST (in CSV format) is basically 1 big table like a google sheet where the first collumn is the label (what number the image should be) and the last 784 are the binary value of the image.
    // In the last 784 collumns, the format iterates between the pixels like (1, 1) (1, 2) (1, 3) ........ (1, 28) (2, 1) (2, 2) all the way to (28, 28)

    public TextAsset baseData;
    
    void Start()
    {
        string[] lines = baseData.text.Split(",");
        for(int i = 0; 1<10; i++)
        {
            print(lines[i]);
        }
    }

    void Update()
    {
        
    }

    /*public int[][] GetImageWithLabel(int index) // index goes top to bottom in the order of rows, with the first row being a heading row 
    {
        string[] values = new string[];
        for (i = index+1;)
    }*/
}
