using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DialogueLine
{
    [Header("Nombre del hablante")]
    public string speakerName;
    
    [Header("Texto de la línea")]
    [TextArea(3,6)]
    public string lineText;
}

public class IntroManager : MonoBehaviour
{
    [Header("Panel Intermedio (imagen 10 segundos)")]
    public GameObject panelIntermedio;       // ← Arrastra aquí el panel con la imagen
    public float tiempoIntermedio = 10f;     // Duración antes de pasar al diálogo

    [Header("Texto del diálogo")]
    public TextMeshProUGUI dialogeText;
    
    [Header("Nombre del hablante")]
    public TextMeshProUGUI nameText;
    
    [Header("Líneas del diálogo")]
    public DialogueLine[] dialogueLines;
    
    public float textSpeed = 0.07f;

    [Header("Botones de decisión (se activan al final del diálogo)")]
    public GameObject[] botonesDecision;

    [Header("Negación")]
    public string negacionText = "¡Has elegido la negación!...";
    public float reloadDelay = 3f;

    private string targetText;
    private bool interactionEnabled = true;
    private int index;

    void Start()
    {
        // Forzar estado limpio cada vez que se carga la escena
        dialogeText.text = string.Empty;
        if (nameText != null) nameText.text = string.Empty;

        interactionEnabled = true;

        // Asegurarse de que el panel intermedio esté desactivado al cargar
        if (panelIntermedio != null)
        {
            panelIntermedio.SetActive(false);
        }

        // Ocultar botones de decisión al inicio (siempre)
        SetDecisionButtonsActive(false);

        // Iniciar flujo completo
        StartCoroutine(FlujoCompleto());
    }

    IEnumerator FlujoCompleto()
    {
        // 1. Mostrar panel intermedio (imagen durante X segundos)
        if (panelIntermedio != null)
        {
            panelIntermedio.SetActive(true);
            yield return new WaitForSeconds(tiempoIntermedio);
            panelIntermedio.SetActive(false);
        }

        // 2. Iniciar diálogo inmediatamente después
        StartDialogue();
    }

    private void StartDialogue()
    {
        index = 0;
        if (dialogueLines.Length > 0)
        {
            SetSpeaker(dialogueLines[0].speakerName);
            StartCoroutine(TypeText(dialogueLines[0].lineText));
        }
        else
        {
            Debug.LogWarning("No hay líneas de diálogo asignadas → activando botones directamente");
            SetDecisionButtonsActive(true);
        }
    }

    private void SetSpeaker(string speakerName)
    {
        if (nameText != null)
        {
            nameText.text = speakerName + ": ";
        }
    }

    IEnumerator TypeText(string text)
    {
        targetText = text;
        dialogeText.text = string.Empty;

        foreach (char letter in text.ToCharArray())
        {
            dialogeText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void Update()
    {
        if (!interactionEnabled) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (dialogeText.text == targetText)
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                dialogeText.text = targetText;
            }
        }
    }

    public void NextLine()
    {
        if (index < dialogueLines.Length - 1)
        {
            index++;
            SetSpeaker(dialogueLines[index].speakerName);
            StartCoroutine(TypeText(dialogueLines[index].lineText));
        }
        else
        {
            // Diálogo terminado → activar botones de decisión
            SetDecisionButtonsActive(true);
            Debug.Log("Diálogo terminado → botones de decisión activados");
        }
    }

    private void SetDecisionButtonsActive(bool active)
    {
        foreach (GameObject boton in botonesDecision)
        {
            if (boton != null)
            {
                boton.SetActive(active);
            }
        }
    }

    public void Negacion()
    {
        StartCoroutine(NegacionSequence());
    }

    IEnumerator NegacionSequence()
    {
        // 1. Bloquear interacción completa
        interactionEnabled = false;

        // 2. Desactivar botones de decisión inmediatamente
        SetDecisionButtonsActive(false);

        // 3. Limpiar nombre (opcional)
        if (nameText != null) nameText.text = "";

        // 4. Mostrar mensaje de negación
        yield return StartCoroutine(TypeText(negacionText));

        // 5. Esperar y recargar escena
        yield return new WaitForSeconds(reloadDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}