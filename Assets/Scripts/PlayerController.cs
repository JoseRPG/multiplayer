using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f; // Velocidad del personaje
    public float rotationSpeed = 720f; // Velocidad de rotación
    public int score = 0; // Puntuación inicial
    public int maxScore = 3; // Puntuación máxima para ganar

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
        if (other.gameObject.name == "Objetivo")
        {
            score++;
            Debug.Log("Score: " + score);
            if (score >= maxScore)
            {
                Debug.Log("¡Has ganado!");
                //cambiar mi color a rojo
                GetComponent<Renderer>().material.color = Color.red;
            }
            // Volver a generar esfera
            other.gameObject.SetActive(false);
            other.GetComponent<MovingObjective>().TeleportRandom();
            other.GetComponent<MovingObjective>().SetNewRandomTarget();
            other.gameObject.SetActive(true);
        }
    }
}
