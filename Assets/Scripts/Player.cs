using UnityEngine;
using UnityTools.DataManagement;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{

    [SerializeField]
    private int playerID;

    private ShellStats currentShellStats;

    private Rigidbody myRigidbody;
    private PressToGetShellUI pressToGetShellUI;

    private bool isInShellRange = false;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        pressToGetShellUI = Instantiate(ConstantsManager.PressToGetShellUIPrefab);
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
        // Movement
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

        // Grab shell

        // TEST
        if(Input.GetButtonDown("Joystick" + playerID + "A"))
        {
            Debug.Log("Player" + playerID + " pressed A");
        }
        // TEST

        // Attack

        // Block
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Shell")
        {
            Shell shell = GetComponentInParent<Shell>();
            isInShellRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Shell")
        {
            isInShellRange = false;   
        }
    }
}