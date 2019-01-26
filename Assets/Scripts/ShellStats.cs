using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Shell")]
public class ShellStats : ScriptableObject
{

    [SerializeField]
    private float movementSpeed;

    #region Properties

    public float MovementSpeed { get { return movementSpeed; } }

    #endregion
}