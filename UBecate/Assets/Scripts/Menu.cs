using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    [Header("Paneles UI")]
    [SerializeField] private GameObject panelMenuPrincipal;    // Panel inicial con botón "Comenzar", música, etc.
    [SerializeField] private GameObject panelInicioJuego;      // Panel que se muestra al comenzar (instrucciones, historia, "Listo", etc.)

    [Header("Opcional - Audio")]
    [SerializeField] private AudioSource musicaMenu;           // Si tienes música de fondo en el menú

    private void Start()
    {
        // Aseguramos el estado inicial correcto
        if (panelMenuPrincipal != null)
            panelMenuPrincipal.SetActive(true);

        if (panelInicioJuego != null)
            panelInicioJuego.SetActive(false);

        // Si tienes música, aquí podrías iniciarla
        if (musicaMenu != null && !musicaMenu.isPlaying)
            musicaMenu.Play();
    }
    public void ComenzarJuego()
    {
        // 1. Desactivamos el panel del menú (y su música si existe)
        if (panelMenuPrincipal != null)
        {
            panelMenuPrincipal.SetActive(false);
        }

        if (musicaMenu != null)
        {
            musicaMenu.Stop();   // o FadeOut si quieres hacerlo más elegante
        }

        // 2. Activamos el panel de inicio del juego
        if (panelInicioJuego != null)
        {
            panelInicioJuego.SetActive(true);
        }
    }
    public void CargarJuego()
    {
        SceneManager.LoadScene("Lvl1");
    }
    
}