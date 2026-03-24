using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

// ── Mapeo de personaje → sprite ──────────────────────────────────
[System.Serializable]
public class PersonajeRetrato
{
    public string nombrePersonaje;   // Debe coincidir EXACTO con speakerName
    public Sprite retrato;           // Arrastra el sprite desde el Inspector
}
// ────────────────────────────────────────────────────────────────

public class CinematicInterManager : MonoBehaviour
{
    [Header("Panel Inicial (aparece al inicio y se desvanece)")]
    public GameObject panelInicial;
    public TextMeshProUGUI textoInicial;
    public float fadeDuration = 2f;

    [Header("Sistema de Diálogo")]
    public TextMeshProUGUI dialogeText;
    public TextMeshProUGUI nameText;
    public DialogueLine[] dialogueLines;
    public float textSpeed = 0.07f;
    public float defaultLineWaitTime = 2f;

    [Header("Audio Bequín")]
    public AudioSource audioSourceBequín;
    public AudioClip[] audioClipsBequin;
    public AudioClip audioClipTitilante;
    private int indiceBequin = 0;

    // ── Retratos de personajes ───────────────────────────────────
    [Header("Retratos de Personajes")]
    public Image imagenPersonaje;            // Image de Unity UI donde se muestra el retrato
    public PersonajeRetrato[] retratos;      // Lista de personaje → sprite en el Inspector
    public Sprite retratoVacio;              // Sprite en blanco/transparente (opcional)
    // ────────────────────────────────────────────────────────────

    [Header("Imagen Titilante (aparece en la línea 3)")]
    public GameObject imagenTitilante;
    public int titilosCount = 5;
    public float titiloDuration = 0.2f;
    private CanvasGroup canvasGroupTitilante;

    [Header("Panel Tercero (línea 3: pausa con botón)")]
    public GameObject panelTercero;
    public Button botonTercero;
    private bool terceroPanelClicado = false;

    [Header("Panel Séptimo (línea 7: imagen + botón para continuar)")]
    public GameObject panelSeptimo;
    public UnityEngine.UI.Button botonSeptimo;

    private CanvasGroup canvasGroupInicial;
    private bool septimoPanelClicado = false;

    void Start()
    {
        if (panelInicial != null)
        {
            canvasGroupInicial = panelInicial.GetComponent<CanvasGroup>();
            if (canvasGroupInicial == null)
                canvasGroupInicial = panelInicial.AddComponent<CanvasGroup>();
        }

        if (dialogeText != null) dialogeText.text = string.Empty;
        if (nameText != null) nameText.text = string.Empty;
        if (textoInicial != null) textoInicial.alpha = 1f;

        if (imagenTitilante != null)
        {
            imagenTitilante.SetActive(false);
            canvasGroupTitilante = imagenTitilante.GetComponent<CanvasGroup>();
            if (canvasGroupTitilante == null)
                canvasGroupTitilante = imagenTitilante.AddComponent<CanvasGroup>();
        }

        if (panelSeptimo != null) panelSeptimo.SetActive(false);
        if (panelTercero != null) panelTercero.SetActive(false);

        // Oculta retrato al inicio
        if (imagenPersonaje != null) imagenPersonaje.gameObject.SetActive(false);

        StartCoroutine(FlujoCinematicoCompleto());
    }

