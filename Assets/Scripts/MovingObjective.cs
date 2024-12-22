using UnityEngine;

public class MovingObjective : MonoBehaviour
{
    private Vector3 areaMin = new Vector3(-5, 0.5f, -5);
    private Vector3 areaMax = new Vector3(5, 0.5f, 5);
    public float speed = 3f; // Velocidad de movimiento
    private Vector3 targetPosition; // Posición objetivo actual

    void Start()
    {
        SetNewRandomTarget();
    }

    void Update()
    {
        // Mover la esfera hacia el objetivo
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Si ha llegado al objetivo, genera uno nuevo
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNewRandomTarget();
        }
    }

    public void SetNewRandomTarget()
    {
        // Generar una nueva posición aleatoria dentro del área definida
        float x = Random.Range(areaMin.x, areaMax.x);
        float z = Random.Range(areaMin.z, areaMax.z);
        targetPosition = new Vector3(x, transform.position.y, z); // Mantener la altura constante
    }

    public void TeleportRandom()
    {
        float x = Random.Range(areaMin.x, areaMax.x);
        float z = Random.Range(areaMin.z, areaMax.z);
        transform.position = new Vector3(x, transform.position.y, z);
    }
}
