using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f; // Velocidad del personaje
    public float rotationSpeed = 720f; // Velocidad de rotación
    public int score = 0; // Puntuación inicial
    public int maxScore = 3; // Puntuación máxima para ganar
    private CanvasManager canvasManager;

    [SyncVar(hook = nameof(OnColorChanged))] // Sincroniza el color entre el servidor y los clientes
    private Color playerColor = Color.white; // Color inicial del jugador
    
    public Canvas winCanvas;

    public Canvas winCanvasVR;

    private Renderer playerRenderer;

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
    }

    void Update()
    {
        // Controlar únicamente al jugador local
        if (!isLocalPlayer) return;

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

    // Incrementar la puntuación al colisionar con el objetivo
    void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer) return;

        if (other.gameObject.CompareTag("Objetivo"))
        {
            score++;
            Debug.Log("Score: " + score);

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
                // Notificar al servidor para cambiar el color
                CmdChangeColor(Color.red);
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

    [Command]
    private void CmdChangeColor(Color newColor)
    {
        // Cambiar el color en el servidor
        playerColor = newColor;
    }

    private void OnColorChanged(Color oldColor, Color newColor)
    {
        // Actualizar el color localmente cuando cambie
        Debug.Log($"Color cambiado de {oldColor} a {newColor}");
        playerRenderer.material.color = newColor;
    }
}
