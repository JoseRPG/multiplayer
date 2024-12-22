using UnityEngine;

public class MovementSound : MonoBehaviour
{
    public AudioClip sound; // Sonido asociado al movimiento
    private AudioSource audioSource;
    private Vector3 lastPosition; // Para almacenar la posición anterior del objeto

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = sound;
        lastPosition = transform.position; // Guardamos la posición inicial
    }

    void Update()
    {
        // Calculamos si el objeto está en movimiento
        bool isMoving = (transform.position - lastPosition).magnitude > 0.01f;

        if (isMoving)
        {
            // Si está en movimiento y el sonido no está reproduciéndose, lo reproducimos
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }


        // Actualizamos la última posición
        lastPosition = transform.position;
    }
}
