using UnityEngine;
using UnityEngine.EventSystems;

public class Cerradura : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject[] panelsToClose; // Arrastra aquí los paneles que quieres cerrar

    public void OnPointerClick(PointerEventData eventData)
    {
        // Solo cierra si el clic fue en este fondo (no en hijos)
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            foreach (var panel in panelsToClose)
            {
                if (panel != null)
                    panel.SetActive(false);
            }
        }
    }
}