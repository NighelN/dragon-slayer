using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenAnimation : MonoBehaviour
{
    public GameObject[] buttons;
    public Text[] buttonText;
    public Transform[] pointerTarget;
    public GameObject pointer;
    public GameObject pointerSprite;


    private int selectedButton;
    private int pointerSpeed;
    float buttonSwitchDelay;

    InputManager InputScript;

    private void Start()
    {
        selectedButton = 0;
        pointer.transform.position = pointerTarget[0].position;
        InputScript = GetComponent<InputManager>();
        Destroy(GameObject.Find("GameManager"));
    }

    private void Update()
    {
        //Button input en logica (input werkt alleen als een pointer animatie niet bezig is)
        if (Mathf.Round(pointer.transform.position.y * 100) / 100.0 == Mathf.Round(pointerTarget[selectedButton].position.y * 100) / 100.0)
        {
            pointerSpeed = 7;
            if (Input.GetKeyDown("return") || InputScript.aButtonPressed)
            {
                if (selectedButton == 0) { StartGame(); }
                if (selectedButton == 2) { EndGame(); }
                if (selectedButton == 1) { Controls(); }
            }
            if (InputScript.verticalMovement == 1 && Time.time >= buttonSwitchDelay)
            {
                if (selectedButton == 0) { selectedButton = 2; pointerSpeed = 20; }
                else if (selectedButton == 1) { selectedButton = 0; }
                else if (selectedButton == 2) { selectedButton = 1; }
                buttonSwitchDelay = Time.time + 0.25f;
            }
            else if (InputScript.verticalMovement == -1 && Time.time >= buttonSwitchDelay)
            {
                if (selectedButton == 0) { selectedButton = 1; }
                else if (selectedButton == 1) { selectedButton = 2; }
                else if (selectedButton == 2) { selectedButton = 0; pointerSpeed = 20; }
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
            for (int i = 0; i < 3; i++)
            {
                if (i != selectedButton)
                {
                    //reset scale en kleur voor de buttons
                    buttonText[i].transform.localScale = new Vector2(1f, 1f);
                    buttonText[i].color = Color.yellow;
                }
                else
                {
                    //Oscillerende scale effect en rood kleur voor de buttons
                    buttonText[i].transform.localScale = new Vector2(1.4f, 1.4f) + new Vector2(0.1f, 0.1f) * Mathf.Sin(Time.time * 3f);
                    buttonText[i].color = Color.red;
                }
            }
        }
    }
    private void StartGame()
    {
        pointerSprite.SetActive(false);
        SceneManager.LoadScene(2);
    }
    private void EndGame()
    {
        Application.Quit();
    }
    private void Controls()
    {
        SceneManager.LoadScene("Controls");
    }
}