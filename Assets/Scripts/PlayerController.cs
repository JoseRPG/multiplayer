using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f; // Velocidad del personaje
    public float rotationSpeed = 720f; // Velocidad de rotación
    public int score = 0; // Puntuación inicial
    public int maxScore = 3; // Puntuación máxima para ganar
    private Color playerColor = Color.white; // Color inicial del jugador
    public Canvas winCanvas;
    public Transform weapon; // Referencia al arma (el cilindro)
    public float attackSpeed = 2f; // Velocidad de la animación de ataque
    private Quaternion initialWeaponRotation; // Rotación inicial del arma
    private bool isAttacking = false; // Bandera para evitar múltiples ataques simultáneos
    private Renderer playerRenderer;

    void Start()
    {
        playerRenderer = GetComponent<Renderer>();

        // Aplicar el color inicial al jugador
        playerRenderer.material.color = playerColor;

        // Asegurarse de que el Canvas esté inicialmente desactivado
        if (winCanvas != null)
        {
            winCanvas.gameObject.SetActive(false);
        }
        // Guardar la rotación inicial del arma
        if (weapon != null)
        {
            initialWeaponRotation = weapon.localRotation;
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
                if (winCanvas != null)
                {
                    winCanvas.gameObject.SetActive(true);
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
}
