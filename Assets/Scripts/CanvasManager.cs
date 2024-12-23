using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    public Canvas vrCanvas;     // Canvas diseñado para VR (World Space)
    public Canvas nonVrCanvas; // Canvas estándar (Screen Space)

    void Start()
    {
        // Asegurarse de que ambos Canvas estén desactivados inicialmente
        if (vrCanvas != null) vrCanvas.gameObject.SetActive(false);
        if (nonVrCanvas != null) nonVrCanvas.gameObject.SetActive(false);
    }

    // Método público para mostrar el Canvas de VR
    public void ShowVrCanvas()
    {
        if (vrCanvas != null)
        {
            vrCanvas.gameObject.SetActive(true);
        }
        if (nonVrCanvas != null)
        {
            nonVrCanvas.gameObject.SetActive(false);
        }
        Debug.Log("VR Canvas activado");
    }

    // Método público para mostrar el Canvas estándar
    public void ShowNonVrCanvas()
    {
        if (nonVrCanvas != null)
        {
            nonVrCanvas.gameObject.SetActive(true);
        }
        if (vrCanvas != null)
        {
            vrCanvas.gameObject.SetActive(false);
        }
        Debug.Log("Non-VR Canvas activado");
    }

    public static void RestartGame()
    {
        Debug.Log("Reiniciando el juego...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recarga la escena actual
    }
}
