using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using System.Collections.Generic;

public class ToggleVR : MonoBehaviour
{
    public GameObject xrOrigin; // XR Origin GameObject
    public GameObject xrDeviceSimulator; // XR Device Simulator prefab
    public Camera mainCamera; // Cámara principal para modo no-VR

    private bool isVRActive = false; // Estado inicial de VR
    private float originalFOV; // Campo de visión original de la cámara principal

    void Start()
    {
        // Desactiva los GameObjects relacionados con VR
        originalFOV = mainCamera.fieldOfView;
        xrOrigin.SetActive(false);
        xrDeviceSimulator.SetActive(false);
        StopXR();
    }
    void Update()
    {
        // Detecta si se presiona la tecla 'V'
        if (Input.GetKeyDown(KeyCode.V))
        {
            isVRActive = !isVRActive; // Alterna el estado de VR

            // Activa o desactiva los GameObjects relacionados con VR
            xrOrigin.SetActive(isVRActive);
            xrDeviceSimulator.SetActive(isVRActive);

            // Alterna la cámara principal
            mainCamera.enabled = !isVRActive;


            // Habilita o deshabilita el modo XR
            if (isVRActive)
            {
                Debug.Log("Activando VR...");
                StartXR();
            }
            else
            {
                Debug.Log("Desactivando VR...");
                StopXR();
                mainCamera.fieldOfView = originalFOV;
            }
        }
    }

    private void StartXR()
    {
        var displaySubsystem = GetXRDisplaySubsystem();
        if (displaySubsystem != null && !displaySubsystem.running)
        {
            displaySubsystem.Start();
        }
    }

    private void StopXR()
    {
        var displaySubsystem = GetXRDisplaySubsystem();
        if (displaySubsystem != null && displaySubsystem.running)
        {
            displaySubsystem.Stop();
        }
    }

    private XRDisplaySubsystem GetXRDisplaySubsystem()
    {
        // Obtener todas las instancias de XRDisplaySubsystem
        List<XRDisplaySubsystem> displaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances(displaySubsystems);

        // Retornar el primer subsistema encontrado, si existe
        if (displaySubsystems.Count > 0)
        {
            return displaySubsystems[0];
        }

        return null;
    }
}
