using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogeManager : MonoBehaviour
{
    public TextMeshProUGUI dialogeText;
    public string[] lines;
    public float textSpeed = 0.07f; // ← Corregí el valor típico (0.7f era muy lento)
    private int index;

    [Header("Botones a activar al finalizar el diálogo")]
    public GameObject[] botones;

    void Start()
    {
        dialogeText.text = string.Empty;
        startDialoge();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (dialogeText.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                dialogeText.text = lines[index];
            }
        }
    }

    public void startDialoge()
    {
        index = 0;
        StartCoroutine(WriteLine());
    }

    IEnumerator WriteLine()
    {
        foreach (char letter in lines[index].ToCharArray())
        {
            dialogeText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    public void NextLine()
    {
        if (index < lines.Length - 1)
        {
            // Aún hay más líneas → avanzamos normalmente
            index++;
            dialogeText.text = string.Empty;
            StartCoroutine(WriteLine());
        }
        else
        {
            // ¡Esta es la última línea! → activamos los botones
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
}

        /*else
        {
            gameObject.SetActive(false);
        }*/