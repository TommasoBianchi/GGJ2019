using UnityEngine;
using UnityTools.DataManagement;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{

    [SerializeField]
    private int playerID;
    [SerializeField]
    private ShellStats baseShellStats;
    [SerializeField]
    private Transform modelContainer;

    private ShellStats currentShellStats;
    private KeyBindings keyBindings;
    private FloatValue healthValue;
    private FloatValue shellValue;

    private float currentHealth;
    private float currentShellHealth;

    private Rigidbody myRigidbody;
    private Animator animator;
    private PressToGetShellUI pressToGetShellUI;

    private KeyCode attackKeyCode;
    private KeyCode defendKeyCode;

    private bool isInShellRange = false;
    private bool areControlsEnabled = false;
    private float nextAttackTime = 0;
    private bool weaponCanHit = false;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        pressToGetShellUI = Instantiate(ConstantsManager.PressToGetShellUIPrefab);
        currentShellStats = baseShellStats;
        currentHealth = ConstantsManager.BaseCrabLife;
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

        bool keyCodeSetup = Enum.TryParse("Joystick" + keyBindings.joystickNumber + "Button" + keyBindings.attackKeyCode, out attackKeyCode);
        keyCodeSetup &= Enum.TryParse("Joystick" + keyBindings.joystickNumber + "Button" + keyBindings.defendKeyCode, out defendKeyCode);
        if (!keyCodeSetup)
        {
            Debug.LogError("Problem in setupping key codes for player" + playerID);
        }

        SetupShell(currentShellStats);
    }

    public void SetKeyBindings(KeyBindings keyBindings)
    {
        if (this.keyBindings != null)
        {
            Debug.LogError("Trying to reassign keyBindings to player " + playerID);
        }
        else
        {
            this.keyBindings = keyBindings;
        }
    }

    public void SetHealthValue(FloatValue healthValue)
    {
        if (this.healthValue != null)
        {
            Debug.LogError("Trying to reassign healthValue to player " + playerID);
        }
        else
        {
            this.healthValue = healthValue;
            healthValue.SetValue(1);
        }
    }

    public void SetShellValue(FloatValue shellValue)
    {
        if (this.shellValue != null)
        {
            Debug.LogError("Trying to reassign shellValue to player " + playerID);
        }
        else
        {
            this.shellValue = shellValue;
            shellValue.SetValue(0);
        }
    }

    public void PickupShell(Shell shell)
    {
        SetupShell(shell.ShellStats);
        Destroy(shell.gameObject);
    }

    public void EnableControls()
    {
        areControlsEnabled = true;
    }

    public void SetWeaponCanHit(bool value)
    {
        this.weaponCanHit = value;
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
            Destroy(modelContainerChildren[i].gameObject);
        }

        Transform newShell = Instantiate(stats.ModelPrefab, modelContainer);
        animator = newShell.GetComponentInChildren<Animator>();
        currentShellStats = stats;
        currentShellHealth = stats.ShellHealth;
        shellValue.SetValue(currentShellHealth > 0 ? 1 : 0);
    }

    private void ReadInputs()
    {
        bool isDefending = Input.GetKey(defendKeyCode);
        bool isInAttackState = animator.GetCurrentAnimatorStateInfo(1).IsName("Attack");
        if (!isInAttackState)
        {
            weaponCanHit = false;
        }
        bool canAttack = currentShellStats.CanAttack && Input.GetKeyDown(attackKeyCode) && !isDefending && !isInAttackState;

        // Movement
        float xAxis = Input.GetAxis("HorizontalJ" + keyBindings.joystickNumber);
        float yAxis = Input.GetAxis("VerticalJ" + keyBindings.joystickNumber);

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
            weaponCanHit = true;
            if (currentShellStats.IsRanged)
            {
                Projectile projectile =  Instantiate(currentShellStats.ProjectilePrefab, 
                                                     GetComponentInChildren<FiringSpot>().transform.position, 
                                                     Quaternion.LookRotation(transform.forward, transform.up));
                projectile.SetSpeed(currentShellStats.ProjectileSpeed);
                projectile.HitDamage = currentShellStats.HitDamage;
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

    private void TakeDamage(float amount)
    {
        Debug.Log("Player " + playerID + " is taking " + amount + " damage");

        if(currentShellHealth > 0)
        {
            currentShellHealth = currentShellHealth - amount;
            if(currentShellHealth < 0)
            {
                currentShellHealth = 0;
            }
            shellValue.SetValue(currentShellHealth / currentShellStats.ShellHealth);

            if (currentShellHealth <= 0)
            {
                // Break the current shell
                SetupShell(baseShellStats);
            }
            else if (shellValue.Value <= 0.5f)
            {
                // TODO: Change material
            }
        }
        else
        {
            currentHealth = currentHealth - amount;
            if(currentShellHealth < 0)
            {
                currentShellHealth = 0;
            }
            healthValue.SetValue(currentHealth / ConstantsManager.BaseCrabLife);

            if(currentHealth <= 0)
            {
                // Die
                areControlsEnabled = false;
                myRigidbody.velocity = Vector3.zero;
                Destroy(gameObject, 5);
            }
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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.tag == "Weapon")
        {
            Player otherPlayer = collision.transform.root.GetComponentInChildren<Player>();
            if (otherPlayer == null)
            {
                Projectile projectile = collision.transform.root.GetComponentInChildren<Projectile>();
                TakeDamage(projectile.HitDamage);
                Destroy(projectile.gameObject);
            }
            else if(otherPlayer.weaponCanHit)
            {
                TakeDamage(otherPlayer.currentShellStats.HitDamage);
                otherPlayer.weaponCanHit = false;
            }
        }
    }
}