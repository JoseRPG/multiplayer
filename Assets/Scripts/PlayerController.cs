using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidad de movimiento del personaje
    public float rotationSpeed = 720f; // Velocidad de rotación del personaje (en grados por segundo)

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Obtener entrada de teclado
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calcular dirección de movimiento
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical).normalized;

        if (movement.magnitude > 0.1f)
        {
            // Rotar hacia la dirección de movimiento
            Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Aplicar movimiento
            rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);
        }
    }
}
