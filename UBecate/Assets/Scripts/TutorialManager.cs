using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [Header("Paneles del Tutorial")]
    public GameObject tutorialOverlay;     // Panel semi-transparente que cubre todo
    public GameObject tooltipPanel;        // Panel flotante con texto + flecha/highlight
    public TextMeshProUGUI tooltipText;    // Texto del tooltip
    public Button btnNextStep;             // Botón "Siguiente" o "Entendido"

    [Header("Elementos de Highlight (Animaciones mejoradas)")]
    public Image highlightCircle;          // Círculo principal (blanco con borde rojo)
    public Image highlightGlow;            // Glow exterior (amarillo/neón para brillo)
    public Image highlightArrow;           // Flecha apuntando (opcional)

    [Header("Referencias de Botones/Paneles del Juego")]
    public Button documentsButton;
    public Button infoButton;
    public Button checklistButton;
    public Button approveButton;
    public Button denyButton;
    public GameObject documentPanel;
    public GameObject infoPanel;
    public GameObject checklistPanel;

    [Header("Configuración de Animaciones")]
    public float highlightDuration = 3f;   // Duración total de la animación
    public AnimationCurve pulseCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);  // Curva para pulso suave
    public Color glowColor = Color.yellow; // Color del glow
    public float glowIntensity = 1.5f;     // Intensidad del brillo

    [Header("NPC de Ejemplo")]
    public GameManager gameManager;
    public bool useSimulatedNPC = true;

    [Header("Transición")]
    public string siguienteNivel = "Lvl2";

    private int currentStep = 0;

    [System.Serializable]
private class TutorialStep
{
    public string texto;
    public GameObject target;   // ← tipo correcto

    public TutorialStep(string texto, GameObject target = null)
    {
        this.texto = texto;
        this.target = target;
    }
}

private TutorialStep[] steps;

void Awake()
{
    steps = new TutorialStep[]
    {
        new TutorialStep("Bienvenido al tutorial. Aquí revisarás documentos de un NPC de ejemplo.", null),
        new TutorialStep("Haz clic en 'Documentos' para ver los papeles del NPC.", 
                         documentsButton?.gameObject),   // ← .gameObject aquí
        new TutorialStep("Abre 'Info' para comparar con los datos oficiales.", 
                         infoButton?.gameObject),
        new TutorialStep("Usa 'Checklist' para marcar lo que revisaste (Sí/No).", 
                         checklistButton?.gameObject),
        new TutorialStep("¡Este NPC está perfecto! Haz clic en 'Aprobar' para dejarlo pasar.", 
                         approveButton?.gameObject),
    };
}

    void Start()
    {
        StartCoroutine(IniciarTutorial());
    }

    IEnumerator IniciarTutorial()
    {
        if (tutorialOverlay != null) tutorialOverlay.SetActive(true);
        if (tooltipPanel != null) tooltipPanel.SetActive(true);

        DeshabilitarBotonesJuego();

        if (useSimulatedNPC && gameManager != null)
        {
            yield return new WaitForSeconds(1f);
            gameManager.StartCoroutine(gameManager.ProcessNextNPC());
        }

        foreach (var step in steps)
        {
            yield return StartCoroutine(ShowStep(step));
        }

        HabilitarBotonesFinales();
        yield return StartCoroutine(EsperarAprobacionCorrecta());

        IrASiguienteNivel();
    }

    IEnumerator ShowStep(TutorialStep step)
    {
        tooltipText.text = step.texto;

        if (step.target != null)
        {
            yield return StartCoroutine(HighlightTargetAnimado(step.target.gameObject));
        }

        btnNextStep.interactable = true;
        yield return new WaitUntil(() => !btnNextStep.interactable);
        btnNextStep.interactable = false;
        yield return new WaitForSeconds(0.5f);
    }

    // ── ANIMACIÓN DE HIGHLIGHT MEJORADA ──────────────────────────────────────
    IEnumerator HighlightTargetAnimado(GameObject target)
    {
        RectTransform targetRect = target.GetComponent<RectTransform>();
        RectTransform highlightRect = highlightCircle.GetComponent<RectTransform>();
        RectTransform glowRect = highlightGlow?.GetComponent<RectTransform>();
        RectTransform arrowRect = highlightArrow?.GetComponent<RectTransform>();

        // Posicionar highlights
        highlightRect.position = targetRect.position;
        if (glowRect != null) glowRect.position = targetRect.position;
        if (arrowRect != null) arrowRect.position = targetRect.position + Vector3.up * 50f;  // Flecha arriba

        // Activar
        highlightCircle.gameObject.SetActive(true);
        if (highlightGlow != null) highlightGlow.gameObject.SetActive(true);
        if (highlightArrow != null) highlightArrow.gameObject.SetActive(true);

        float time = 0f;

        while (time < highlightDuration)
        {
            time += Time.deltaTime;
            float normalizedTime = time / highlightDuration;

            // 1. PULSO DE ESCALA (suave con curva)
            float pulseScale = 1f + pulseCurve.Evaluate(normalizedTime) * 0.3f;
            highlightRect.localScale = Vector3.one * pulseScale;

            // 2. BRILLO/GLOW (intensidad variable)
            if (highlightGlow != null)
            {
                Color glowCol = glowColor;
                glowCol.a = Mathf.Sin(time * 6f) * 0.5f + 0.5f;  // Parpadeo
                highlightGlow.color = glowCol;
                glowRect.localScale = Vector3.one * (1f + glowIntensity * (1f - normalizedTime));
            }

            // 3. REBOTE DE FLECHA (opcional)
            if (highlightArrow != null)
            {
                float bounce = Mathf.Sin(time * 8f) * 0.2f;
                arrowRect.localPosition = new Vector3(0, 50f + bounce * 20f, 0);
            }

            yield return null;
        }

        // Desactivar
        highlightCircle.gameObject.SetActive(false);
        if (highlightGlow != null) highlightGlow.gameObject.SetActive(false);
        if (highlightArrow != null) highlightArrow.gameObject.SetActive(false);
    }

    public void NextTutorialStep()
    {
        btnNextStep.interactable = false;
    }

    // ── Resto de métodos sin cambios ─────────────────────────────────────────
    void DeshabilitarBotonesJuego()
    {
        documentsButton.interactable = false;
        infoButton.interactable = false;
        checklistButton.interactable = false;
        approveButton.interactable = false;
        denyButton.interactable = false;
    }

    void HabilitarBotonesFinales()
    {
        approveButton.interactable = true;
        denyButton.interactable = true;
    }

    public bool TutorialCompletado { get; private set; } = false;

    public void MarcarTutorialCompletado()
    {
        TutorialCompletado = true;
    }

    IEnumerator EsperarAprobacionCorrecta()
    {
        yield return new WaitUntil(() => TutorialCompletado);
        if (tutorialOverlay != null) tutorialOverlay.SetActive(false);
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    void IrASiguienteNivel()
    {
        SceneManager.LoadScene(siguienteNivel);
    }
}