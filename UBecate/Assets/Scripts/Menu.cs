using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("Paneles Hijos del Fade")]
    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject panelAviso;
    [SerializeField] private GameObject panelJuego;
    [SerializeField] private GameObject panelCreditos;
    [SerializeField] private GameObject panelOpciones;  // NUEVO

    [Header("Referencias a otros componentes")]
    [SerializeField] private IntroManager introManager;  // Referencia al IntroManager del Fade

    [Header("Audio")]
    [SerializeField] private AudioSource musicaMenu;

    private void Start()
    {
        // Estado inicial: Solo menú activo
        MostrarSoloPanel(panelMenu);

        // Iniciar música
        if (musicaMenu != null && !musicaMenu.isPlaying)
            musicaMenu.Play();
    }

    // ===== BOTONES DEL MENÚ PRINCIPAL =====
    
    public void BotonJugar()
    {
        // Apagar música del menú
        if (musicaMenu != null)
            musicaMenu.Stop();

        // Mostrar panel de aviso
        MostrarSoloPanel(panelAviso);

        // Iniciar secuencia del IntroManager
        if (introManager != null)
        {
            introManager.IniciarSecuencia();
        }
    }

    public void BotonCreditos()
    {
        MostrarSoloPanel(panelCreditos);
        StartCoroutine(GetComponent<CreditosManager>().SecuenciaCreditos());
    }

    public void BotonOpciones()
    {
        MostrarSoloPanel(panelOpciones);
    }

    public void BotonSalir()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // ===== NAVEGACIÓN ENTRE PANELES =====

    public void VolverAlMenu()
    {
        MostrarSoloPanel(panelMenu);
        
        // Reiniciar música si no está sonando
        if (musicaMenu != null && !musicaMenu.isPlaying)
            musicaMenu.Play();
    }

    public void CargarSiguienteNivel()
    {
        int escenaActualIndex = SceneManager.GetActiveScene().buildIndex;
        int siguienteEscenaIndex = escenaActualIndex + 1;
    
        if (siguienteEscenaIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(siguienteEscenaIndex);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    // ===== MÉTODO AUXILIAR =====

    private void MostrarSoloPanel(GameObject panelActivo)
    {
        // Desactivar todos los paneles
        if (panelMenu != null) panelMenu.SetActive(false);
        if (panelAviso != null) panelAviso.SetActive(false);
        if (panelJuego != null) panelJuego.SetActive(false);
        if (panelCreditos != null) panelCreditos.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);

        // Activar solo el panel deseado
        if (panelActivo != null) panelActivo.SetActive(true);
    }

    // Método público para que IntroManager pueda cambiar paneles
    public void MostrarPanelJuego()
    {
        MostrarSoloPanel(panelJuego);
    }
}