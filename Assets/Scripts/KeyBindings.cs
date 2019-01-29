using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "KeyBindings")]
public class KeyBindings : ScriptableObject
{

    public int joystickNumber;
    public KeyCode attackKeyCode;
    public KeyCode defendKeyCode;

    public void Clear()
    {
        joystickNumber = -1;
        attackKeyCode = KeyCode.None;
        defendKeyCode = KeyCode.None;
    }
}