using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "KeyBindings")]
public class KeyBindings : ScriptableObject
{
    
    public int attackKeyCode;
    public int defendKeyCode;
    public int joystickNumber;
}