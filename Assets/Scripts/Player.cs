﻿using UnityEngine;
using UnityTools.DataManagement;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{

    [SerializeField]
    private int playerID;
    [SerializeField]
    private KeyBindings keyBindings;
    [SerializeField]
    private ShellStats baseShellStats;
    [SerializeField]
    private Transform modelContainer;

    private ShellStats currentShellStats;

    private Rigidbody myRigidbody;
    private Animator animator;
    private PressToGetShellUI pressToGetShellUI;

    private KeyCode attackKeyCode;
    private KeyCode defendKeyCode;

    private bool isInShellRange = false;

    private bool areControlsEnabled = false;

    private float nextAttackTime = 0;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        pressToGetShellUI = Instantiate(ConstantsManager.PressToGetShellUIPrefab);
        currentShellStats = baseShellStats;
        SetupShell(currentShellStats);
    }

    private void Update()
    {
        if (!areControlsEnabled)
        {
            return;
        }

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

    public void SetKeyBindings(KeyBindings keyBindings)
    {
        if (this.keyBindings != null)
        {
            Debug.Log(keyBindings.name);
            Debug.LogError("Trying to reassign keyBindings to player " + playerID);
        }
        else
        {
            this.keyBindings = keyBindings;
        }
    }

    public void PickupShell(Shell shell)
    {
        currentShellStats = shell.ShellStats;
        Destroy(shell.gameObject);
        SetupShell(currentShellStats);
    }

    public void EnableControls()
    {
        areControlsEnabled = true;
    }

    private void SetupShell(ShellStats stats)
    {
        List<Transform> modelContainerChildren = new List<Transform>();
        for (int i = 0; i < modelContainer.childCount; i++)
        {
            modelContainerChildren.Add(modelContainer.GetChild(i));
        }
        for (int i = 0; i < modelContainerChildren.Count; i++)
        {
            DestroyImmediate(modelContainerChildren[i].gameObject);
        }

        Instantiate(stats.ModelPrefab, modelContainer);
        animator = GetComponentInChildren<Animator>();
    }

    private void ReadInputs()
    {
        bool isDefending = Input.GetKey(defendKeyCode);
        bool isInAttackState = animator.GetCurrentAnimatorStateInfo(1).IsName("Attack");
        bool canAttack = currentShellStats.CanAttack && Input.GetKeyDown(attackKeyCode) && !isDefending && !isInAttackState;

        // Movement
        float xAxis = Input.GetAxis("HorizontalJ" + playerID);
        float yAxis = Input.GetAxis("VerticalJ" + playerID);

        animator.SetBool("Walking", xAxis != 0 || yAxis != 0);

        myRigidbody.velocity = new Vector3(xAxis, 0, yAxis).normalized * currentShellStats.MovementSpeed;
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
        if(currentShellStats.CanPickupShells && isInShellRange && Input.GetKeyDown(attackKeyCode) && !isDefending)
        {
            pressToGetShellUI.StartPressing();
        }
        if (Input.GetKeyUp(attackKeyCode))
        {
            pressToGetShellUI.StopPressing();
        }

        // Attack
        if(canAttack && Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack");
            if (currentShellStats.IsRanged)
            {
                Projectile projectile =  Instantiate(currentShellStats.ProjectilePrefab, 
                                                     GetComponentInChildren<FiringSpot>().transform.position, 
                                                     Quaternion.LookRotation(transform.forward, transform.up));
                projectile.SetSpeed(currentShellStats.ProjectileSpeed);
                nextAttackTime = Time.time + currentShellStats.AttackCooldown;
            }
        }

        // Block
        if (currentShellStats.CanBlock && Input.GetKeyDown(defendKeyCode))
        {
            animator.SetBool("Block", true);
        }
        else if (currentShellStats.CanBlock && Input.GetKeyUp(defendKeyCode))
        {
            animator.SetBool("Block", false);
        }

        if (isDefending)
        {
            myRigidbody.velocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(currentShellStats.CanPickupShells && collision.gameObject.tag == "Shell")
        {
            Shell shell = collision.gameObject.GetComponentInParent<Shell>();
            isInShellRange = true;
            pressToGetShellUI.Show(this, shell);
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (!isInShellRange && currentShellStats.CanPickupShells && collision.gameObject.tag == "Shell")
        {
            Shell shell = collision.gameObject.GetComponentInParent<Shell>();
            isInShellRange = true;
            pressToGetShellUI.Show(this, shell);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Shell")
        {
            isInShellRange = false;
            pressToGetShellUI.Hide();
        }
    }
}