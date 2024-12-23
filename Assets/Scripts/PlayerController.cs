using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f; // Velocidad del personaje
    public float rotationSpeed = 720f; // Velocidad de rotación
    public int score = 0; // Puntuación inicial
    public int maxScore = 3; // Puntuación máxima para ganar
    private CanvasManager canvasManager;
    private Color playerColor = Color.white; // Color inicial del jugador
    public Canvas winCanvas;

    public Canvas winCanvasVR;
    public Transform weapon; // Referencia al arma (el cilindro)
    public float attackSpeed = 2f; // Velocidad de la animación de ataque
    private Quaternion initialWeaponRotation; // Rotación inicial del arma
    private bool isAttacking = false; // Bandera para evitar múltiples ataques simultáneos
    private Renderer playerRenderer;

    public GameObject CrazyCat_Red; // Objeto CrazyCat_Red
    public GameObject CrazyCat_Black; // Objeto CrazyCat_Black

    void Start()
    {
        playerRenderer = GetComponent<Renderer>();

        // Aplicar el color inicial al jugador
        playerRenderer.material.color = playerColor;

        canvasManager = FindObjectOfType<CanvasManager>();

        if (canvasManager == null)
        {
            Debug.LogError("CanvasManager no encontrado en la escena.");
        }
        // Guardar la rotación inicial del arma
        if (weapon != null)
        {
            initialWeaponRotation = weapon.localRotation;
        }
        // Guardar la rotación inicial del arma
        if (weapon != null)
        {
            initialWeaponRotation = weapon.localRotation;
        }

        if (isServer)
        {
            AssignCrazyCat(); // Configura el CrazyCat en el servidor
        }
    }

    void Update()
    {
        // Controlar únicamente al jugador local
        if (!isLocalPlayer) return;

        // Mover al jugador
        HandleMovement();

        // Animar el arma si está atacando
        if (isAttacking && weapon != null)
        {
            AnimateWeaponAttack();
        }
    }

    private void HandleMovement()
    {
        // Obtener entrada del teclado
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calcular dirección de movimiento
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical).normalized;

        if (movement.magnitude > 0.1f)
        {
            // Rotar hacia la dirección de movimiento
            Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Aplicar movimiento local
            transform.position += movement * moveSpeed * Time.deltaTime;
        }
    }

    private void AnimateWeaponAttack()
    {
        // Inclinación objetivo del arma
        Quaternion attackRotation = Quaternion.Euler(90, 0, 0);

        // Interpolar hacia la rotación de ataque
        weapon.localRotation = Quaternion.Lerp(weapon.localRotation, attackRotation, attackSpeed * Time.deltaTime);

        // Restaurar la rotación inicial después de completar el ataque
        if (Quaternion.Angle(weapon.localRotation, attackRotation) < 1f)
        {
            isAttacking = false;
            weapon.localRotation = initialWeaponRotation;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer) return;

        if (other.gameObject.CompareTag("Objetivo"))
        {
            score++;
            // Activar la animación de ataque
            if (!isAttacking)
            {
                isAttacking = true;
            }

            if (score >= maxScore)
            {
                Debug.Log("¡Has ganado!");
                // Mostrar el Canvas al ganar
                if (canvasManager != null)
                {
                    if (UnityEngine.XR.XRSettings.isDeviceActive) // Detección VR
                    {
                        canvasManager.ShowVrCanvas();
                    }
                    else
                    {
                        canvasManager.ShowNonVrCanvas();
                    }
                }
                
            }
            
            // Notificar al servidor sobre la colisión
            CmdHandleObjectiveCollision(other.gameObject);
        }
    }

    [Command]
    private void CmdHandleObjectiveCollision(GameObject objetivo)
    {
        // Llamar al método en el servidor para teletransportar el objetivo
        MovingObjective movingObjective = objetivo.GetComponent<MovingObjective>();
        if (movingObjective != null)
        {
            movingObjective.TeleportAndSetNewTarget();
        }
    }

    [Server]
    private void AssignCrazyCat()
    {
        int playerIndex = NetworkServer.connections.Count;

        if (playerIndex == 1)
        {
            RpcConfigureCrazyCat(true, false); // Primer jugador: CrazyCat_Red
        }
        else if (playerIndex == 2)
        {
            RpcConfigureCrazyCat(false, true); // Segundo jugador: CrazyCat_Black
        }else {
            RpcConfigureCrazyCat(false, true);
        }
    }

    [ClientRpc]
    private void RpcConfigureCrazyCat(bool enableRed, bool enableBlack)
    {
        if (CrazyCat_Red != null)
        {
            CrazyCat_Red.SetActive(enableRed);
        }

        if (CrazyCat_Black != null)
        {
            CrazyCat_Black.SetActive(enableBlack);
        }
    }
}