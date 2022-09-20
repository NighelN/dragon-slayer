using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public Text endScore;
    public Text endTime;
    InputManager inputManager;

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        endScore.text = "Ending score: " + GameManager.instance.currentScore;
        endTime.text = "Ending score: " + (int) GameManager.instance.currentTime;
    }

    private void Update()
    {
        if(inputManager.aButtonPressed)
        {
            ReturnToMenu();
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("TItle Screen");
    }
}
