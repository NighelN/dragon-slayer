using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    
    public float horizontalMovement
    {
        get;
        set;
    }

    public float verticalMovement
    {
        get;
        set;
    }

    public bool aButtonPressed
    {
        get;
        set;
    }

    public bool bButtonPressed
    {
        get;
        set;
    }

    public bool xButtonPressed
    {
        get;
        set;
    }

    public bool yButtonPressed
    {
        get;
        set;
    }

    public bool lbButtonPressed
    {
        get;
        set;
    }

    public bool rbButtonPressed
    {
        get;
        set;
    }

    public bool rbButtonHeld
    {
        get;
        set;
    }

    public bool quitButtonPressed
    {
        get;
        set;
    }

    private void Update()
    {
        bool controllerConnected = false;

        //Loops though all connected joysticks
        foreach(string name in Input.GetJoystickNames())
        {
            //Wireless gamepad = nintendo pro controller/joycons
            if(name.Equals("Wireless Gamepad"))
            {
                controllerConnected = true;
            }
        }

        //Nintendo joycon connected
        if(controllerConnected)
        {
            horizontalMovement = Input.GetAxisRaw("LeftJoyStickHorizontalJoycon");
            verticalMovement = Input.GetAxisRaw("LeftJoyStickVerticalJoycon");
            aButtonPressed = Input.GetButtonDown("AButton");
            bButtonPressed = Input.GetButtonDown("BButton");
            xButtonPressed = Input.GetButtonDown("XButton");
            yButtonPressed = Input.GetButtonDown("YButton");
            quitButtonPressed = Input.GetButtonDown("MinusButton");
            lbButtonPressed = Input.GetButtonDown("SLButton");
            rbButtonPressed = Input.GetButtonDown("SRButton");
            rbButtonHeld = Input.GetButton("SRButton");
        }
        //No controller connected so use keyboard
        else
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal");
            verticalMovement = Input.GetAxisRaw("Vertical");
            aButtonPressed = Input.GetButtonDown("Jump");
            bButtonPressed = Input.GetKeyDown(KeyCode.F);
            xButtonPressed = Input.GetKeyDown(KeyCode.R);
            quitButtonPressed = Input.GetKeyDown(KeyCode.Escape);
            rbButtonPressed = Input.GetKeyDown(KeyCode.LeftShift);
            rbButtonHeld = Input.GetKey(KeyCode.LeftShift);
        }

        if (quitButtonPressed)
        {
            ReturnToMenu();
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("TItle Screen");
    }

}
