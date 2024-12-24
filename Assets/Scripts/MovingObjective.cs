using UnityEngine;
using Mirror;

public class MovingObjective : NetworkBehaviour
{
    private Vector3 areaMin = new Vector3(-5, 0.5f, -5);
    private Vector3 areaMax = new Vector3(5, 0.5f, 5);
    public float speed = 3f; // Velocidad de movimiento

    [SyncVar] // Sincroniza esta variable entre el servidor y los clientes
    private Vector3 targetPosition; // Posición objetivo actual

    private Vector3 initialPosition; // Posición inicial para reinicios

    private bool isPaused = false; // Indica si el objetivo está en pausa

    void Start()
    {
        initialPosition = transform.position; // Guardar la posición inicial
        if (isServer) // Solo el servidor establece el primer objetivo
        {
            SetNewRandomTarget();
        }
    }

    void Update()
    {
        if (isServer && !isPaused) // Solo el servidor controla el movimiento si no está en pausa
        {
            MoveTowardsTarget();
        }
    }

    [Server]
    private void MoveTowardsTarget()
    {
        // Orientar el objetivo hacia el target
        Vector3 direction = targetPosition - transform.position;
        if (direction != Vector3.zero) // Evitar problemas con vectores nulos
        {
            transform.rotation = Quaternion.LookRotation(direction); // Girar hacia el objetivo
        }
        
        // Mover el objetivo hacia el target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Si ha llegado al objetivo, genera uno nuevo
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNewRandomTarget();
        }
    }

    [Server]
    private void SetNewRandomTarget()
    {
        // Generar una nueva posición aleatoria dentro del área definida
        float x = Random.Range(areaMin.x, areaMax.x);
        float z = Random.Range(areaMin.z, areaMax.z);
        targetPosition = new Vector3(x, transform.position.y, z); // Mantener la altura constante

        // Sincronizar la posición del target a los clientes
        RpcUpdateObjectiveTarget(targetPosition);
    }

    [Server]
    public void TeleportAndSetNewTarget()
    {
        if (isPaused) return; // No hacer nada si está en pausa

        // Teletransportar el objetivo a una posición aleatoria
        float x = Random.Range(areaMin.x, areaMax.x);
        float z = Random.Range(areaMin.z, areaMax.z);
        transform.position = new Vector3(x, transform.position.y, z);

        // Establecer un nuevo objetivo
        SetNewRandomTarget();

        // Notificar a los clientes para que sincronicen la posición
        RpcUpdateObjectivePosition(transform.position, targetPosition);
    }

    [Server]
    public void Pause()
    {
        isPaused = true; // Pausar el movimiento
    }

    [Server]
    public void Reactivate()
    {
        isPaused = false; // Reactivar el movimiento

        // Mover el objetivo a la posición inicial
        transform.position = initialPosition;

        // Establecer un nuevo objetivo
        SetNewRandomTarget();

        // Notificar a los clientes para que sincronicen el estado
        RpcUpdateObjectivePosition(transform.position, targetPosition);
    }

    [ClientRpc]
    private void RpcUpdateObjectivePosition(Vector3 newPosition, Vector3 newTarget)
    {
        // Actualizar posición y objetivo localmente en los clientes
        transform.position = newPosition;
        targetPosition = newTarget;
    }

    [ClientRpc]
    private void RpcUpdateObjectiveTarget(Vector3 newTarget)
    {
        // Actualizar el objetivo localmente en los clientes
        targetPosition = newTarget;
    }
}
