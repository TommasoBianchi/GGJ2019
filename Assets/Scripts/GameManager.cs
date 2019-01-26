using UnityEngine;

public class GameManager : MonoBehaviour
{

    private GameManager _instance;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogError("There should be only one GameManager in a scene.");
            DestroyImmediate(gameObject);
        }

        Debug.Log("Connected controllers:");
        foreach(var name in Input.GetJoystickNames())
        {
            Debug.Log("Controller: " + name);
        }
    }
}
