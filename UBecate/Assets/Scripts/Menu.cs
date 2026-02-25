using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    [Header("Paneles UI")]
    [SerializeField] private GameObject panelMenuPrincipal;    // Panel inicial con botón "Comenzar", música, etc.
    [SerializeField] private GameObject panelInicioJuego;      // Panel que se muestra al comenzar (instrucciones, historia, "Listo", etc.)
    [SerializeField] private GameObject panelCreditos;         // Panel de créditos

    [Header("Créditos - Configuración")]
    [SerializeField] private TextMeshProUGUI textoCreditos;    // Texto que se moverá
    [SerializeField] private Button botonAcelerarCreditos;     // Botón para acelerar los créditos
    [TextArea(2, 4)]
    [SerializeField] private string[] lineasCreditos = new string[5]; // Array de 5 líneas de texto

    [Header("Créditos - Animación")]
    [SerializeField] private float velocidadNormal = 100f;      // Velocidad normal de desplazamiento
    [SerializeField] private float velocidadRapida = 300f;      // Velocidad acelerada
    [SerializeField] private float tiempoEntreLineas = 0.5f;    // Tiempo entre cada línea

    [Header("Opcional - Audio")]
    [SerializeField] private AudioSource musicaMenu;           // Si tienes música de fondo en el menú

    private bool acelerarCreditos = false;
    private RectTransform rectTextoCreditos;

    private void Start()
    {
        // Aseguramos el estado inicial correcto
        if (panelMenuPrincipal != null)
            panelMenuPrincipal.SetActive(true);

        if (panelInicioJuego != null)
            panelInicioJuego.SetActive(false);

        if (panelCreditos != null)
            panelCreditos.SetActive(false);

        // Obtener RectTransform del texto de créditos
        if (textoCreditos != null)
            rectTextoCreditos = textoCreditos.GetComponent<RectTransform>();

        // Configurar botón de acelerar
        if (botonAcelerarCreditos != null)
        {
            botonAcelerarCreditos.onClick.AddListener(AcelerarCreditos);
        }

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
        int escenaActualIndex = SceneManager.GetActiveScene().buildIndex;
        int siguienteEscenaIndex = escenaActualIndex + 1;
    
        // Verifica si hay una siguiente escena en Build Settings
        if (siguienteEscenaIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(siguienteEscenaIndex);
        }
        else
        {
            // Si es la última escena, carga la primera (o un menú principal)
            SceneManager.LoadScene(0);
        }
    }

    // ===== SISTEMA DE CRÉDITOS =====

    public void MostrarCreditos()
    {
        // Desactivar menú principal y activar panel de créditos
        if (panelMenuPrincipal != null)
            panelMenuPrincipal.SetActive(false);

        if (panelCreditos != null)
            panelCreditos.SetActive(true);

        // Resetear velocidad
        acelerarCreditos = false;

        // Mostrar botón de acelerar
        if (botonAcelerarCreditos != null)
            botonAcelerarCreditos.gameObject.SetActive(true);

        // Iniciar secuencia de créditos
        StartCoroutine(SecuenciaCreditos());
    }

    IEnumerator SecuenciaCreditos()
    {
        // Recorrer las 5 líneas de créditos
        for (int i = 0; i < lineasCreditos.Length && i < 5; i++)
        {
            yield return StartCoroutine(MostrarLineaCredito(lineasCreditos[i]));
            
            // Esperar un poco entre líneas (solo si no es la última)
            if (i < lineasCreditos.Length - 1)
            {
                yield return new WaitForSeconds(tiempoEntreLineas);
            }
        }

        // Ocultar el botón de acelerar
        if (botonAcelerarCreditos != null)
            botonAcelerarCreditos.gameObject.SetActive(false);

        // Esperar un momento antes de recargar
        yield return new WaitForSeconds(0.5f);

        // Recargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator MostrarLineaCredito(string textoLinea)
    {
        if (textoCreditos == null || rectTextoCreditos == null)
            yield break;

        // Configurar el texto
        textoCreditos.text = textoLinea;

        // Obtener el tamaño del canvas/pantalla
        Canvas canvas = panelCreditos.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float alturaCanvas = canvasRect.rect.height;

        // Posición inicial: debajo de la pantalla, en el centro horizontal
        rectTextoCreditos.anchoredPosition = new Vector2(0, -alturaCanvas / 2 - 100);

        // Posición final: arriba de la pantalla
        float posicionFinalY = alturaCanvas / 2 + 100;

        // Animar el texto desde abajo hacia arriba
        while (rectTextoCreditos.anchoredPosition.y < posicionFinalY)
        {
            // Usar velocidad actual (normal o rápida)
            float velocidadActual = acelerarCreditos ? velocidadRapida : velocidadNormal;
            
            // Mover hacia arriba
            rectTextoCreditos.anchoredPosition += new Vector2(0, velocidadActual * Time.deltaTime);
            
            yield return null;
        }
    }

    void AcelerarCreditos()
    {
        acelerarCreditos = true;
        
        // Opcional: cambiar el texto del botón para dar feedback
        TextMeshProUGUI textoBoton = botonAcelerarCreditos.GetComponentInChildren<TextMeshProUGUI>();
        if (textoBoton != null)
        {
            textoBoton.text = "Acelerado >>>";
        }
    }

    // Método para volver al menú principal desde los créditos (opcional)
    public void VolverAlMenuDesdeCreditos()
    {
        StopAllCoroutines();
        
        if (panelCreditos != null)
            panelCreditos.SetActive(false);

        if (panelMenuPrincipal != null)
            panelMenuPrincipal.SetActive(true);
    }
}