using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class InputChecker : MonoBehaviour
{

    [SerializeField]
    private RectTransform pressAnyText;
    [SerializeField]
    private RectTransform pressAText;
    [SerializeField]
    private RectTransform pressBText;
    [SerializeField]
    private RectTransform readyText;
    [SerializeField]
    private int playerID;

    [SerializeField]
    private KeyBindings keyBindings;

    private int joystickNumber = -1;

    private bool anyPressed = false;
    private bool aPressed = false;
    private bool bPressed = false;

    private static List<InputChecker> allInputCheckers;
    private static List<int> boundJoystick;

    private void Awake()
    {
        if(allInputCheckers == null)
        {
            allInputCheckers = new List<InputChecker>() { null, null, null, null };
            boundJoystick = new List<int>();
        }

        allInputCheckers[playerID - 1] = this;
    }

    private void Update()
    {
        if(anyPressed && aPressed && bPressed)
        {
            return;
        }

        for (int i = 0; i < playerID - 1; i++)
        {
            if (!allInputCheckers[i].anyPressed)
            {
                return;
            }
        }

        if (!anyPressed)
        {
            for (int i = 1; i <= 16; i++)
            {
                int b = CheckButtonNumber(i);
                if (b != -1 && !boundJoystick.Contains(i))
                {
                    joystickNumber = i;
                    pressAnyText.gameObject.SetActive(false);
                    pressAText.gameObject.SetActive(true);
                    Debug.Log("Player" + playerID + " selected to use joystick " + joystickNumber);
                    break;
                }
            }
        }
        else if (!aPressed)
        {
            int b = CheckButtonNumber(joystickNumber);
            if (b != -1)
            {
                keyBindings.attackKeyCode = b;
                Debug.Log("Button " + b + " pressed as A on joystick " + joystickNumber);
                aPressed = true;
                pressAText.gameObject.SetActive(false);
                pressBText.gameObject.SetActive(true);
            }
        }
        else
        {
            int b = CheckButtonNumber(joystickNumber);
            if (b != -1)
            {
                keyBindings.defendKeyCode = b;
                Debug.Log("Button " + b + " pressed as B on joystick " + joystickNumber);
                bPressed = true;
                pressBText.gameObject.SetActive(false);
                readyText.gameObject.SetActive(true);

                if(allInputCheckers.Count(ic => ic.anyPressed && ic.aPressed && ic.bPressed) >= 2)
                {
                    // Two players are ready, so start the countdown
                    FindObjectOfType<StartLevelCountDown>().StartCountDown();
                }
            }
        }
    }

    private void LateUpdate()
    {
        if(joystickNumber != -1)
        {
            anyPressed = true;
            boundJoystick.Add(joystickNumber);
        }
    }

    public static int ReadyPlayersCount()
    {
        return allInputCheckers.Count(ic => ic.anyPressed && ic.aPressed && ic.bPressed);
    }

    private int CheckButtonNumber(int joystickNumber)
    {
        for (int i = 0; i < 16; i++)
        {
            KeyCode buttonKeyCode;
            Enum.TryParse("Joystick" + joystickNumber + "Button" + i, out buttonKeyCode);
            if (Input.GetKeyDown(buttonKeyCode))
            {
                return i;
            }
        }

        return -1;
    }
}
