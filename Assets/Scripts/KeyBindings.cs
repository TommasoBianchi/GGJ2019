using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "KeyBindings")]
public class KeyBindings : ScriptableObject
{
    
    public int attackKeyCode;
    public int defendKeyCode;
    public int joystickNumber;

    public void Clear()
    {
        attackKeyCode = -1;
        defendKeyCode = -1;
        joystickNumber = -1;
    }
}