    IEnumerator FlujoCinematicoCompleto()
    {
        yield return StartCoroutine(FadePanelInicial(true, fadeDuration));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadePanelInicial(false, fadeDuration));
        yield return StartCoroutine(PlayDialogueSequence());
    }

    IEnumerator FadePanelInicial(bool fadeIn, float duration)
    {
        if (panelInicial == null || canvasGroupInicial == null) yield break;

        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha   = fadeIn ? 1f : 0f;
        float elapsed = 0f;

        canvasGroupInicial.alpha = startAlpha;
        panelInicial.SetActive(true);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroupInicial.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroupInicial.alpha = endAlpha;
        if (!fadeIn) panelInicial.SetActive(false);
    }

    IEnumerator PlayDialogueSequence()
    {
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            DialogueLine currentLine = dialogueLines[i];

            // Nombre del hablante
            if (nameText != null)
                nameText.text = currentLine.speakerName + ": ";

            // ── Retrato del personaje ────────────────────────────
            ActualizarRetrato(currentLine.speakerName);
            // ────────────────────────────────────────────────────

            // Audio de Bequín
            if (currentLine.speakerName == "Bequín" && audioSourceBequín != null &&
                audioClipsBequin != null && indiceBequin < audioClipsBequin.Length)
            {
                audioSourceBequín.Stop();
                audioSourceBequín.clip = audioClipsBequin[indiceBequin];
                audioSourceBequín.Play();
                indiceBequin++;
            }

            // ── BUG FIX: solo una llamada a TypeText ─────────────
            yield return StartCoroutine(TypeText(currentLine.lineText));
            // ────────────────────────────────────────────────────

            // Evento especial: Línea 3 (index 2)
            if (i == 2)
            {
                if (imagenTitilante != null)
                    yield return StartCoroutine(TitilarImagen());

                if (panelTercero != null)
                {
                    panelTercero.SetActive(true);
                    yield return new WaitForSeconds(20f);
                    botonTercero.interactable = true;

                    while (!terceroPanelClicado)
                        yield return null;

                    panelTercero.SetActive(false);
                    terceroPanelClicado = false;
                }
            }

            // Evento especial: Línea 7 (index 6)
            if (i == 7 && panelSeptimo != null)
            {
                panelSeptimo.SetActive(true);
                botonSeptimo.interactable = true;

                while (!septimoPanelClicado)
                    yield return null;

                panelSeptimo.SetActive(false);
                septimoPanelClicado = false;
            }

            yield return new WaitForSeconds(defaultLineWaitTime);
        }

        // Oculta retrato al terminar
        if (imagenPersonaje != null) imagenPersonaje.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);
        CargarSiguienteNivel();
    }

    // ── Busca el sprite del personaje y lo asigna ────────────────
    void ActualizarRetrato(string nombreHablante)
    {
        if (imagenPersonaje == null) return;

        foreach (var entrada in retratos)
        {
            if (entrada.nombrePersonaje == nombreHablante)
            {
                imagenPersonaje.sprite = entrada.retrato;
                imagenPersonaje.gameObject.SetActive(true);
                return;
            }
        }

        // Si no encuentra el personaje, muestra retrato vacío o lo oculta
        if (retratoVacio != null)
        {
            imagenPersonaje.sprite = retratoVacio;
            imagenPersonaje.gameObject.SetActive(true);
        }
        else
        {
            imagenPersonaje.gameObject.SetActive(false);
        }
    }
    // ────────────────────────────────────────────────────────────

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
        canvasGroupTitilante.alpha = 1f;

        for (int i = 0; i < titilosCount; i++)
        {
            yield return StartCoroutine(FadeTitilo(1f, 0f, titiloDuration));
            yield return StartCoroutine(FadeTitilo(0f, 1f, titiloDuration));
        }

        yield return StartCoroutine(FadeTitilo(1f, 1f, titiloDuration));

        if (audioSourceBequín != null && audioClipTitilante != null)
        {
            audioSourceBequín.Stop();
            audioSourceBequín.clip = audioClipTitilante;
            audioSourceBequín.Play();
        }
    }

    private IEnumerator FadeTitilo(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            canvasGroupTitilante.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        canvasGroupTitilante.alpha = endAlpha;
    }

    public void OnBotonSeptimoClicked()
    {
        septimoPanelClicado = true;
        botonSeptimo.interactable = false;
    }

    public void OnBotonTerceroClicked()
    {
        terceroPanelClicado = true;
        botonTercero.interactable = false;
    }

    private void CargarSiguienteNivel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        SceneManager.LoadScene(nextIndex < SceneManager.sceneCountInBuildSettings ? nextIndex : 0);
    }
}