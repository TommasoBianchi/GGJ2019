using UnityEngine;
using UnityEngine.Experimental.Input;

[CreateAssetMenu(menuName = "KeyBindings")]
public class KeyBindings : ScriptableObject
{

    public InputDevice inputDevice;

    public int joystickNumber;
    public KeyCode attackKeyCode;
    public KeyCode defendKeyCode;

    public void Clear()
    {
        inputDevice = null;

        joystickNumber = -1;
        attackKeyCode = KeyCode.None;
        defendKeyCode = KeyCode.None;
    }
}