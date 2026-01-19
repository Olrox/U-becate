using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class NPCData
{
    [Header("Información Básica del NPC")]
    public string npcName = "Nombre Real";
    public Sprite photo;                    // Foto real (la que aparece en el panel de información)
    public string idNumber;         // Número de ID
    public string city;             // Ciudad
    public string keyCode;          // Clave

    [Header("Documentos Presentados")]
    public Sprite presentedPhoto;           // Foto que presenta en la identificación
    public string presentedName = "Nombre Presentado";

    [Header("Nacionalidad")]
    public Sprite nationalityImage;         // Imagen oficial de nacionalidad
    public Sprite presentedNationality;     // Imagen que entrega (puede ser falsa)

    [Header("Carta del Consejo")]
    public bool hasCouncilLetter;           // ¿Tiene carta que autoriza pasar aunque no se parezca?
    public Sprite councilLetterImage;       // Imagen de la carta (si la tiene)

    [Header("Errores / Inconsistencias (para el checklist)")]
    public bool photoIsCorrect;         // true = la foto realmente coincide
    public bool nameIsCorrect;          // true = el nombre es correcto
    public bool nationalityIsValid;     // true = la nacionalidad es la oficial/válida
    public bool councilLetterIsPresent; // true = tiene carta del consejo cuando la necesita
    public GameObject npcPrefab;
}

public class GameManager : MonoBehaviour
{
    [Header("--- Configuración del Juego ---")]
    [Tooltip("Lista de los 6 NPCs que pasarán por la ventanilla")]
    public List<NPCData> npcs = new List<NPCData>();

    [Header("--- Referencias UI ---")]
    [Space(10)]
    [Header("Paneles")]
    public GameObject documentPanel;
    public GameObject infoPanel;
    public GameObject checklistPanel;
    public GameObject victoryPanel;
    public GameObject gameOverPanel;
    public GameObject InfoBancoPanel;
    public Sprite edoCuenta;
    public GameObject DesplegablePanel;

    [Header("Botones Principales")]
    public Button documentsButton;
    public Button infoButton;
    public Button checklistButton;
    public Button InfoBancoPanelButton;
    public Button approveButton;
    public Button denyButton;

    [Header("Botones de Game Over")]
    public Button restartButton;
    public Button mainMenuButton;

    [Header("--- Elementos de Documentos ---")]
    [Space(5)]
    public Image docPhoto;
    public TextMeshProUGUI docNameText;
    public TextMeshProUGUI docIdNumberText;
    public TextMeshProUGUI docCityText;
    public TextMeshProUGUI docKeyCodeText;
    public Image docNationality;
    public Image docLetter;

    [Header("--- Panel de Información ---")]
    public Image infoPhoto;
    public TextMeshProUGUI infoNameText;
    public TextMeshProUGUI infoIdNumberText;
    public TextMeshProUGUI infoCityText;
    public TextMeshProUGUI infoKeyCodeText;
    public Image infoNationality;

    [Header("--- Checklist (Toggles) ---")]
    public Toggle photoToggle;
    public Toggle nameToggle;
    public Toggle nationalityToggle;
    public Toggle letterToggle;
    public bool checklistSubmitted = false;

    [Header("--- Feedback y Sonidos ---")]
    public GameObject feedbackPanel;           // Panel temporal para mostrar feedback (con TextMeshProUGUI)
    public TextMeshProUGUI feedbackText;       // Texto del feedback (ej: "3/4 correctas! +25 pts")
    public AudioSource audioSource;            // AudioSource (agrega uno al GameManager)
    public AudioClip correctSound;             // Sonido para aciertos (arrastra .mp3/wav)
    public AudioClip wrongSound;               // Sonido para errores (arrastra .mp3/wav)
    public AudioClip submitSound;              // Opcional: sonido general de submit
    public AudioClip checkSound;

    [Header("Feedback simple al salir cada NPC")]
    public GameObject dialogePanel;           // Panel que contiene el texto (desactívalo al inicio)
    public TextMeshProUGUI dialogeText;       // El TextMeshPro dentro del panel
    public string[] npcFeedbackMessages;       // Arreglo con un mensaje por cada NPC (6 mensajes)

    [Header("--- Puntuación ---")]
    public TextMeshProUGUI scoreText;

    // Variables de control
    private int currentIndex = 0;
    private NPCData currentNPC;
    private GameObject currentNPCInstance;
    private int score = 0;

