using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicaMisionRequ : MonoBehaviour
{
    public LogicaPC logicaPC;
    void OnTriggerEnter (Collider col)
    {
    if (col.tag == "Player")
    {
        logicaPC.numObjetivos--;
        logicaPC.textoMision.text = "Busca los requisitos de la beca" + "\n Restantes: " + logicaPC.numObjetivos;
            if (logicaPC.numObjetivos <= 0)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                logicaPC.textoMision.text = "Completaste la búsqueda";
                logicaPC.botonMision.SetActive(true);
            }
            //animación
            transform.parent.gameObject.SetActive(false);
    }
    }
}
