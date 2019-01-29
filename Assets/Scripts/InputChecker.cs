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

        keyBindings.Clear();
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
            int i = CheckJoystickNumber();
            if(i != -1)
            {
                Debug.Log("Assigning joystick " + i + " to player" + playerID + " in frame " + Time.frameCount);
                joystickNumber = i;
                keyBindings.joystickNumber = i;
                pressAnyText.gameObject.SetActive(false);
                pressAText.gameObject.SetActive(true);
            }
        }
        else if (!aPressed)
        {
            KeyCode k = CheckButtonNumber(joystickNumber);
            if (k != KeyCode.None)
            {
                Debug.Log("Assigning button " + k.ToString() + " to attack for player" + playerID + " in frame " + Time.frameCount);
                keyBindings.attackKeyCode = k;
                aPressed = true;
                pressAText.gameObject.SetActive(false);
                pressBText.gameObject.SetActive(true);
            }
        }
        else
        {
            KeyCode k = CheckButtonNumber(joystickNumber);
            if (k != KeyCode.None && k != keyBindings.attackKeyCode)
            {
                keyBindings.defendKeyCode = k;
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
            if (boundJoystick != null)
            {
                boundJoystick.Add(joystickNumber);
            }
        }
    }

    public static int ReadyPlayersCount()
    {
        return allInputCheckers.Count(ic => ic.anyPressed && ic.aPressed && ic.bPressed);
    }

    public static void ClearData()
    {
        allInputCheckers = null;
        boundJoystick = null;
    }

    private int CheckJoystickNumber()
    {
        // Check keyboard
        KeyCode kk = CheckKeyboardKey();
        if (kk != KeyCode.None && !boundJoystick.Contains(0))
        {
            return 0;
        }

        // Check all joysticks
        for (int i = 1; i <= 8; i++)
        {
            KeyCode k = CheckButtonNumber(i);
            if (k != KeyCode.None && !boundJoystick.Contains(i))
            {
                return i;
            }
        }

        return -1;
    }

    private KeyCode CheckButtonNumber(int joystickNumber)
    {
        if(joystickNumber == 0)
        {
            return CheckKeyboardKey();
        }

        for (int i = 0; i < 20; i++)
        {
            KeyCode buttonKeyCode;
            Enum.TryParse("Joystick" + joystickNumber + "Button" + i, out buttonKeyCode);
            if (Input.GetKeyDown(buttonKeyCode))
            {
                return buttonKeyCode;
            }
        }

        return KeyCode.None;
    }

    private KeyCode CheckKeyboardKey()
    {
        foreach (var k in keyboardKeycodes)
        {
            if (Input.GetKeyDown(k))
            {
                return k;
            }
        }

        return KeyCode.None;
    }

    private static readonly KeyCode[] joystickKeycodes = GetAllJoystickKeycodes().ToArray();
    private static readonly KeyCode[] keyboardKeycodes = Enum.GetValues(typeof(KeyCode)).OfType<KeyCode>().Except(joystickKeycodes).
                                                            Except(new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D,
                                                                   KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow }).ToArray();

    private static List<KeyCode> GetAllJoystickKeycodes()
    {
        List<KeyCode> codes = new List<KeyCode>();
        KeyCode buttonKeyCode;

        for (int b = 0; b < 20; b++)
        {
            for (int j = 1; j <= 8; j++)
            {
                Enum.TryParse("Joystick" + j + "Button" + b, out buttonKeyCode);
                codes.Add(buttonKeyCode);
            }
            Enum.TryParse("Joystick" + "Button" + b, out buttonKeyCode);
            codes.Add(buttonKeyCode);
        }

        return codes;
    }
}
