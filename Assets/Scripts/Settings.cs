using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Settings")]
public class Settings : ScriptableObject
{
    
    public int NumberOfPlayers;
    public float musicVolume;
    public float sfxVolume;
}