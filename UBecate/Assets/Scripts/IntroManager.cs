using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DialogueLine  // ← ¡Cambia struct por class!
{
    [Header("Nombre del hablante")]
    public string speakerName;
    
    [Header("Texto de la línea")]
    public string lineText;
}

public class IntroManager : MonoBehaviour
{
    [Header("Texto del diálogo")]
    public TextMeshProUGUI dialogeText;
    
    [Header("Nombre del hablante (asigna un TextMeshProUGUI separado)")]
    public TextMeshProUGUI nameText;  // ← Nuevo: Asigna este campo en el Inspector con un TextMeshProUGUI para los nombres
    
    [Header("Líneas del diálogo")]
    public DialogueLine[] dialogueLines;  // ← Reemplaza string[] lines → Ahora cada línea tiene nombre + texto
    
    public float textSpeed = 0.07f;
    private int index;

    [Header("Botones a activar al finalizar el diálogo")]
    public GameObject[] botones;

    [Header("Negación")]
    public string negacionText = "¡Has elegido la negación!...";
    public float reloadDelay = 3f;

    private string targetText;
    private bool interactionEnabled = true;

    void Start()
    {
        dialogeText.text = string.Empty;
        if (nameText != null) nameText.text = string.Empty;  // ← Limpia el nombre al inicio
        interactionEnabled = true;
        StartDialogue();
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

    public void StartDialogue()
    {
        index = 0;
        if (dialogueLines.Length > 0)
        {
            SetSpeaker(dialogueLines[0].speakerName);  // ← Muestra el nombre del primer hablante
            StartCoroutine(TypeText(dialogueLines[0].lineText));
        }
    }

    // ← Nueva función: Configura el nombre del hablante
    private void SetSpeaker(string speakerName)
    {
        if (nameText != null)
        {
            nameText.text = speakerName + ": ";  // ← Formato "Nombre: "
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

    public void NextLine()
    {
        if (index < dialogueLines.Length - 1)
        {
            index++;
            SetSpeaker(dialogueLines[index].speakerName);  // ← Cambia el nombre para la siguiente línea
            StartCoroutine(TypeText(dialogueLines[index].lineText));
        }
        else
        {
            ActivateButtons();
        }
    }

    private void ActivateButtons()
    {
        foreach (GameObject boton in botones)
        {
            if (boton != null)
                boton.SetActive(true);
        }
    }

    private void DeactivateButtons()
    {
        foreach (GameObject boton in botones)
        {
            if (boton != null)
                boton.SetActive(false);
        }
    }

    private void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Negacion()
    {
        StartCoroutine(NegacionSequence());
    }

    IEnumerator NegacionSequence()
    {
        DeactivateButtons();

        // ← Limpia el nombre para la negación (o pon "Sistema: " si quieres)
        if (nameText != null) nameText.text = string.Empty;

        // Escribe la línea de negación
        yield return StartCoroutine(TypeText(negacionText));

        interactionEnabled = false;
        yield return new WaitForSeconds(reloadDelay);
        ReloadLevel();
    }
}