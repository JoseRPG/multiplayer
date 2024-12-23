using UnityEngine;
using UnityEngine.UI; // Necesario para usar Text

public class CanvasTextManager : MonoBehaviour
{
    public Text vrText;       // Referencia al texto del Canvas VR
    public Text nonVrText;    // Referencia al texto del Canvas no VR
    public Canvas ScoreCanvas; // Referencia al Canvas de puntuación
    public Canvas ScoreCanvasVR;
    private PlayerController playerController; // Referencia al script con la variable pública

    void Start()
    {   
        // Asegurarse de que ambos textos están inicializados
        if (vrText != null) vrText.gameObject.SetActive(false);
        if (nonVrText != null) nonVrText.gameObject.SetActive(false);
        if (ScoreCanvas != null) ScoreCanvas.gameObject.SetActive(false);
        if (ScoreCanvasVR != null) ScoreCanvasVR.gameObject.SetActive(false);

        // Activar solo el texto correspondiente
        if (UnityEngine.XR.XRSettings.isDeviceActive)
        {
            if (vrText != null) vrText.gameObject.SetActive(true);
            if (ScoreCanvasVR != null) ScoreCanvasVR.gameObject.SetActive(true);
        }
        else
        {
            if (nonVrText != null) nonVrText.gameObject.SetActive(true);
            if (ScoreCanvas != null) ScoreCanvas.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (playerController == null)
        {
            // Buscar el script PlayerController en la escena
            playerController = FindObjectOfType<PlayerController>();
        }
        else
        {
            string scoreText = "Puntuación: " + playerController.score.ToString();

            if (UnityEngine.XR.XRSettings.isDeviceActive)
            {
                if (ScoreCanvas != null) ScoreCanvas.gameObject.SetActive(false);
                if (nonVrText != null) nonVrText.gameObject.SetActive(false);
                if (ScoreCanvasVR != null) ScoreCanvasVR.gameObject.SetActive(true);
                if (vrText != null) vrText.gameObject.SetActive(true);
                
                if (UnityEngine.XR.XRSettings.isDeviceActive && vrText != null)
                {
                    // Actualizar el texto VR
                    vrText.text = scoreText;
                }
            }
            else
            {
                if (ScoreCanvas != null) ScoreCanvas.gameObject.SetActive(true);
                if (nonVrText != null) nonVrText.gameObject.SetActive(true);
                if (ScoreCanvasVR != null) ScoreCanvasVR.gameObject.SetActive(false);
                if (vrText != null) vrText.gameObject.SetActive(false);

                if (!UnityEngine.XR.XRSettings.isDeviceActive && nonVrText != null)
                {
                    // Actualizar el texto normal
                    nonVrText.text = scoreText;
                }
            }     
        }
    }
}
