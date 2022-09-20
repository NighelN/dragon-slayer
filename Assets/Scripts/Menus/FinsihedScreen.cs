using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinsihedScreen : MonoBehaviour
{
    public Text endScore;
    public Text endTime;
    InputManager inputManager;

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        endScore.text = "You scored a total of: " + GameManager.instance.currentScore + " points";
        endTime.text = "You finished the game in a time of: "+(int) GameManager.instance.currentTime+" seconds";
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
