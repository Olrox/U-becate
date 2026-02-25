using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Para CanvasGroup y RawImage
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SecondDialogueLine
{
    [Header("Nombre del hablante")]
    public string speakerName;
    
    [Header("Texto de la línea")]
    public string lineText;
}

public class CinematicInterManager : MonoBehaviour
{
    [Header("Panel Inicial (aparece al inicio y se desvanece)")]
    public GameObject panelInicial;
    public TextMeshProUGUI textoInicial;
    public float fadeDuration = 2f;  // Duración del fade in/out

    [Header("Sistema de Diálogo")]
    public TextMeshProUGUI dialogeText;
    public TextMeshProUGUI nameText;
    public DialogueLine[] dialogueLines;
    public float textSpeed = 0.07f;
    public float defaultLineWaitTime = 2f;  // Espera después de cada línea

    [Header("Imagen Titilante (aparece en la línea 3)")]
    public GameObject imagenTitilante;
    public int titilosCount = 5;           // Número de parpadeos completos
    public float titiloDuration = 0.2f;    // Duración de cada fade (rápido para titilar)
    private CanvasGroup canvasGroupTitilante;  // Para fade suave
    [Header("Panel Tercero (línea 3: pausa con botón)")]
    public GameObject panelTercero;
    public Button botonTercero;

private bool terceroPanelClicado = false;

    [Header("Panel Séptimo (línea 7: imagen + botón para continuar)")]
    public GameObject panelSeptimo;
    public UnityEngine.UI.Button botonSeptimo;  // Asigna el botón aquí

    private CanvasGroup canvasGroupInicial;  // Para fade del panel inicial
    private bool septimoPanelClicado = false;  // Bandera para continuar después del clic

    void Start()
    {
        // Inicializaciones
        if (panelInicial != null)
        {
            canvasGroupInicial = panelInicial.GetComponent<CanvasGroup>();
            if (canvasGroupInicial == null)
                canvasGroupInicial = panelInicial.AddComponent<CanvasGroup>();  // Auto-agrega si no existe
        }

        // Limpia textos
        if (dialogeText != null) dialogeText.text = string.Empty;
        if (nameText != null) nameText.text = string.Empty;
        if (textoInicial != null) textoInicial.alpha = 1f;  // Empieza invisible

        // Oculta elementos iniciales
        if (imagenTitilante != null) imagenTitilante.SetActive(false);
        if (panelSeptimo != null) panelSeptimo.SetActive(false);

        // Inicia el flujo completo
        StartCoroutine(FlujoCinematicoCompleto());

        if (imagenTitilante != null)
        {
            canvasGroupTitilante = imagenTitilante.GetComponent<CanvasGroup>();
            if (canvasGroupTitilante == null)
                canvasGroupTitilante = imagenTitilante.AddComponent<CanvasGroup>(); 
        }
        if (panelTercero != null)
        panelTercero.SetActive(false);
    }

    IEnumerator FlujoCinematicoCompleto()
    {
        // 1. Fade IN panel inicial
        yield return StartCoroutine(FadePanelInicial(true, fadeDuration));

        // 2. Espera un poco con texto visible
        yield return new WaitForSeconds(1f);

        // 3. Fade OUT panel inicial
        yield return StartCoroutine(FadePanelInicial(false, fadeDuration));

        // 4. Inicia diálogo
        yield return StartCoroutine(PlayDialogueSequence());
    }

    IEnumerator FadePanelInicial(bool fadeIn, float duration)
    {
        if (panelInicial == null || canvasGroupInicial == null) yield break;

        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;
        float elapsed = 0f;

        canvasGroupInicial.alpha = startAlpha;
        panelInicial.SetActive(true);  // Activa antes del fade

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroupInicial.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroupInicial.alpha = endAlpha;

        if (!fadeIn) panelInicial.SetActive(false);  // Desactiva al final del fade out
    }

