using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;   // ← Necesario para cambiar de escena

public class LogicaPC : MonoBehaviour
{
    public GameObject simboloMision;
    public PlayerMovement3D jugador;
    public GameObject panel1PC;
    public GameObject panel2PC;
    public GameObject panel3PC;               // ← Mantenido tal cual
    public GameObject panel1PCMision;
    public GameObject posterGuia;

    // Nuevos paneles para la secuencia después de "Sí"
    public GameObject panelPostSi1;           // Imagen + texto + botón "Siguiente"
    public GameObject panelPostSi2;           // Imagen + texto + botón "Iniciar búsqueda"

    // Nuevos paneles para los botones en panel3PC
    public GameObject panelTextoForte;        // Solo texto al presionar "Forte"
    public GameObject panelTextoPago;         // Texto antes de cambiar de nivel

    public TextMeshProUGUI textoMision;
    public bool jugadorCerca;
    public bool aceptarMision;
    public bool diaDPago = false;
    public GameObject[] objetivos;
    public int numObjetivos;

    [Header("Panel de texto al rechazar (No)")]
    public GameObject panelTextoNo;         // ← Asigna aquí el nuevo panel
    public float tiempoEsperaNo = 3f;       // Segundos que se muestra el panel antes de continuar

    [Header("Configuración de pago")]
    public float tiempoEsperaPago = 3f;              // Segundos antes de cambiar de nivel
    public string nombreSiguienteNivel = "Menu";     // Cambia al nombre de tu siguiente escena

    void Start()
    {
        numObjetivos = objetivos.Length;
        textoMision.text = "Encuentra los requisitos de la beca" + "\n Restantes: " + numObjetivos;
        jugador = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement3D>();
        simboloMision.SetActive(true);

        // Ocultar todos los paneles al inicio
        panel1PC.SetActive(false);
        panel2PC.SetActive(false);
        panel3PC.SetActive(false);
        panel1PCMision.SetActive(false);
        panelPostSi1.SetActive(false);
        panelPostSi2.SetActive(false);
        panelTextoForte.SetActive(false);
        panelTextoPago.SetActive(false);
    }

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
            panel3PC.SetActive(true);  // ← Se mantiene exactamente igual
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

        // Muestra el panel de texto de rechazo
        if (panelTextoNo != null)
            panelTextoNo.SetActive(true);

        StartCoroutine(ProcesarRechazo());
    }

    private IEnumerator ProcesarRechazo()
    {
        yield return new WaitForSeconds(tiempoEsperaNo);

        jugador.enabled = true;
        panel1PC.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Cierra el panel de texto (opcional, si quieres que desaparezca automáticamente)
        if (panelTextoNo != null)
            panelTextoNo.SetActive(false);
    }

    public void Si()
    {
        panel2PC.SetActive(false);
        panelPostSi1.SetActive(true);  // ← Abre el primer panel nuevo después de "Sí"
    }

    // Botón "Siguiente" en panelPostSi1
    public void BotonSiguienteClicked()
    {
        panelPostSi1.SetActive(false);
        panelPostSi2.SetActive(true);
    }

    // Botón "Iniciar búsqueda" en panelPostSi2
    public void BotonIniciarBusquedaClicked()
    {
        panelPostSi2.SetActive(false);

        // Activar la misión (como en el código original)
        aceptarMision = true;
        for (int i = 0; i < objetivos.Length; i++)
        {
            objetivos[i].SetActive(true);
        }

        simboloMision.SetActive(false);
        posterGuia.SetActive(true);
        panel1PCMision.SetActive(true);

        jugador.enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        jugadorCerca = false;
    }

    // Botón "Forte" en panel3PC
    public void BotonForteClicked()
    {
        panelTextoForte.SetActive(true);
        // Opcional: puedes agregar un botón "Cerrar" en este panel para que el jugador lo cierre manualmente
    }

    // Botón "Aceptar pago" (o como lo llames) en panel3PC
    public void BotonAceptarPagoClicked()
    {
        panelTextoPago.SetActive(true);
        StartCoroutine(CambiarNivelDespuesDeEspera());
    }

    private IEnumerator CambiarNivelDespuesDeEspera()
    {
        yield return new WaitForSeconds(tiempoEsperaPago);
        SceneManager.LoadScene(nombreSiguienteNivel);
    }
}