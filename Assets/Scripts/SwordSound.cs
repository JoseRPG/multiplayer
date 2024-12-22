using UnityEngine;

public class SwordSound : MonoBehaviour
{
    public AudioClip swingSound; // Sonido de la espada
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = swingSound;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Botón izquierdo del ratón
        {
            PlaySwingSound();
        }
    }

    void PlaySwingSound()
    {
        audioSource.Play();
    }
}
