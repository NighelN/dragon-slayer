using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class RekenOPsFrontEnd : MonoBehaviour
{
    public GameObject[] buttons;
    public Text[] buttonText;
    public Transform[] pointerTarget;
    public GameObject pointer;
    public GameObject pointerSprite;
    public Image blackPanel;

    private int selectedButton;
    private int pointerSpeed;
    private bool correctAnswerGiven = false;
    private bool isInteractable = true;
    float buttonSwitchDelay;

    RekenOPsBackEnd BackEnd;
    InputManager InputScript;

    private void Start()
    {
        selectedButton = 0;
        pointer.transform.position = pointerTarget[0].position;
        BackEnd = GetComponent<RekenOPsBackEnd>();
        InputScript = GetComponent<InputManager>();
        blackPanel.rectTransform.localScale = new Vector2(Screen.width, Screen.height);
        blackPanel.color = Color.clear;
        pointerSprite.SetActive(true);
    }
    private void Update()
    {
        //Button input en logica (input werkt alleen als een pointer animatie niet bezig is)
        if (Mathf.Round(pointer.transform.position.y * 100) / 100.0 == Mathf.Round(pointerTarget[selectedButton].position.y * 100) / 100.0 && isInteractable)
        {
            pointerSpeed = 7;
            if (Input.GetKeyDown("return") || InputScript.aButtonPressed)
            {
                Debug.Log("je hebt enter gedrukt");
                Debug.Log("Dit is de field text " + BackEnd.FieldText[selectedButton]);
                Debug.Log("dit is de result " + BackEnd.result);
                if (BackEnd.FieldText[selectedButton] == BackEnd.result) { CorrectAnswer(); Debug.Log("Correct"); correctAnswerGiven = true; isInteractable = false; } else { WrongAnswer(); isInteractable = false; }
            }
            if (InputScript.verticalMovement == 1 && Time.time >= buttonSwitchDelay)
            {
                if (selectedButton == 0) { selectedButton = 3; pointerSpeed = 40; }
                else if (selectedButton == 1) { selectedButton = 0; }
                else if (selectedButton == 2) { selectedButton = 1; }
                else if (selectedButton == 3) { selectedButton = 2; }
                buttonSwitchDelay = Time.time + 0.25f;
            }
            else if (InputScript.verticalMovement == -1 && Time.time >= buttonSwitchDelay)
            {
                if (selectedButton == 0) { selectedButton = 1; }
                else if (selectedButton == 1) { selectedButton = 2; }
                else if (selectedButton == 2) { selectedButton = 3; }
                else if (selectedButton == 3) { selectedButton = 0; pointerSpeed = 40; }
                buttonSwitchDelay = Time.time + 0.25f;
            }
        }
        //Pointer verplaatst zich naar de pointer target position van de geselecteerd button
        {
            pointer.transform.position = Vector2.MoveTowards(new Vector2(pointer.transform.position.x, pointer.transform.position.y), pointerTarget[selectedButton].position, pointerSpeed * Time.deltaTime);
        }
        //Oscilerende horizontale shift effect voor de pointer sprite
        {
            pointerSprite.transform.localPosition = new Vector2(.15f * Mathf.Sin(Time.time * 3f), 0);
        }
        {
            if (isInteractable)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i != selectedButton)
                    {
                        //reset scale en kleur voor de buttons
                        buttons[i].transform.localScale = new Vector2(1f, 1f);
                        buttonText[i].color = Color.yellow;
                    }
                    else
                    {
                        //Oscillerende scale effect en rood kleur voor de buttons
                        buttons[i].transform.localScale = new Vector2(1.1f, 1.1f) + new Vector2(0.1f, 0.1f) * Mathf.Sin(Time.time * 3f);
                        buttonText[i].color = Color.red;
                    }
                }
            }
        }
    }
    void CorrectAnswer()
    {
        buttonText[selectedButton].color = Color.green;
        buttons[selectedButton].transform.localScale = new Vector2(1.3f, 1.3f);
        BackEnd.equationText.text = "Correct!";
        StartCoroutine(Wait123());
    }
    void WrongAnswer()
    {
        buttonText[selectedButton].color = Color.grey;
        buttons[selectedButton].transform.localScale = new Vector2(1.3f, 1.3f);
        BackEnd.equationText.text = "Wrong!";
        StartCoroutine(Wait123());
    }
    IEnumerator Wait123()
    {
        Debug.Log("waiting");
        yield return new WaitForSeconds(3);
        if (correctAnswerGiven)
        {
            BackEnd.equationText.gameObject.SetActive(false);
            StartCoroutine(FadeToBlack());
            pointerSprite.SetActive(false);
        }
        else SceneManager.LoadScene(2);
    }
    private IEnumerator FadeToBlack()
    {
        while (blackPanel.color.a < 0.99f)
        {
            blackPanel.color = Color.Lerp(blackPanel.color, Color.black, 7f * Time.deltaTime);
            WaitForSeconds wait = new WaitForSeconds(Time.deltaTime);
            yield return wait;
        }
        { SceneManager.LoadScene(1); }
    }
}