    void Start()
    {
        Debug.Log("GameManager Start - NPCs en lista: " + npcs.Count);
        // Ocultar todos los paneles al inicio
        documentPanel.SetActive(false);
        infoPanel.SetActive(false);
        checklistPanel.SetActive(false);
        victoryPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        DesplegablePanel.SetActive(false);

        if (feedbackPanel != null) feedbackPanel.SetActive(false);

        DisableInteractionButtons();

        // Iniciar el primer NPC
        StartCoroutine(ProcessNextNPC());
    }
internal IEnumerator ProcessNextNPC()
{
    Debug.Log($"Procesando NPC #{currentIndex}");

    if (currentIndex >= npcs.Count)
    {
        Debug.Log("¡Todos los NPCs terminados! → Victoria");
        victoryPanel.SetActive(true);
        yield break;
    }

    currentNPC = npcs[currentIndex];
    Debug.Log($"Instanciando NPC: {currentNPC.npcName}");

    currentNPCInstance = Instantiate(currentNPC.npcPrefab, Vector3.zero, Quaternion.identity);
    Debug.Log("NPC instanciado con éxito");

    Animator anim = currentNPCInstance.GetComponent<Animator>();
    if (anim != null)
    {
        Debug.Log("Animator encontrado → Reproduciendo Approach");
        anim.Play("Acercandose");
    }
    else
    {
        Debug.LogError("¡El prefab NO tiene componente Animator!");
    }

    yield return new WaitForSeconds(2f);

    Debug.Log("Habilitando botón de documentos");
    if (documentsButton != null)
        documentsButton.interactable = true;
    else
        Debug.LogError("documentsButton NO está asignado en el Inspector!");
        checklistSubmitted = false;  // Resetear para nuevo NPC
}

    // ── Botones de interacción ────────────────────────────────────────────────

    public void OpenDocuments()
    {
        documentPanel.SetActive(true);
        DesplegablePanel.SetActive(true);

        docPhoto.sprite = currentNPC.presentedPhoto;
        docNameText.text = currentNPC.presentedName;
        docNationality.sprite = edoCuenta;

        docIdNumberText.text = currentNPC.idNumber;     // Asume TextMeshProUGUI docIdNumberText
        docCityText.text = currentNPC.city;             // Asume TextMeshProUGUI docCityText
        docKeyCodeText.text = currentNPC.keyCode;       // Asume TextMeshProUGUI docKeyCodeText

        docLetter.gameObject.SetActive(currentNPC.hasCouncilLetter);
        if (currentNPC.hasCouncilLetter)
            docLetter.sprite = currentNPC.councilLetterImage;

        // Habilitar el resto de botones una vez vistos los documentos
        infoButton.interactable = true;
        checklistButton.interactable = true;
    }

    public void ToggleInfoPanel()
    {
        bool isActive = !infoPanel.activeSelf;
        infoPanel.SetActive(isActive);
        DesplegablePanel.SetActive(true);
        if (isActive)
        {
            infoNationality.sprite = currentNPC.presentedNationality;
            infoNameText.text = currentNPC.npcName;
            infoIdNumberText.text = currentNPC.idNumber;     // Asume TextMeshProUGUI docIdNumberText
            infoCityText.text = currentNPC.city;             // Asume TextMeshProUGUI docCityText
            infoKeyCodeText.text = currentNPC.keyCode;       // Asume TextMeshProUGUI docKeyCodeText
        }
    } 

    public void BancoInfoPanel()
    {
       InfoBancoPanel.SetActive(!InfoBancoPanel.activeSelf);
       DesplegablePanel.SetActive(true);
    }

    public void ToggleChecklistPanel()
    {
        bool isActive = !checklistPanel.activeSelf;
        checklistPanel.SetActive(isActive);

        if (isActive)
        {
            photoToggle.isOn = false;
            nameToggle.isOn = false;
            nationalityToggle.isOn = false;
            letterToggle.isOn = false;
        }
        approveButton.interactable = true;
        denyButton.interactable = true;
    }

