using UnityEngine;
using System.Collections.Generic;

public class MNISTRead : MonoBehaviour
{
    int[] byMergeLowercase = { 97, 98, 100, 101, 102, 103, 104, 110, 113, 114, 116 };
    // Of course, all of the below is a formatted CSV, and by default it is just a string, commas separating collums and newlines separating rows

    // MNIST (in CSV format) is basically 1 big table like a google sheet where the first collumn is the label (what number the image should be) and the last 784 are the 0-255 representing the brightness of the pixel.
    // In the last 784 collumns, the format iterates between the pixels like (1, 1) (1, 2) (1, 3) ........ (1, 28) (2, 1) (2, 2) all the way to (28, 28)


    public int label;
    public int[] vals;
    string[] rows;
    string[] rowsTest;

    public void Init() //init instead of start becuase start runs after GetLabelAndVals is called
    {
        rows = Resources.Load<TextAsset>("MNIST/emnist_letters_train").text.Split("\n"); // list with each index being a 785 long string with comma separators, first row is headings
        rowsTest = Resources.Load<TextAsset>("MNIST/emnist_letters_test").text.Split("\n");
        //Debug.Log(rows.Length);
    }

    public void GetLabelAndVals(ref int labelPtr, ref int[] valsPtr, int index, bool training) // pointer allows you to reference the var you pass in
    {
        string[] currentRow;
        if (training)
        {
            currentRow = rows[index + 1].Split(",");
        } else
        {
            currentRow = rowsTest[index + 1].Split(","); // +1 accounts for heading row
        }

        int localLabel;
        int.TryParse(currentRow[0], out localLabel);
        int[] localVals = new int[784];

        for(int i = 0; i < currentRow.Length-1; i++)
        {
            int.TryParse(currentRow[i+1], out localVals[i]);
        }

        labelPtr = localLabel;
        valsPtr = localVals;

        //Debug.Log(labelPtr + ": " + GetCharFromLabel(labelPtr));
    }


    public char GetCharFromLabelByclass(int label)
    {
        if (label < 10)
        {
            return (char)('0' + label); //returns 0-9
        }
        else if (label < 36)
        {
            return (char)('A' + (label - 10)); //retrun A-Z
        } else
        {
            return (char)(byMergeLowercase[label - 36]);
            //return (char)('a' + (label - 36));
        }
    }

    public char GetCharFromLabelLet(int label)
    {
        return (char)('A' + label); //retrun A-Z
    }
}
