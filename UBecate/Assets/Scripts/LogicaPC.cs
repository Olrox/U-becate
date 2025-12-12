using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogicaPC : MonoBehaviour
{
    public GameObject simboloMision;
    public PlayerMovement3D jugador;
    public GameObject panel1PC;
    public GameObject panel2PC;
    public GameObject panel3PC;
    public GameObject panel1PCMision;
    public TextMeshProUGUI textoMision;
    public bool jugadorCerca;
    public bool aceptarMision;
    public bool diaDPago = false;
    public GameObject[] objetivos;
    public int numObjetivos;
    public GameObject botonMision;
    public GameObject posterGuia;
    // Start is called before the first frame update
    void Start()
    {
        numObjetivos = objetivos.Length;
        textoMision.text = "Encuentra los requisitos de la beca" + "\n Restantes: " + numObjetivos;
        jugador = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement3D>();
        simboloMision.SetActive(true);
        panel1PC.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && jugadorCerca && aceptarMision == false)
        {
            Vector3 posicionjugador = new Vector3(transform.position.x, jugador.gameObject.transform.position.y, transform.position.z);
            jugador.gameObject.transform.LookAt(posicionjugador);

            jugador.enabled = false;
            panel1PC.SetActive(false);
            panel2PC.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.E) && jugadorCerca && aceptarMision == true && diaDPago == true)
        {
            Vector3 posicionjugador = new Vector3(transform.position.x, jugador.gameObject.transform.position.y, transform.position.z);
            jugador.gameObject.transform.LookAt(posicionjugador);

            jugador.enabled = false;
            panel1PCMision.SetActive(false);
            panel3PC.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            jugadorCerca = true;
            if (aceptarMision == false)
            {
                panel1PC.SetActive(true);
                
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            jugadorCerca = false;
            panel1PC.SetActive(false);
            panel2PC.SetActive(false);
        }
    }

    public void No()
    {
        jugador.enabled = true;
        panel2PC.SetActive(false);
        panel1PC.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Si()
    {
        jugador.enabled = true;
        aceptarMision = true;
        for (int i = 0; i < objetivos.Length; i++)
        {
            objetivos[i].SetActive(true);
        }

        jugadorCerca = false;
        simboloMision.SetActive(false);
        panel1PC.SetActive(false);
        panel2PC.SetActive(false);
        panel1PCMision.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        posterGuia.SetActive(true);
    }
}
