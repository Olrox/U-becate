using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreditosManager : MonoBehaviour
{
    [Header("Créditos - Configuración")]
    [SerializeField] private TextMeshProUGUI textoCreditos;
    [SerializeField] private Button botonAcelerarCreditos;
    [TextArea(2, 4)]
    [SerializeField] private string[] lineasCreditos = new string[5];

    [Header("Créditos - Animación")]
    [SerializeField] private float velocidadNormal = 100f;
    [SerializeField] private float velocidadRapida = 300f;
    [SerializeField] private float tiempoEntreLineas = 0.5f;

    private bool acelerarCreditos = false;
    private RectTransform rectTextoCreditos;
    private Menu menuManager;

    void Start()
    {
        menuManager = GetComponent<Menu>();
        
        if (textoCreditos != null)
            rectTextoCreditos = textoCreditos.GetComponent<RectTransform>();

        if (botonAcelerarCreditos != null)
        {
            botonAcelerarCreditos.onClick.AddListener(AcelerarCreditos);
        }
    }

    public IEnumerator SecuenciaCreditos()
    {
        acelerarCreditos = false;

        if (botonAcelerarCreditos != null)
            botonAcelerarCreditos.gameObject.SetActive(true);

        for (int i = 0; i < lineasCreditos.Length && i < 6; i++)
        {
            yield return StartCoroutine(MostrarLineaCredito(lineasCreditos[i]));
            
            if (i < lineasCreditos.Length - 1)
            {
                yield return new WaitForSeconds(tiempoEntreLineas);
            }
        }

        if (botonAcelerarCreditos != null)
            botonAcelerarCreditos.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        if (menuManager != null)
        {
            menuManager.VolverAlMenu();
        }
    }

    IEnumerator MostrarLineaCredito(string textoLinea)
    {
        if (textoCreditos == null || rectTextoCreditos == null)
            yield break;

        textoCreditos.text = textoLinea;

        Canvas canvas = textoCreditos.GetComponentInParent<Canvas>();
        if (canvas == null) yield break;
        
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float alturaCanvas = canvasRect.rect.height;

        rectTextoCreditos.anchoredPosition = new Vector2(0, -alturaCanvas / 2 - 100);
        float posicionFinalY = alturaCanvas / 2 + 100;

        while (rectTextoCreditos.anchoredPosition.y < posicionFinalY)
        {
            float velocidadActual = acelerarCreditos ? velocidadRapida : velocidadNormal;
            rectTextoCreditos.anchoredPosition += new Vector2(0, velocidadActual * Time.deltaTime);
            yield return null;
        }
    }

    void AcelerarCreditos()
    {
        acelerarCreditos = true;
        
        TextMeshProUGUI textoBoton = botonAcelerarCreditos.GetComponentInChildren<TextMeshProUGUI>();
        if (textoBoton != null)
        {
            textoBoton.text = "Acelerado >>>";
        }
    }
}