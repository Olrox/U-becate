using UnityEngine;
using System.Collections;

public class NPCSpawner : MonoBehaviour
{
    public GameObject NPCPrefab; // Prefab del NPC
    public Transform SpawnPoint; // Punto de spawn en el centro de la pantalla
    public UIManager uiManager; // Referencia al UIManager

    private GameObject currentNPC;
    private Animator npcAnimator;

    void Start()
    {
        // Suscribirse al evento de decisión de la UI
        uiManager.OnNPCDecision += HandleNPCDecision;

        // Spawnear el primer NPC
        SpawnNPC();
    }

    private void SpawnNPC()
    {
        // Instanciar NPC en el spawn point
        currentNPC = Instantiate(NPCPrefab, SpawnPoint.position, Quaternion.identity);
        npcAnimator = currentNPC.GetComponent<Animator>();

        // Jugar animación de Acercandose
        npcAnimator.Play("Acercandose");

        // Esperar a que termine la animación de Acercandose (ajusta el tiempo según tu animación)
        StartCoroutine(WaitForAcercandoseEnd(2f)); // Ejemplo: 2 segundos
    }

    private IEnumerator WaitForAcercandoseEnd(float duration)
    {
        yield return new WaitForSeconds(duration);

        // Habilitar el botón de papeles en la UI
        uiManager.EnablePapersButton();
    }

    private void HandleNPCDecision(bool accepted)
    {
        if (accepted)
        {
            npcAnimator.Play("Happy"); // Animación para aceptar (derecha)
        }
        else
        {
            npcAnimator.Play("Sed"); // Animación para rechazar (izquierda)
        }

        // Esperar a que termine la animación de movimiento y desactivar NPC
        StartCoroutine(WaitForExitAndDespawn(2f)); // Ajusta el tiempo
    }

    private IEnumerator WaitForExitAndDespawn(float duration)
    {
        yield return new WaitForSeconds(duration);

        // Desactivar/destruir NPC
        Destroy(currentNPC);

        // Resetear UI para el próximo NPC
        uiManager.ResetUI();

        // Spawnear el siguiente NPC
        SpawnNPC();
    }

    // Opcional: Para limpiar al destruir
    private void OnDestroy()
    {
        uiManager.OnNPCDecision -= HandleNPCDecision;
    }
}