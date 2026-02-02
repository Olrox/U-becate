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
    public TextMeshProUGUI speakerNameText; // Texto para el nombre del hablante
    public Button btnNextStep;             // Botón "Siguiente" o "Entendido"

    [Header("Elementos de Highlight (Animaciones mejoradas)")]
    public Image highlightCircle;          // Círculo principal (blanco con borde rojo)
    public Image highlightGlow;            // Glow exterior (amarillo/neón para brillo)
    public Image highlightArrow;           // Flecha apuntando (opcional)

    [Header("Referencias de Botones/Paneles del Juego")]
    public Button documentsButton;
    public Button infoButton;
    public Button checklistButton;
    public Button BancoInfoButton;
    public Button approveButton;
    public Button denyButton;
    public GameObject documentPanel;
    public GameObject infoPanel;
    public GameObject checklistPanel;
    public GameObject BancoInfoPanel;

    [Header("Panel de Cierre")]
    public GameObject clickToClosePanel;   // Panel invisible que detecta clics para cerrar

    [Header("Configuración de Animaciones")]
    public float highlightDuration = 3f;   // Duración total de la animación
    public AnimationCurve pulseCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);  // Curva para pulso suave
    public Color glowColor = Color.yellow; // Color del glow
    public float glowIntensity = 1.5f;     // Intensidad del brillo

    [Header("Transición")]
    public string siguienteNivel = "Lvl2";

    private int currentStep = 0;

    [System.Serializable]
    private class DialogueStep
    {
        public string nombreHablante;
        public string texto;

        public DialogueStep(string nombreHablante, string texto)
        {
            this.nombreHablante = nombreHablante;
            this.texto = texto;
        }
    }

    [System.Serializable]
    private class TutorialStep
    {
        public string nombreHablante;
        public string texto;
        public GameObject target;
        public GameObject panelToOpen;  // Panel que se abrirá al presionar el botón
        public bool esUltimoStep;       // Indica si es el último paso

        public TutorialStep(string nombreHablante, string texto, GameObject target = null, GameObject panelToOpen = null, bool esUltimoStep = false)
        {
            this.nombreHablante = nombreHablante;
            this.texto = texto;
            this.target = target;
            this.panelToOpen = panelToOpen;
            this.esUltimoStep = esUltimoStep;
        }
    }

    private DialogueStep[] dialogoInicial;
    private TutorialStep[] steps;

    void Awake()
    {
        // Diálogo inicial
        dialogoInicial = new DialogueStep[]
        {
            new DialogueStep("Goyo", "¡Bienvenido a tu primer día de trabajo!"),
            new DialogueStep("Tú", "Dónde me llevaste, espera, ¿Por qué estoy trabajando en una ventanilla? y más importante ¿Por qué estoy existiendo en 2D?"),
            new DialogueStep("Goyo", "Te acabo de preparar una simulación dibujada por mí y para que quedarás a juego con ella, te acabo de redimensionar. ¿Actualmente estás viendo la vida como yo la veo, una gran diferencia a tu cuarto, no es así?"),
            new DialogueStep("Tú","¿Estás diciendo que mi cuarto se veía aburrido y sin color? Dejame decirte que mi casa tiene un árbol completamente verde y me tardé mucho en pintarlo."),
            new DialogueStep("Goyo", "¡¿Pintaste tu el árbol?! Wow, yo que pensé que era un error de la programac… En fin ese tampoco es el caso, te explicaré cómo realizar el trabajo.")
        };

        // Pasos del tutorial
        steps = new TutorialStep[]
        {
            new TutorialStep("Goyo","Es muy importante saber que, para dar información para algún proceso, los datos deben estar actualizados y conforme lo establecido en la convocatoria.", null, null, false),
            new TutorialStep("Goyo","Los diversos personajes que dibujé presentarán sus datos en nuestro escritorio.", 
                             documentsButton?.gameObject, documentPanel, false),
            new TutorialStep("Goyo","Deberás corroborar que sus nombres estén iguales a como los tenemos en la pantalla.", 
                             infoButton?.gameObject, infoPanel, false),
            new TutorialStep("Goyo","Usa las 'anotaciones' para marcar AFIRMATIVAMENTE los puntos que cumplen correctamente con el trámite revisaste.", 
                             checklistButton?.gameObject, checklistPanel, false),
            new TutorialStep("Goyo","Además de que el estado de cuenta será en el formato que tenemos pegado del lado derecho de la ventanilla, para verlo solo necesitas picar sobre mi dibujo que tiene un banco y dinero.", 
                             BancoInfoButton?.gameObject, BancoInfoPanel, false),
            new TutorialStep("Goyo","Algunos personajes se presentarán por sus nietos o hijos, es importante que revises que traigan una carta poder para que ellos puedan seguir con el trámite.", 
                             documentsButton?.gameObject, documentPanel, false),
            new TutorialStep("Goyo","El botón VERDE es para confirmar que está todo en orden y puede continuar con el trámite.", 
                             approveButton?.gameObject, null, false),
            new TutorialStep("Goyo","El botón ROJO es para marcar que hay un error en los requisitos presentados.", 
                             denyButton?.gameObject, null, false),                                                   
            new TutorialStep("Goyo","¡Ya conoces todos los botones! ¿Estás listo? \n**Verde** → continuar al siguiente nivel \n**Rojo** → repetir las instrucciones", 
                             null, null, true),  // ← AQUÍ está el cambio: true en vez de omitir el parámetro
        };
    }

    void Start()
    {
        // Asegurar que los paneles estén cerrados al inicio
        CerrarTodosPaneles();
        if (clickToClosePanel != null) clickToClosePanel.SetActive(false);
        
        StartCoroutine(IniciarTutorial());
    }

    IEnumerator IniciarTutorial()
    {
        if (tutorialOverlay != null) tutorialOverlay.SetActive(true);
        if (tooltipPanel != null) tooltipPanel.SetActive(true);

        DeshabilitarBotonesJuego();

        // Mostrar diálogo inicial
        yield return StartCoroutine(MostrarDialogoInicial());

        // Continuar con el tutorial
        foreach (var step in steps)
        {
            yield return StartCoroutine(ShowStep(step));
        }
    }

    IEnumerator MostrarDialogoInicial()
    {
        foreach (var dialogo in dialogoInicial)
        {
            // Mostrar nombre del hablante
            if (speakerNameText != null)
            {
                speakerNameText.text = dialogo.nombreHablante;
                speakerNameText.gameObject.SetActive(true);
            }

            // Mostrar texto del diálogo
            tooltipText.text = dialogo.texto;

            // Esperar a que el jugador presione "Siguiente"
            btnNextStep.interactable = true;
            yield return new WaitUntil(() => !btnNextStep.interactable);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator ShowStep(TutorialStep step)
    {
        // Mostrar nombre del hablante
        if (speakerNameText != null)
        {
            speakerNameText.text = step.nombreHablante;
            speakerNameText.gameObject.SetActive(true);
        }

        tooltipText.text = step.texto;

        if (step.target != null)
        {
            // Solo habilitar el botón si NO es aprobar ni rechazar
            Button targetButton = step.target.GetComponent<Button>();
            if (targetButton != null && targetButton != approveButton && targetButton != denyButton)
            {
                targetButton.interactable = true;
                
                // Si hay un panel asociado, configurar el evento para abrirlo
                if (step.panelToOpen != null)
                {
                    targetButton.onClick.RemoveAllListeners();
                    targetButton.onClick.AddListener(() => AbrirPanelConCierre(step.panelToOpen));
                }
            }

            yield return StartCoroutine(HighlightTargetAnimado(step.target.gameObject));
        }

        // Si es el último step, habilitar botones de aprobar/rechazar
        if (step.esUltimoStep)
        {
            HabilitarBotonesFinales();
        }

        btnNextStep.interactable = true;
        yield return new WaitUntil(() => !btnNextStep.interactable);
        yield return new WaitForSeconds(0.5f);
    }

    // Abrir panel y activar el panel de cierre
    void AbrirPanelConCierre(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true);
            
            // Activar el panel de cierre
            if (clickToClosePanel != null)
            {
                clickToClosePanel.SetActive(true);
                Button closeButton = clickToClosePanel.GetComponent<Button>();
                if (closeButton != null)
                {
                    closeButton.onClick.RemoveAllListeners();
                    closeButton.onClick.AddListener(() => CerrarPanelActual(panel));
                }
            }
        }
    }

    // Cerrar el panel actual
    void CerrarPanelActual(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
        
        if (clickToClosePanel != null)
        {
            clickToClosePanel.SetActive(false);
        }
    }

    // Cerrar todos los paneles informativos
    void CerrarTodosPaneles()
    {
        if (documentPanel != null) documentPanel.SetActive(false);
        if (infoPanel != null) infoPanel.SetActive(false);
        if (checklistPanel != null) checklistPanel.SetActive(false);
        if (BancoInfoPanel != null) BancoInfoPanel.SetActive(false);
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

    // ── Resto de métodos ─────────────────────────────────────────
    void DeshabilitarBotonesJuego()
    {
        documentsButton.interactable = false;
        infoButton.interactable = false;
        checklistButton.interactable = false;
        BancoInfoButton.interactable = false;
        approveButton.interactable = false;
        denyButton.interactable = false;
    }

    void HabilitarBotonesFinales()
    {
        approveButton.interactable = true;
        denyButton.interactable = true;
        
        // Configurar eventos de los botones finales
        approveButton.onClick.RemoveAllListeners();
        approveButton.onClick.AddListener(IrASiguienteNivel);
        
        denyButton.onClick.RemoveAllListeners();
        denyButton.onClick.AddListener(RepetirTutorial);
    }

    void RepetirTutorial()
    {
        // Reiniciar la escena actual para repetir el tutorial
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void IrASiguienteNivel()
    {
        if (tutorialOverlay != null) tutorialOverlay.SetActive(false);
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
        SceneManager.LoadScene(siguienteNivel);
    }
}