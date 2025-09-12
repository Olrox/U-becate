using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogeManager : MonoBehaviour
{
    public TextMeshProUGUI dialogeText;
    public string[] lines;
    public float textSpeed = 0.7f;
    public int index;
    [Header("Botones a activar")]
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
            if(dialogeText.text == lines[index])
            {
                nexLine();
            }
            else
            {
                StopAllCoroutines();
                dialogeText.text = lines[index];

            }          
        }

        /*if (Input.GetMouseButtonUp(0)){

            if(index == 6)
            {
                SceneManager.LoadScene("JefeLVL1");
            }
        }*/

        
    }

    public void startDialoge()
    {
        index = 0;
        StartCoroutine(writeLine());
    }

    IEnumerator writeLine()
    {
        foreach (char letter in lines[index].ToCharArray())
        {
            dialogeText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }
    public void nexLine()
    {
        if (index < lines.Length - 1)
        {
            index ++;
            dialogeText.text = string.Empty;
            StartCoroutine(writeLine());
            // Activar botones
            foreach (GameObject boton in botones)
            {
                boton.SetActive(true);
            }
        }

        else
        {
            gameObject.SetActive(false);
        }
    }
}