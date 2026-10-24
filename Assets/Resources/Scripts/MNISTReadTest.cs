using UnityEngine;
using System.Collections.Generic;

public class MNISTReadTest
{

    // Of course, all of the below is a formatted CSV, and by default it is just a string, commas separating collums and newlines separating rows

    // MNIST (in CSV format) is basically 1 big table like a google sheet where the first collumn is the label (what number the image should be) and the last 784 are the 0-255 representing the brightness of the pixel.
    // In the last 784 collumns, the format iterates between the pixels like (1, 1) (1, 2) (1, 3) ........ (1, 28) (2, 1) (2, 2) all the way to (28, 28)

    public TextAsset MNISTData;
    public TextAsset extendedMNISTData;
    public int label;
    public int[] vals;
    string[] rows;

    public void Init() //init instead of start becuase start runs after GetLabelAndVals is called
    {
        MNISTData = Resources.Load<TextAsset>("MNIST/mnist_train");
        extendedMNISTData = Resources.Load<TextAsset>("MNIST/emnist-byclass-train");
        rows = MNISTData.text.Split("\n"); // list with each index being a 785 long string with comma separators, first row is headings
        //string[] gib = extendedMNISTData.text.Split("\n"); // did this as a test and it took 15 seconds to load
    }

    public void GetLabelAndVals(ref int labelPtr, ref int[] valsPtr, int index) // pointer allows you to reference the var you pass in
    {
        string[] currentRow = rows[index+1].Split(","); // +1 accounts for heading row

        int localLabel;
        int.TryParse(currentRow[0], out localLabel);
        int[] localVals = new int[784];

        for(int i = 0; i < currentRow.Length-1; i++)
        {
            int.TryParse(currentRow[i+1], out localVals[i]);
        }

        labelPtr = localLabel;
        valsPtr = localVals;
    }
}
