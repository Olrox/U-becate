using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LogicaPC : MonoBehaviour
{
    public GameObject simboloMision;
    public PlayerMovement3D jugador;
    public GameObject panel1PC;
    public GameObject panel2PC;
    public GameObject panel3PC;
    public GameObject panel1PCMision;
    public GameObject posterGuia;
    public AudioSource Bequin;

    public GameObject panelPostSi1;
    public GameObject panelPostSi2;
    public GameObject panelTextoForte;
    public GameObject panelTextoPago;

    public TextMeshProUGUI textoMision;
    public bool jugadorCerca;
    public bool aceptarMision;
    public bool diaDPago = false;
    public GameObject[] objetivos;
    public int numObjetivos;

    [Header("Panel de texto al rechazar (No)")]
    public GameObject panelTextoNo;
    public float tiempoEsperaNo = 3f;

    [Header("Configuración de pago")]
    public float tiempoEsperaPago = 3f;
    public string nombreSiguienteNivel = "Menu";

    // ─────────────────────────────────────────────
    [Header("Audios de cada panel")]
    public AudioClip audioPanel2PC;        // Diálogo inicial (¿aceptas misión?)
    public AudioClip audioPanel3PC;        // Diálogo día de pago
    public AudioClip audioPanelTextoNo;    // Respuesta al rechazar
    public AudioClip audioPanelPostSi1;    // Primer panel después de Sí
    public AudioClip audioPanelPostSi2;    // Segundo panel después de Sí
    public AudioClip audioPanelTextoForte; // Panel botón Forte
    public AudioClip audioPanelTextoPago;  // Panel aceptar pago
    // ─────────────────────────────────────────────

    void Start()
    {
        numObjetivos = objetivos.Length;
        textoMision.text = "Encuentra los requisitos de la beca" + "\n Restantes: " + numObjetivos;
        jugador = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement3D>();
        simboloMision.SetActive(true);

        panel1PC.SetActive(false);
        panel2PC.SetActive(false);
        panel3PC.SetActive(false);
        panel1PCMision.SetActive(false);
        panelPostSi1.SetActive(false);
        panelPostSi2.SetActive(false);
        panelTextoForte.SetActive(false);
        panelTextoPago.SetActive(false);
    }

    // ── Helper: activa panel y reproduce su audio ──
    private void MostrarPanel(GameObject panel, AudioClip clip)
    {
        panel.SetActive(true);
        if (clip != null && Bequin != null)
        {
            Bequin.Stop();          // Detiene cualquier audio previo
            Bequin.clip = clip;
            Bequin.Play();
        }
    }
    // ──────────────────────────────────────────────

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && jugadorCerca && aceptarMision == false)
        {
            Vector3 posicionjugador = new Vector3(transform.position.x, jugador.gameObject.transform.position.y, transform.position.z);
            jugador.gameObject.transform.LookAt(posicionjugador);

            jugador.enabled = false;
            panel1PC.SetActive(false);
            MostrarPanel(panel2PC, audioPanel2PC); // ← Cambiado
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.E) && jugadorCerca && aceptarMision == true && diaDPago == true)
        {
            Vector3 posicionjugador = new Vector3(transform.position.x, jugador.gameObject.transform.position.y, transform.position.z);
            jugador.gameObject.transform.LookAt(posicionjugador);

            jugador.enabled = false;
            panel1PCMision.SetActive(false);
            MostrarPanel(panel3PC, audioPanel3PC); // ← Cambiado
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
                panel1PC.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            jugadorCerca = false;
            if (Bequin != null) Bequin.Stop(); // ← Detiene audio al alejarse
            panel1PC.SetActive(false);
            panel2PC.SetActive(false);
            panel3PC.SetActive(false);
            panelPostSi1.SetActive(false);
            panelPostSi2.SetActive(false);
            panelTextoForte.SetActive(false);
            panelTextoPago.SetActive(false);
        }
    }

    public void No()
    {
        panel2PC.SetActive(false);
        MostrarPanel(panelTextoNo, audioPanelTextoNo); // ← Cambiado
        StartCoroutine(ProcesarRechazo());
    }

    private IEnumerator ProcesarRechazo()
    {
        yield return new WaitForSeconds(tiempoEsperaNo);

        jugador.enabled = true;
        panel1PC.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (panelTextoNo != null)
            panelTextoNo.SetActive(false);
    }

    public void Si()
    {
        panel2PC.SetActive(false);
        MostrarPanel(panelPostSi1, audioPanelPostSi1); // ← Cambiado
    }

    public void BotonSiguienteClicked()
    {
        panelPostSi1.SetActive(false);
        MostrarPanel(panelPostSi2, audioPanelPostSi2); // ← Cambiado
    }

    public void BotonIniciarBusquedaClicked()
    {
        panelPostSi2.SetActive(false);

        aceptarMision = true;
        for (int i = 0; i < objetivos.Length; i++)
            objetivos[i].SetActive(true);

        simboloMision.SetActive(false);
        posterGuia.SetActive(true);
        panel1PCMision.SetActive(true);

        jugador.enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        jugadorCerca = false;
    }

    public void BotonForteClicked()
    {
        MostrarPanel(panelTextoForte, audioPanelTextoForte); // ← Cambiado
    }

    public void BotonAceptarPagoClicked()
    {
        MostrarPanel(panelTextoPago, audioPanelTextoPago); // ← Cambiado
        StartCoroutine(CambiarNivelDespuesDeEspera());
    }

    private IEnumerator CambiarNivelDespuesDeEspera()
    {
        yield return new WaitForSeconds(tiempoEsperaPago);
        SceneManager.LoadScene(nombreSiguienteNivel);
    }
}