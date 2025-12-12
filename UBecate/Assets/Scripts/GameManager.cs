// --- File: GameManager.cs ---
using UnityEngine;
using System.Collections.Generic;
using TMPro; // Si usas TextMeshPro

public class GameManager : MonoBehaviour
{
    public SpawnPoint spawner;
    public LogicaUI ui;

    [Header("Puntajes")]
    public int pointsForPerfectAccept = 100;
    public int pointsPerCorrectMark = 25;
    public int pointsForCorrectReject = 20;

    [Header("Condiciones de pérdida")]
    public int maxInvalidAccepts = 2;

    private NPC currentNPC;
    private int score = 0;
    private int invalidAccepts = 0;
    private bool isMarkingMode = false;

    void Start()
    {
        NextTurn();
    }

    public void NextTurn()
    {
        currentNPC = spawner.PeekNext();
        if (currentNPC == null)
        {
            Debug.Log("¡Nivel completado! Puntuación final: " + score);
            return;
        }

        ui.ShowNPC(currentNPC);
        isMarkingMode = false;
        ui.HideMarkPanel();
    }

    // === BOTÓN ACEPTAR ===
    public void OnAcceptPressed()
    {
        if (currentNPC == null) return;

        bool isValid = VerifyNPC(currentNPC);

        if (isValid)
        {
            score += pointsForPerfectAccept;
            Debug.Log($"Aceptado correctamente. +{pointsForPerfectAccept} pts. Puntaje: {score}");
            EndAndAdvance();
        }
        else
        {
            invalidAccepts++;
            isMarkingMode = true;
            ui.ShowMarkPanel();
            Debug.Log($"¡Error! NPC inválido aceptado. Intentos fallidos: {invalidAccepts}/{maxInvalidAccepts}");
            CheckLossCondition();
        }
    }

    // === CONFIRMAR MARCAS ===
    public void ConfirmMarking()
    {
        if (currentNPC == null || !isMarkingMode) return;

        var marks = ui.GetMarkedIssues();
        int correctMarks = EvaluateMarks(marks, currentNPC);

        score += correctMarks * pointsPerCorrectMark;
        Debug.Log($"Marcas correctas: {correctMarks} → +{correctMarks * pointsPerCorrectMark} pts. Puntaje: {score}");

        EndAndAdvance();
    }

    // === BOTÓN RECHAZAR ===
    public void OnRejectPressed()
    {
        if (currentNPC == null) return;

        bool isValid = VerifyNPC(currentNPC);

        if (!isValid)
        {
            score += pointsForCorrectReject;
            Debug.Log($"Rechazado correctamente. +{pointsForCorrectReject} pts. Puntaje: {score}");
        }
        else
        {
            Debug.Log("Rechazado, pero el NPC era válido.");
        }

        EndAndAdvance();
    }

    // === VERIFICAR SI EL NPC ES VÁLIDO (usando datos REALES) ===
    private bool VerifyNPC(NPC npc)
    {
        if (npc == null) return false;

        // CASO ESPECIAL: Tiene carta válida y foto diferente → VÁLIDO
        if (npc.isSpecialCase && npc.letter != null && !string.IsNullOrEmpty(npc.letter.councilReference))
        {
            return true;
        }

        // CASO NORMAL: Todo debe coincidir con los datos REALES
        bool nameOK = npc.idCard.fullName == npc.realFullName;
        bool numberOK = npc.idCard.userNumber == npc.realUserNumber;
        bool keyOK = npc.idCard.uniqueKey == npc.realUniqueKey;
        bool cityOK = npc.idCard.city == npc.realCity;
        bool photoOK = npc.idCard.photo == npc.realAppearanceSprite; // Foto debe coincidir con apariencia real
        bool paymentOK = Mathf.Abs((float)(npc.paymentProof.dateTime - npc.realPaymentDateTime).TotalDays) <= 5; // Dentro de rango

        return nameOK && numberOK && keyOK && cityOK && photoOK && paymentOK;
    }

    // === EVALUAR MARCAS DEL JUGADOR ===
    private int EvaluateMarks(List<string> marks, NPC npc)
    {
        int correct = 0;

        foreach (var m in marks)
        {
            switch (m)
            {
                case "foto":
                    if (npc.idCard.photo != npc.realAppearanceSprite && !npc.isSpecialCase) correct++;
                    break;
                case "nombre":
                    if (npc.idCard.fullName != npc.realFullName) correct++;
                    break;
                case "numero":
                    if (npc.idCard.userNumber != npc.realUserNumber) correct++;
                    break;
                case "clave":
                    if (npc.idCard.uniqueKey != npc.realUniqueKey) correct++;
                    break;
                case "ciudad":
                    if (npc.idCard.city != npc.realCity) correct++;
                    break;
                case "fechaHora":
                    if (Mathf.Abs((float)(npc.paymentProof.dateTime - npc.realPaymentDateTime).TotalDays) > 5) correct++;
                    break;
            }
        }
        return correct;
    }

    // === FINALIZAR TURNO Y AVANZAR ===
    private void EndAndAdvance()
    {
        if (currentNPC != null)
        {
            spawner.DequeueNext();
        }
        NextTurn();
    }

    // === VERIFICAR DERROTA ===
    private void CheckLossCondition()
    {
        if (invalidAccepts >= maxInvalidAccepts)
        {
            Debug.Log("¡GAME OVER! Has aceptado demasiados NPCs inválidos.");
            ui.ShowGameOver();
            this.enabled = false;
        }
    }
}