using UnityEngine;
using UnityTools.DataManagement;
using System;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{

    [SerializeField]
    private int playerID;
    [SerializeField]
    private KeyBindings keyBindings;
    [SerializeField]
    private ShellStats baseShellStats;

    private ShellStats currentShellStats;

    private Rigidbody myRigidbody;
    private Animator animator;
    private PressToGetShellUI pressToGetShellUI;

    private KeyCode attackKeyCode;
    private KeyCode defendKeyCode;

    private bool isInShellRange = false;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        pressToGetShellUI = Instantiate(ConstantsManager.PressToGetShellUIPrefab);
        animator = GetComponentInChildren<Animator>();
        currentShellStats = baseShellStats;
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

        bool keyCodeSetup = Enum.TryParse("Joystick" + playerID + "Button" + keyBindings.attackKeyCode, out attackKeyCode);
        keyCodeSetup &= Enum.TryParse("Joystick" + playerID + "Button" + keyBindings.defendKeyCode, out defendKeyCode);
        if (!keyCodeSetup)
        {
            Debug.LogError("Problem in setupping key codes for player" + playerID);
        }
    }

    private void ReadInputs()
    {
        // Movement
        float xAxis = Input.GetAxis("HorizontalJ" + playerID);
        float yAxis = Input.GetAxis("VerticalJ" + playerID);

        animator.SetBool("Walking", xAxis != 0 || yAxis != 0);

        myRigidbody.velocity = new Vector3(xAxis, 0, yAxis) * Time.deltaTime * currentShellStats.MovementSpeed;
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
        //if (Input.GetKeyDown(attackKeyCode))
        //{
        //    Debug.Log("Player" + playerID + " pressed A");
        //}
        //if (Input.GetKeyDown(defendKeyCode))
        //{
        //    Debug.Log("Player" + playerID + " pressed B");
        //}
        // TEST

        // Attack
        if(currentShellStats.CanAttack && Input.GetKeyDown(attackKeyCode))
        {
            animator.SetTrigger("Attack");
        }

        // Block
        //if (currentShellStats.CanBlock && Input.GetKeyDown(defendKeyCode))
        //{
        //    animator.SetBool("Block", true);
        //}
        //else if (currentShellStats.CanBlock && Input.GetKeyUp(defendKeyCode))
        //{
        //    animator.SetBool("Block", false);
        //}

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