    public void SubmitChecklist()
    {
        if (checklistSubmitted)
        {
            checklistPanel.SetActive(false);
            return;
        }

    // Reproducir sonido de submit (opcional)
        if (submitSound != null && audioSource != null)
            audioSource.PlayOneShot(submitSound);

        int correctAnswers = 0;
        int wrongAnswers = 0;

    // Evaluar cada toggle
        if (photoToggle.isOn == currentNPC.photoIsCorrect) correctAnswers++; else wrongAnswers++;
        if (nameToggle.isOn == currentNPC.nameIsCorrect) correctAnswers++; else wrongAnswers++;
        if (nationalityToggle.isOn == currentNPC.nationalityIsValid) correctAnswers++; else wrongAnswers++;
        if (letterToggle.isOn == currentNPC.councilLetterIsPresent) correctAnswers++; else wrongAnswers++;

    // CÁLCULO DE PUNTOS: +10 por correcto, -5 por error
        int pointsGained = correctAnswers * 10;
        int pointsLost = wrongAnswers * 5;
        int totalChange = pointsGained - pointsLost;

        score += totalChange;

    // MENSAJE DE FEEDBACK
        string feedbackMsg = $"{correctAnswers}/4 correctas!\n{pointsGained} pts ganados - {pointsLost} pts perdidos\nTotal: {(totalChange >= 0 ? "+" : "")}{totalChange} pts";

    // Mostrar feedback con sonido
        StartCoroutine(ShowFeedbackCoroutine(feedbackMsg, correctAnswers > wrongAnswers ? correctSound : wrongSound));

        checklistSubmitted = true;
        UpdateScore();
        checklistPanel.SetActive(false);
    }

// 3. CORUTINA PARA MOSTRAR FEEDBACK CON ANIMACIÓN (fade in/out + sonido)

private IEnumerator ShowFeedbackCoroutine(string message, AudioClip soundClip)
{
    // Configurar texto
    feedbackText.text = message;
    feedbackPanel.SetActive(true);

    // Reproducir sonido
    if (soundClip != null && audioSource != null)
        audioSource.PlayOneShot(soundClip);

    // Fade in (opcional, usando CanvasGroup para alpha)
    CanvasGroup cg = feedbackPanel.GetComponent<CanvasGroup>();
    if (cg == null) cg = feedbackPanel.AddComponent<CanvasGroup>();

    float fadeDuration = 0.3f;
    cg.alpha = 0f;
    float timer = 0f;

    // Fade in
    while (timer < fadeDuration)
    {
        timer += Time.deltaTime;
        cg.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
        yield return null;
    }

    // Mantener visible 2 segundos
    yield return new WaitForSeconds(2f);

    // Fade out
    timer = 0f;
    while (timer < fadeDuration)
    {
        timer += Time.deltaTime;
        cg.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
        yield return null;
    }

    feedbackPanel.SetActive(false);
    }

    public void Approve() => HandleDecision(true);
    public void Deny()    => HandleDecision(false);

    private void HandleDecision(bool approved)
    {
        // Determinar si el NPC REALMENTE debería pasar (según la verdad)
        bool shouldPass = 
        currentNPC.nameIsCorrect &&                  // Nombre correcto
        currentNPC.nationalityIsValid &&             // Nacionalidad válida
        (currentNPC.photoIsCorrect || currentNPC.councilLetterIsPresent);  // Foto OK o tiene carta

        // ¿La decisión del jugador fue la correcta?
        bool correctDecision = (approved && shouldPass) || (!approved && !shouldPass);

        if (correctDecision)
        {
            score += 50;           // Bono por decisión correcta
            UpdateScore();
            StartCoroutine(LeaveNPC(approved));
        }
        else
        {
            GameOver();
        }
    }

    IEnumerator LeaveNPC(bool accepted)
{
    Debug.Log($"[DEBUG] Iniciando salida del NPC #{currentIndex}");

    DisableInteractionButtons();

    if (dialogePanel == null)
    {
        Debug.LogError("dialogePanel NO está asignado en el Inspector!");
    }
    else if (npcFeedbackMessages == null || npcFeedbackMessages.Length <= currentIndex)
    {
        Debug.LogError($"No hay mensaje para NPC #{currentIndex}. Longitud arreglo: {npcFeedbackMessages?.Length ?? 0}");
    }
    else
    {
        Debug.Log($"Mostrando feedback: {npcFeedbackMessages[currentIndex]}");
        dialogeText.text = npcFeedbackMessages[currentIndex];
        dialogePanel.SetActive(true);
        Debug.Log("dialogePanel activado → debería verse ahora");
    }

    // 2. Reproducir animación de salida del NPC
    Animator anim = currentNPCInstance?.GetComponent<Animator>();
    if (anim != null)
    {
        string animName = accepted ? "Happy" : "Sed";
        anim.Play(animName);
    }

    // 3. Esperar a que termine la animación (ajusta el tiempo según tu animación)
    yield return new WaitForSeconds(2f);  // ← Cambia este valor al tiempo real de tus animaciones

    // 4. Destruir el NPC
    if (currentNPCInstance != null)
        Destroy(currentNPCInstance);

    // 5. Desactivar el panel de diálogo (ya terminó la salida)
    if (dialogePanel != null)
        dialogePanel.SetActive(false);

    // 6. Continuar con el siguiente NPC
    currentIndex++;
    StartCoroutine(ProcessNextNPC());
}

    IEnumerator AutoCloseFeedback(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);
    
        currentIndex++;
        StartCoroutine(ProcessNextNPC());
    }

    void GameOver()
    {
        gameOverPanel.SetActive(true);
        DisableInteractionButtons();
    }

    public void Restart()
    {
        currentIndex = 0;
        score = 0;
        UpdateScore();
        gameOverPanel.SetActive(false);
        StartCoroutine(ProcessNextNPC());
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lvl2"); //
    }

    public void MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"); //
    }

    private void DisableInteractionButtons()
    {
        documentsButton.interactable = false;
        infoButton.interactable = false;
        checklistButton.interactable = false;
        approveButton.interactable = false;
        denyButton.interactable = false;
    }

    private void UpdateScore()
    {
        if (scoreText != null)
            scoreText.text = $"Puntos: {score}";
    }
}