using UnityEngine;
using UnityTools.DataManagement;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{

    [SerializeField]
    private int playerID;

    private ShellStats currentShellStats;

    private Rigidbody myRigidbody;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Read controller inputs
        ReadInputs();
    }

    public void SetID(int ID)
    {
        if(playerID != 0)
        {
            Debug.LogError("Trying to reassign ID to player " + playerID);
        }
        else
        {
            playerID = ID;
        }
    }

    private void ReadInputs()
    {
        float xAxis = Input.GetAxis("HorizontalJ" + playerID);
        float yAxis = Input.GetAxis("VerticalJ" + playerID);

        myRigidbody.velocity = new Vector3(xAxis, 0, yAxis) * Time.deltaTime * 200;// currentShellStats.MovementSpeed;
        if (xAxis != 0 || yAxis != 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(xAxis, 0, yAxis), Vector3.up);
            float rotationSpeed = ConstantsManager.RotationSpeed;
            if(rotationSpeed > 0)
            {
                myRigidbody.rotation = Quaternion.Slerp(myRigidbody.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
            else
            {
                myRigidbody.rotation = targetRotation;
            }            
        }
    }
}