    IEnumerator PlayDialogueSequence()
    {
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            DialogueLine currentLine = dialogueLines[i];

            // Cambia nombre
            if (nameText != null)
                nameText.text = currentLine.speakerName + ": ";

            // Escribe texto
            yield return StartCoroutine(TypeText(currentLine.lineText));

            // Evento especial: Línea 3 (index 2)
            if (i == 2)
            {
                if (imagenTitilante != null)
                    yield return StartCoroutine(TitilarImagen());

                if (panelTercero != null)
                {
                    // ── CAMBIO AQUÍ ──────────────────────────────────────────────────────
                    panelTercero.SetActive(true);               // Primero se muestra el panel
                    //botonTercero.interactable = false;          // Botón desactivado al inicio

                    // Espera 5 segundos antes de habilitar el botón
                    yield return new WaitForSeconds(9f);

                    botonTercero.interactable = true;           // Ahora sí se puede interactuar

                    // Espera hasta que el jugador presione el botón
                    while (!terceroPanelClicado)
                    {
                        yield return null;
                    }

                    panelTercero.SetActive(false);
                    terceroPanelClicado = false; // Reset
                    // ────────────────────────────────────────────────────────────────────
                }
            }

            // Evento especial: Línea 7 (index 6) - Pausa y espera clic
            if (i == 7 && panelSeptimo != null)
            {
                panelSeptimo.SetActive(true);
                botonSeptimo.interactable = true;

                // Espera hasta que se haga clic en el botón
                while (!septimoPanelClicado)
                {
                    yield return null;
                }

                panelSeptimo.SetActive(false);
                septimoPanelClicado = false;  // Reset
            }

            // Espera normal después de la línea
            yield return new WaitForSeconds(defaultLineWaitTime);
        }

        // 5. Al finalizar: Carga siguiente nivel
        yield return new WaitForSeconds(1f);  // Pausa final opcional
        CargarSiguienteNivel();
    }

    IEnumerator TypeText(string text)
    {
        if (dialogeText == null) yield break;

        dialogeText.text = string.Empty;
        foreach (char letter in text.ToCharArray())
        {
            dialogeText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    IEnumerator TitilarImagen()
{
    if (imagenTitilante == null || canvasGroupTitilante == null) yield break;

    imagenTitilante.SetActive(true);
    botonTercero.interactable = false;
    canvasGroupTitilante.alpha = 1f;  // Empieza visible

    for (int i = 0; i < titilosCount; i++)
    {
        // Fade OUT (1 → 0)
        yield return StartCoroutine(FadeTitilo(1f, 0f, titiloDuration));
        
        // Fade IN (0 → 1)
        yield return StartCoroutine(FadeTitilo(0f, 1f, titiloDuration));
    }

    // Fade final OUT para ocultar
    yield return StartCoroutine(FadeTitilo(1f, 1f, titiloDuration));
    //imagenTitilante.SetActive(false);
}

// Nueva corrutina reutilizable para cada fade (con SmoothStep para suavidad extrema)
    private IEnumerator FadeTitilo(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        float startA = canvasGroupTitilante.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float easedT = Mathf.SmoothStep(0f, 1f, t);  // ¡Suavizado perfecto!
            canvasGroupTitilante.alpha = Mathf.Lerp(startAlpha, endAlpha, easedT);
            yield return null;
        }

        canvasGroupTitilante.alpha = endAlpha;
    }

    // Llamada por el botón séptimo (OnClick en Unity)
    public void OnBotonSeptimoClicked()
    {
        septimoPanelClicado = true;
        botonSeptimo.interactable = false;  // Desactiva para evitar clics múltiples
    }

    private void CargarSiguienteNivel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            SceneManager.LoadScene(0);  // Vuelve al menú si es la última
        }
    }
    public void OnBotonTerceroClicked()
    {
        terceroPanelClicado = true;
        botonTercero.interactable = false;
    }
}
