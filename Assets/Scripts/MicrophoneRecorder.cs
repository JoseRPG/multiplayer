using UnityEngine;
using System.IO;

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

            SaveRecordingToWav(audioSource.clip, "Recording_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".wav");
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

    private void SaveRecordingToWav(AudioClip clip, string fileName)
    {
        if (clip == null)
        {
            Debug.LogError("No hay grabación para guardar.");
            return;
        }

        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        Debug.Log("Guardando grabación en: " + filePath);

        // Convertir el AudioClip a un array de bytes
        byte[] wavData = ConvertAudioClipToWav(clip);

        // Guardar los bytes en un archivo .wav
        File.WriteAllBytes(filePath, wavData);
        Debug.Log("Grabación guardada correctamente.");
    }

    private byte[] ConvertAudioClipToWav(AudioClip clip)
    {
        // Obtener los datos de audio
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        // Convertir a formato PCM
        byte[] pcmData = new byte[samples.Length * 2]; // 16 bits por muestra
        for (int i = 0; i < samples.Length; i++)
        {
            short sample = (short)(samples[i] * 32767);
            pcmData[i * 2] = (byte)(sample & 0xFF);
            pcmData[i * 2 + 1] = (byte)((sample >> 8) & 0xFF);
        }

        // Escribir encabezado WAV
        byte[] header = WriteWavHeader(clip, pcmData.Length);

        // Combinar encabezado y datos PCM
        byte[] wavData = new byte[header.Length + pcmData.Length];
        header.CopyTo(wavData, 0);
        pcmData.CopyTo(wavData, header.Length);

        return wavData;
    }

    private byte[] WriteWavHeader(AudioClip clip, int dataSize)
    {
        int sampleRate = clip.frequency;
        int channels = clip.channels;
        int byteRate = sampleRate * channels * 2; // 16 bits por muestra

        using (MemoryStream stream = new MemoryStream(44))
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + dataSize);
                writer.Write("WAVE".ToCharArray());

                writer.Write("fmt ".ToCharArray());
                writer.Write(16); // Tamaño del subchunk
                writer.Write((short)1); // Formato de audio (1 = PCM)
                writer.Write((short)channels);
                writer.Write(sampleRate);
                writer.Write(byteRate);
                writer.Write((short)(channels * 2)); // Tamaño del bloque
                writer.Write((short)16); // Bits por muestra

                writer.Write("data".ToCharArray());
                writer.Write(dataSize);
            }

            return stream.ToArray();
        }
    }
}
