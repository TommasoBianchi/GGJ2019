using UnityEngine;
using UnityTools.DataManagement;
using System;
using System.Collections.Generic;
using System.Linq;

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
    private Material baseMaterial;

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

    public bool IsAlive { get; private set; }

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        pressToGetShellUI = Instantiate(ConstantsManager.PressToGetShellUIPrefab);
        currentShellStats = baseShellStats;
        currentHealth = ConstantsManager.BaseCrabLife;
        IsAlive = true;
    }

    private void Update()
    {
        if(UnityEngine.Random.value < Time.deltaTime * 0.01f)
        {
            SFXManager.PlaySFX(SFXManager.SFXType.PaguroVoice);
        }

        if (!areControlsEnabled)
        {
            return;
        }

        // Read controller inputs
        ReadInputs();
    }

    public void SetID(int ID)
    {
        if (playerID != 0)
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

        ApplyBaseMaterial();
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

    public void SetBaseMaterial(Material baseMaterial)
    {
        if (this.baseMaterial != null)
        {
            Debug.LogError("Trying to reassign baseMaterial to player " + playerID);
        }
        else
        {
            this.baseMaterial = baseMaterial;
        }
    }

    public void PickupShell(Shell shell)
    {
        SetupShell(shell.ShellStats);
        Destroy(shell.gameObject);
        SFXManager.PlaySFX(SFXManager.SFXType.EquipShell);
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

        ApplyBaseMaterial();
    }

    private void ApplyBaseMaterial()
    {
        foreach (var renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (renderer.gameObject.tag != "SpecialMaterial")
            {
                renderer.material = baseMaterial;
            }
        }
    }

    private void ReadInputs()
    {
        bool isDefending = Input.GetKey(defendKeyCode);
        bool isInAttackState = animator.GetCurrentAnimatorStateInfo(1).IsName("Attack");
        if (!isInAttackState)
        {
            weaponCanHit = false;
            TrailRenderer trailRenderer = GetComponentInChildren<TrailRenderer>();
            if (trailRenderer != null)
            {
                trailRenderer.enabled = false;
            }
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
            SFXManager.PlaySFX(SFXManager.SFXType.ButtonsSwitch);
        }
        if (Input.GetKeyUp(attackKeyCode))
        {
            pressToGetShellUI.StopPressing();
            SFXManager.PlaySFX(SFXManager.SFXType.ButtonsSwitch);
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
                SFXManager.PlaySFX(SFXManager.SFXType.CannonShoot);
            }

            TrailRenderer trailRenderer = GetComponentInChildren<TrailRenderer>();
            if(trailRenderer != null)
            {
                trailRenderer.enabled = true;
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

    private void TakeDamage(float amount, bool fromRanged)
    {
        if (currentShellStats.CanBlock && animator.GetBool("Block"))
        {
            amount *= ConstantsManager.ShieldAbsorbRate;

            if (fromRanged)
            {
                SFXManager.PlaySFX(SFXManager.SFXType.CannonImpactBlock);
            }
            else
            {
                SFXManager.PlaySFX(SFXManager.SFXType.MeleeWeaponBlock);
            }
        }
        else
        {
            if (fromRanged)
            {
                SFXManager.PlaySFX(SFXManager.SFXType.CannonImpact);
            }
            else
            {
                SFXManager.PlaySFX(SFXManager.SFXType.MeleeWepon);
            }
        }

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
                SFXManager.PlaySFX(SFXManager.SFXType.ShellBreak);
                Destroy(Instantiate(ConstantsManager.PlayerDieVFXPrefab, transform.position, Quaternion.identity), 10);
            }
            else if (shellValue.Value <= 0.5f)
            {
                // Setup broken material
                PlayerShell playerShell = GetComponentInChildren<PlayerShell>();
                if (playerShell != null)
                {
                    playerShell.ActivateBrokenMaterial();
                    SFXManager.PlaySFX(SFXManager.SFXType.ShellBreak);
                }
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
                SFXManager.PlaySFX(SFXManager.SFXType.Death);
                IsAlive = false;
                areControlsEnabled = false;
                animator.SetBool("Walking", false);
                myRigidbody.velocity = Vector3.zero;
                Destroy(gameObject, 3);
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
                TakeDamage(projectile.HitDamage, true);
                Destroy(projectile.gameObject);
            }
            else if(otherPlayer.weaponCanHit)
            {
                TakeDamage(otherPlayer.currentShellStats.HitDamage, false);
                otherPlayer.weaponCanHit = false;
            }
        }
    }

    private void OnDestroy()
    {
        if (GameManager.IsPlayingRound)
        {
            Destroy(Instantiate(ConstantsManager.PlayerDieVFXPrefab, transform.position, Quaternion.identity), 10);
        }

        if (pressToGetShellUI != null)
        {
            Destroy(pressToGetShellUI.gameObject);
        }
        GameManager.PlayerDied();
    }
}