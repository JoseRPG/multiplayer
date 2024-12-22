using UnityEngine;

public class WebcamDisplay : MonoBehaviour
{
    private WebCamTexture webCamTexture;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (WebCamTexture.devices.Length > 0)
        {
            webCamTexture = new WebCamTexture();
            renderer.material.mainTexture = webCamTexture;
            webCamTexture.Play();
        }
        else
        {
            Debug.LogWarning("No se encontró ninguna cámara web.");
        }
    }
}
