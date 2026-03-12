using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

// ESTA ES LA CLASE QUE FALTABA
[System.Serializable]
public class DialogueLine
{
    [Header("Nombre del hablante")]
    public string speakerName;
    
    [Header("Texto de la línea")]
    [TextArea(3,6)]
    public string lineText;
    
    [Header("Audio de la línea")]
    public AudioClip audioNarrado;
    
    [Header("Personaje que habla")]
    public Sprite imagenPersonaje;
}

public class IntroManager : MonoBehaviour
{
    [Header("Referencias al Menú")]
    public Menu menuManager;

    [Header("Panel de Click para Avanzar")]
    [Tooltip("Botón invisible en toda la pantalla")]
    public GameObject panelInteraccionClick;

    [Header("Panel de Diálogo Principal")]
    [Tooltip("El contenedor de toda la UI de diálogo (Fondo, Texto, Botones)")]
    public GameObject panelContenedorDialogo;

    [Header("Panel de Aviso")]
    public float tiempoAviso = 10f;

    [Header("Texto del diálogo")]
    public TextMeshProUGUI dialogeText;
    public TextMeshProUGUI nameText;
    
    [Header("Líneas del diálogo")]
    public DialogueLine[] dialogueLines;
    public float textSpeed = 0.07f;

    [Header("Sistema de Audio")]
    public AudioSource audioSource;
    public bool permitirSaltarAudio = true;
    
    [Header("Sistema de Imágenes de Personajes")]
    public Image imagenPersonajeUI;
    public GameObject panelImagenPersonaje;
    
    [Header("Animación de Personajes")]
    public bool usarFadePersonajes = true;
    public float velocidadFadePersonaje = 2f;
    private CanvasGroup canvasGroupPersonaje;

    [Header("Botones de decisión")]
    public GameObject[] botonesDecision;

    [Header("Negación")]
    public string negacionText = "¡Has elegido la negación!...";
    public AudioClip audioNegacion;
    public float reloadDelay = 3f;

    private string targetText;
    private int index;
    private bool isTyping = false;
    private bool dialogoTerminado = false;
    private Coroutine typeCoroutine;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        if (imagenPersonajeUI != null && usarFadePersonajes)
        {
            canvasGroupPersonaje = imagenPersonajeUI.GetComponent<CanvasGroup>() ?? imagenPersonajeUI.gameObject.AddComponent<CanvasGroup>();
            canvasGroupPersonaje.alpha = 0f;
        }
    }

    public void IniciarSecuencia()
    {
        StartCoroutine(SecuenciaCompleta());
    }

    IEnumerator SecuenciaCompleta()
    {
        if(panelInteraccionClick != null) panelInteraccionClick.SetActive(false);
        yield return new WaitForSeconds(tiempoAviso);

        if (menuManager != null) menuManager.MostrarPanelJuego();

        PrepararDialogo();
        StartDialogue();
    }

    void PrepararDialogo()
    {
        if (dialogeText != null) dialogeText.text = string.Empty;
        if (nameText != null) nameText.text = string.Empty;

        isTyping = false;
        dialogoTerminado = false;

        if (panelImagenPersonaje != null) panelImagenPersonaje.SetActive(false);
        if (panelContenedorDialogo != null) panelContenedorDialogo.SetActive(true);
        
        SetDecisionButtonsActive(false);
        if(panelInteraccionClick != null) panelInteraccionClick.SetActive(true);
    }

    private void StartDialogue()
    {
        index = 0;
        if (dialogueLines != null && dialogueLines.Length > 0)
        {
            SetSpeaker(dialogueLines[0].speakerName);
            StartCoroutine(MostrarLineaCompleta(dialogueLines[0]));
        }
    }

    private void SetSpeaker(string speakerName)
    {
        if (nameText != null) nameText.text = speakerName + ": ";
    }

    public void IntentoAvanzarDialogo()
    {
        if (dialogoTerminado) return;

        if (isTyping)
        {
            CompletarTextoInmediatamente();
        }
        else
        {
            NextLine();
        }
    }

    private void CompletarTextoInmediatamente()
    {
        if (typeCoroutine != null) StopCoroutine(typeCoroutine);
        isTyping = false;
        if (dialogeText != null) dialogeText.text = targetText;
        if (permitirSaltarAudio && audioSource != null) audioSource.Stop();
    }

    public void NextLine()
    {
        if (index < dialogueLines.Length - 1)
        {
            index++;
            SetSpeaker(dialogueLines[index].speakerName);
            StartCoroutine(MostrarLineaCompleta(dialogueLines[index]));
        }
        else
        {
            FinalizarYEsperarDecision();
        }
    }

    private void FinalizarYEsperarDecision()
    {
        dialogoTerminado = true;
        
        // Apagamos el panel de clics para que no estorbe a los botones
        if(panelInteraccionClick != null) panelInteraccionClick.SetActive(false);
        
        // Mostramos los botones
        SetDecisionButtonsActive(true);

        // Hacemos que el personaje se vaya si quieres, pero NO apagamos el panelContenedorDialogo
        if (usarFadePersonajes && canvasGroupPersonaje != null)
            StartCoroutine(FadeOutPersonaje());
    }

    IEnumerator MostrarLineaCompleta(DialogueLine linea)
    {
        if (linea.imagenPersonaje != null)
            yield return StartCoroutine(CambiarImagenPersonaje(linea.imagenPersonaje));

        if (linea.audioNarrado != null && audioSource != null)
        {
            audioSource.clip = linea.audioNarrado;
            audioSource.Play();
        }

        typeCoroutine = StartCoroutine(TypeText(linea.lineText));
        yield return typeCoroutine;
    }

    IEnumerator TypeText(string text)
    {
        targetText = text;
        isTyping = true;
        dialogeText.text = string.Empty;
        foreach (char letter in text.ToCharArray())
        {
            dialogeText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
    }

    IEnumerator CambiarImagenPersonaje(Sprite nuevaImagen)
    {
        if (imagenPersonajeUI == null) yield break;
        if (panelImagenPersonaje != null) panelImagenPersonaje.SetActive(true);
        if (usarFadePersonajes && canvasGroupPersonaje != null)
        {
            while (canvasGroupPersonaje.alpha > 0) { canvasGroupPersonaje.alpha -= Time.deltaTime * velocidadFadePersonaje; yield return null; }
            imagenPersonajeUI.sprite = nuevaImagen;
            while (canvasGroupPersonaje.alpha < 1) { canvasGroupPersonaje.alpha += Time.deltaTime * velocidadFadePersonaje; yield return null; }
        }
        else { imagenPersonajeUI.sprite = nuevaImagen; }
    }

    IEnumerator FadeOutPersonaje()
    {
        if (canvasGroupPersonaje == null) yield break;
        while (canvasGroupPersonaje.alpha > 0) { canvasGroupPersonaje.alpha -= Time.deltaTime * velocidadFadePersonaje; yield return null; }
        if (panelImagenPersonaje != null) panelImagenPersonaje.SetActive(false);
    }

    private void SetDecisionButtonsActive(bool active)
    {
        if (botonesDecision == null) return;
        foreach (GameObject boton in botonesDecision) if (boton != null) boton.SetActive(active);
    }

    public void Negacion()
    {
        StartCoroutine(NegacionSequence());
    }

    IEnumerator NegacionSequence()
    {
        SetDecisionButtonsActive(false);
        if (nameText != null) nameText.text = "";
        
        yield return StartCoroutine(TypeText(negacionText));
        yield return new WaitForSeconds(reloadDelay);
        
        if (menuManager != null) menuManager.VolverAlMenu();
        else SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}