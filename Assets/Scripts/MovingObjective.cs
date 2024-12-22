using UnityEngine;

public class MovingObjective : MonoBehaviour
{
    public Transform pointA; // Primer punto
    public Transform pointB; // Segundo punto
    public float speed = 3f; // Velocidad de movimiento
    private bool movingToB = true;

    void Update()
    {
        // Movimiento entre dos puntos
        if (movingToB)
        {
            transform.position = Vector3.MoveTowards(transform.position, pointB.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, pointB.position) < 0.1f)
                movingToB = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, pointA.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, pointA.position) < 0.1f)
                movingToB = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}
