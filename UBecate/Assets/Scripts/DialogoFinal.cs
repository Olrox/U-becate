using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogoFinal : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelInicial;           // Panel que se desvanecerá al inicio
    public CanvasGroup panelInicialCanvasGroup; // CanvasGroup del panel inicial para fade
    public GameObject panelDialogo;           // Panel donde aparecen los diálogos
    public GameObject imagenFinal;            // Imagen que aparece al final
    public GameObject botonRegreso;           // Botón para regresar al menú
    
    [Header("Textos")]
    public TextMeshProUGUI dialogoText;       // Texto del diálogo
    public TextMeshProUGUI nombreHablanteText; // Nombre del hablante (opcional)
    
    [Header("Audio")]
    public AudioSource audioSource;           // AudioSource para reproducir sonidos
    public AudioClip sonidoEspecial;          // Sonido que se reproduce en medio
    public AudioClip musicaFondo;             // Música de fondo
    
    [Header("Configuración de Tiempo")]
    public float tiempoFadeInicial = 2f;      // Tiempo para desvanecer el panel inicial
    public float tiempoAntesDeDialogo = 1f;   // Tiempo de espera antes del primer diálogo
    public float velocidadEscritura = 0.05f;  // Velocidad de escritura del texto (segundos por letra)
    public float tiempoEntrDialogos = 2f;     // Tiempo entre diálogos
    public float tiempoAntesSonido = 2f;      // Tiempo antes de reproducir el sonido
    public float tiempoAntesImagen = 1f;      // Tiempo antes de mostrar la imagen final
    
    [Header("Escena de Regreso")]
    public string escenaInicio = "MenuPrincipal"; // Nombre de la escena del menú inicial
    
    [System.Serializable]
    private class Dialogo
    {
        public string nombreHablante;
        public string texto;
        public bool reproducirSonidoDespues; // Si debe reproducir sonido después de este diálogo
        public bool reproducirMusicaDespues; // Si debe reproducir música después de este diálogo
        
        public Dialogo(string nombre, string texto, bool sonido = false, bool musica = false)
        {
            this.nombreHablante = nombre;
            this.texto = texto;
            this.reproducirSonidoDespues = sonido;
            this.reproducirMusicaDespues = musica;
        }
    }
    
    private Dialogo[] dialogos;
    
    void Awake()
    {
        // Configura tus diálogos aquí
        dialogos = new Dialogo[]
        {
            new Dialogo("Yo", "Han pasado algunas semanas y he estado revisando constantementede manera constante el portal de INTEGRA para conocer el estatus de mi beca. M, mi pago único de beca debería caer el día de hoy", false, false),
            new Dialogo("Yo", "Mi pago único de beca debería caer el día de hoy.", true, false), // Reproduce sonido después
            new Dialogo("Yo", "¡YA CAYÓ LA BECA!", false, true), // Reproduce música después
            new Dialogo("Yo", "¡Gracias compañero!", false, false)
        };
    }
    
    void Start()
    {
        // Inicializar estado
        if (imagenFinal != null) imagenFinal.SetActive(false);
        if (botonRegreso != null) botonRegreso.SetActive(false);
        if (panelDialogo != null) panelDialogo.SetActive(false);
        
        // Asegurarse de que el panel inicial tenga un CanvasGroup
        if (panelInicial != null && panelInicialCanvasGroup == null)
        {
            panelInicialCanvasGroup = panelInicial.GetComponent<CanvasGroup>();
            if (panelInicialCanvasGroup == null)
            {
                panelInicialCanvasGroup = panelInicial.AddComponent<CanvasGroup>();
            }
        }
        
        StartCoroutine(SecuenciaDialogoFinal());
    }
    
    IEnumerator SecuenciaDialogoFinal()
    {
        // 1. Desvanecer panel inicial
        if (panelInicial != null && panelInicialCanvasGroup != null)
        {
            yield return StartCoroutine(DesvanecerPanel(panelInicialCanvasGroup, tiempoFadeInicial));
            panelInicial.SetActive(false);
        }
        
        // 2. Esperar un momento antes de mostrar diálogos
        yield return new WaitForSeconds(tiempoAntesDeDialogo);
        
        // 3. Activar panel de diálogo
        if (panelDialogo != null) panelDialogo.SetActive(true);
        
        // 4. Mostrar cada diálogo con efecto de escritura
        foreach (var dialogo in dialogos)
        {
            // Mostrar nombre del hablante
            if (nombreHablanteText != null)
            {
                nombreHablanteText.text = dialogo.nombreHablante;
            }
            
            // Escribir el texto con efecto de máquina de escribir
            yield return StartCoroutine(EscribirTexto(dialogo.texto));
            
            // Esperar entre diálogos
            yield return new WaitForSeconds(tiempoEntrDialogos);
            
            // Reproducir sonido si está marcado
            if (dialogo.reproducirSonidoDespues && sonidoEspecial != null)
            {
                yield return new WaitForSeconds(tiempoAntesSonido);
                audioSource.PlayOneShot(sonidoEspecial);
                yield return new WaitForSeconds(sonidoEspecial.length); // Esperar a que termine el sonido
            }
            
            // Reproducir música si está marcado
            if (dialogo.reproducirMusicaDespues && musicaFondo != null)
            {
                audioSource.clip = musicaFondo;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        
        // 5. Esperar antes de mostrar imagen final
        yield return new WaitForSeconds(tiempoAntesImagen);
        
        // 6. Mostrar imagen final y ocultar panel de diálogo simultáneamente
        if (imagenFinal != null) imagenFinal.SetActive(true);
        if (panelDialogo != null) panelDialogo.SetActive(false);
        
        // 7. Mostrar botón de regreso
        if (botonRegreso != null) botonRegreso.SetActive(true);
    }
    
    // Efecto de escritura gradual
    IEnumerator EscribirTexto(string texto)
    {
        dialogoText.text = "";
        
        foreach (char letra in texto)
        {
            dialogoText.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }
    }
    
    // Desvanecer panel gradualmente
    IEnumerator DesvanecerPanel(CanvasGroup canvasGroup, float duracion)
    {
        float tiempoTranscurrido = 0f;
        
        while (tiempoTranscurrido < duracion)
        {
            tiempoTranscurrido += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, tiempoTranscurrido / duracion);
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
    }
    
    // Función pública para el botón de regreso
    public void RegresarAlInicio()
    {
        // Detener música si está reproduciéndose
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
        SceneManager.LoadScene(0);
    }
}