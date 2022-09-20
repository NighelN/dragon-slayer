using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RekenOPsBackEnd : MonoBehaviour
{
    private int num1, num2;
    private int calcoperator; //0 = -, 1 = +
    private bool[] isCorrect = new bool[4];

    public Text equationText;
    public int[] FieldText = new int[4];
    public Text[] buttonText;
    public int result;

    void Start()
    {
        num1 = Random.Range(0, 100);
        num2 = Random.Range(0, 100);
        calcoperator = Random.Range(0, 1);
        if (calcoperator == 0) { result = num1 - num2; equationText.text = num1 + " - " + num2; }
        else { result = num1 + num2; equationText.text = num1 + " + " + num2; }
        for (int i = 0; i < isCorrect.Length; i++)
        {
            isCorrect[i] = false;
        }
        isCorrect[Random.Range(0, 3)] = true;
        for (int i = 0; i < FieldText.Length; i++)
        {
            if (!isCorrect[i])
            {
                int calcoperator; //0 = -, 1 = +
                calcoperator = Random.Range(0, 1);
                if (calcoperator == 0) { FieldText[i] = result - Random.Range(1, 10); }
                else { FieldText[i] = result + Random.Range(1, 10); }
            }
            else { FieldText[i] = result; }
            buttonText[i].text = FieldText[i].ToString();
        }
        Debug.Log(result);
    }
}
