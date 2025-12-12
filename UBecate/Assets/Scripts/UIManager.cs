using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Referencias a los botones
    public Button IDRef;
    public Button BancoRef;
    public Button PapersButton;
    public Button AcceptButton;
    public Button RejectButton;

    // Referencias a las imágenes (GameObjects que se activan/desactivan)
    public GameObject Refer;
    public GameObject Encia;
    public GameObject PapersImagen;

    // Eventos para aceptar/rechazar (los usará el Spawner)
    public delegate void NPCDecision(bool accepted);
    public event NPCDecision OnNPCDecision;

    void Start()
    {
        // Asignar listeners a los botones
        IDRef.onClick.AddListener(() => ToggleImage(Refer));
        BancoRef.onClick.AddListener(() => ToggleImage(Encia));
        PapersButton.onClick.AddListener(() => ToggleImage(PapersImagen));
        AcceptButton.onClick.AddListener(() => HandleDecision(true));
        RejectButton.onClick.AddListener(() => HandleDecision(false));

        // Desactivar el botón de papeles inicialmente
        PapersButton.interactable = false;

        // Desactivar imágenes inicialmente
        Refer.SetActive(false);
        Encia.SetActive(false);
        PapersImagen.SetActive(false);
    }

    // Función para activar/desactivar una imagen (toggle)
    private void ToggleImage(GameObject image)
    {
        image.SetActive(!image.activeSelf);
    }

    // Función para manejar la decisión de aceptar/rechazar
    private void HandleDecision(bool accepted)
    {
        // Invocar el evento para notificar al Spawner
        OnNPCDecision?.Invoke(accepted);

        // Desactivar botones de decisión temporalmente (puedes ajustarlo)
        AcceptButton.interactable = false;
        RejectButton.interactable = false;

        // Opcional: Cerrar imágenes abiertas
        Refer.SetActive(false);
        Encia.SetActive(false);
        PapersImagen.SetActive(false);
    }

    // Función pública para activar el botón de papeles (llamada desde el Spawner)
    public void EnablePapersButton()
    {
        PapersButton.interactable = true;
    }

    // Función para resetear la UI para el próximo NPC
    public void ResetUI()
    {
        PapersButton.interactable = false;
        AcceptButton.interactable = true;
        RejectButton.interactable = true;
        // Cerrar imágenes si están abiertas
        Refer.SetActive(false);
        Encia.SetActive(false);
        PapersImagen.SetActive(false);
    }
}