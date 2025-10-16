using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogicaMisionRequ : MonoBehaviour
{
    public int numObjetivos;
    public TextMeshProUGUI textoMision;

    void Start()
    {
        numObjetivos = GameObject.FindGameObjectsWithTag("Objetivo").Length;
        textoMision.text = "Busca los requisitos de la beca" + "\n Restantes: " + numObjetivos;
    }


    void Update()
    {
        
    }

    void OnTriggerEnter (Collider col)
    {
    if (col.gameObject.tag == "Objetivo")
    {
        Destroy(col.transform.parent.gameObject);
        numObjetivos--;
        textoMision.text = "Busca los requisitos de la beca" + "\n Restantes: " + numObjetivos;
        if (numObjetivos <= 0)
        {
            textoMision.text = "Completaste la búsqueda";

        }
    }
    }
}
