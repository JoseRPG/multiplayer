using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;

    private ScoreManager2 scoreManager;
    private Renderer playerRenderer;

    public Transform weapon;
    public float attackSpeed = 2f;

    private Quaternion initialWeaponRotation;
    private bool isAttacking = false;

    public GameObject CrazyCat_Red;
    public GameObject CrazyCat_Black;

    [SyncVar] private int playerIndex;
    private bool hasTriggered = false; 


    void Start()
    {
        playerRenderer = GetComponent<Renderer>();

        if (isServer)
        {
            playerIndex = NetworkServer.connections.Count;
            AssignCrazyCat();
        }

        scoreManager = FindObjectOfType<ScoreManager2>();

        if (weapon != null)
            initialWeaponRotation = weapon.localRotation;
    }

    void Update()
    {
        if (isAttacking && weapon != null)
        {
            AnimateWeaponAttack();
        }
        if (!isLocalPlayer) return;
        HandleMovement();


    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical).normalized;

        if (movement.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.position += movement * moveSpeed * Time.deltaTime;
        }
    }

    private void AnimateWeaponAttack()
    {
        Quaternion attackRotation = Quaternion.Euler(90, 0, 0);
        weapon.localRotation = Quaternion.Lerp(weapon.localRotation, attackRotation, attackSpeed * Time.deltaTime);

        if (Quaternion.Angle(weapon.localRotation, attackRotation) < 1f)
        {
            isAttacking = false;
            weapon.localRotation = initialWeaponRotation;
        }
    }

    [Server]
    private void AssignCrazyCat()
    {
        bool enableRed = playerIndex == 1;
        bool enableBlack = playerIndex == 2;

        RpcConfigureCrazyCat(enableRed, enableBlack);
    }

    [ClientRpc]
    private void RpcConfigureCrazyCat(bool enableRed, bool enableBlack)
    {
        CrazyCat_Red?.SetActive(enableRed);
        CrazyCat_Black?.SetActive(enableBlack);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Objetivo") && !hasTriggered)
        {
            hasTriggered = true; // Marca que este trigger ya fue procesado

            if (!isAttacking)
            {
                isAttacking = true;
            }

            // Solo el cliente local envía el comando
            if (isLocalPlayer)
            {
                CmdHandleObjectiveCollision(other.gameObject);
            }

            // Reinicia la bandera después de un breve tiempo para permitir futuros triggers
            Invoke(nameof(ResetTrigger), 0.2f); // Cooldown de 200 ms
        }
    }

    private void ResetTrigger()
    {
        hasTriggered = false;
    }

    [Command]
    private void CmdHandleObjectiveCollision(GameObject objetivo)
    {
        if (scoreManager == null) return;

        // Actualiza el puntaje en el servidor
        scoreManager.UpdateScore(playerIndex);

        // Teletransporta el objetivo
        MovingObjective movingObjective = objetivo.GetComponent<MovingObjective>();
        movingObjective?.TeleportAndSetNewTarget();
    }
}
