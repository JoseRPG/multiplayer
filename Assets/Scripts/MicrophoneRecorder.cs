using UnityEngine;

public class MicrophoneRecorder : MonoBehaviour
{
    private AudioSource audioSource;
    private bool isRecording = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleRecording();
        }
    }

    private void ToggleRecording()
    {
        if (isRecording)
        {
            StopRecording();
        }
        else
        {
            StartRecording();
        }
    }

    private void StartRecording()
    {
        if (Microphone.devices.Length > 0)
        {
            GetComponent<Renderer>().material.color = Color.red;
            audioSource.clip = Microphone.Start(null, true, 10, 44100); // Graba 10 segundos
            isRecording = true;
        }
        else
        {
            Debug.LogWarning("No se detectó ningún micrófono.");
        }
    }

    private void StopRecording()
    {
        if (isRecording)
        {
            GetComponent<Renderer>().material.color = Color.white;
            Microphone.End(null);
            isRecording = false;
            audioSource.Play(); // Reproduce la grabación
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isRecording)
        {
            StartRecording();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isRecording)
        {
            StopRecording();
        }
    }
}
