using UnityEngine;
using Mirror;

public class MovingObjective : NetworkBehaviour
{
    private Vector3 areaMin = new Vector3(-5, 0.5f, -5);
    private Vector3 areaMax = new Vector3(5, 0.5f, 5);
    public float speed = 3f; // Velocidad de movimiento

    [SyncVar] // Sincroniza esta variable entre el servidor y los clientes
    private Vector3 targetPosition; // Posición objetivo actual

    void Start()
    {
        if (isServer) // Solo el servidor establece el primer objetivo
        {
            SetNewRandomTarget();
        }
    }

    void Update()
    {
        if (isServer) // Solo el servidor controla el movimiento
        {
            MoveTowardsTarget();
        }
    }

    [Server]
    private void MoveTowardsTarget()
    {
        // Mover la esfera hacia el objetivo
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
    }

    [Server]
    public void TeleportAndSetNewTarget()
    {
        // Teletransportar el objetivo a una posición aleatoria
        float x = Random.Range(areaMin.x, areaMax.x);
        float z = Random.Range(areaMin.z, areaMax.z);
        transform.position = new Vector3(x, transform.position.y, z);

        // Establecer un nuevo objetivo
        SetNewRandomTarget();

        // Notificar a los clientes para que sincronicen la posición
        RpcUpdateObjectivePosition(transform.position, targetPosition);
    }

    [ClientRpc]
    private void RpcUpdateObjectivePosition(Vector3 newPosition, Vector3 newTarget)
    {
        // Actualizar posición y objetivo localmente en los clientes
        transform.position = newPosition;
        targetPosition = newTarget;
    }
}
