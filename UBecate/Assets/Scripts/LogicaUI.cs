// --- File: UIManager.cs ---
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro; // Para TextMeshProUGUI

public class LogicaUI : MonoBehaviour {
    [Header("Panel principal de documentos")]
    public Image appearanceImage; // Apariencia real del NPC en la fila
    public Image idPhotoImage; // Fotografía en la ID card
    public TextMeshProUGUI idNameText;
    public TextMeshProUGUI idNumberText;
    public TextMeshProUGUI idKeyText;
    public TextMeshProUGUI idCityText;

    public Image paymentImage;
    public TextMeshProUGUI paymentDateText;
    public TextMeshProUGUI paymentTimeText;

    [Header("Panel computadora (datos de referencia)")]
    public TextMeshProUGUI compNameText;
    public TextMeshProUGUI compNumberText;
    public TextMeshProUGUI compKeyText;
    public TextMeshProUGUI compCityText;
    public TextMeshProUGUI compPaymentDateText;
    public TextMeshProUGUI compPaymentTimeText;

    [Header("Panel de marcado (cuando hay inconsistencias)")]
    public GameObject markPanel; // Panel con toggles para marcar faltas
    public List<Toggle> markToggles; // Orden: foto, nombre, número, clave, ciudad, fecha/hora

    [Header("Botones")]
    public Button acceptButton;
    public Button rejectButton;

    [Header("Referencias")]
    public Sprite correctPaymentImage; // Sprite "oficial" para comprobante válido

    private GameManager gm;

    void Awake() {
        gm = FindObjectOfType<GameManager>();
        acceptButton.onClick.AddListener(() => gm.OnAcceptPressed());
        rejectButton.onClick.AddListener(() => gm.OnRejectPressed());
    }

    public void ShowNPC(NPC npc) {
        if (npc == null) return;
        appearanceImage.sprite = npc.appearanceSprite;

        if (npc.idCard != null) {
            idPhotoImage.sprite = npc.idCard.photo;
            idNameText.text = npc.idCard.fullName;
            idNumberText.text = npc.idCard.userNumber.ToString();
            idKeyText.text = npc.idCard.uniqueKey;
            idCityText.text = npc.idCard.city;
        }

        if (npc.paymentProof != null) {
            paymentImage.sprite = npc.paymentProof.image;
            paymentDateText.text = npc.paymentProof.dateTime.ToString("yyyy-MM-dd");
            paymentTimeText.text = npc.paymentProof.dateTime.ToString("HH:mm:ss");
        }

        // Actualizamos la computadora con los datos "oficiales" REALES
        UpdateComputerDisplay(npc);
        // Ocultar panel de marcado por defecto
        HideMarkPanel();
        ClearMarkToggles();
    }

    void UpdateComputerDisplay(NPC npc) {
        // Muestra datos REALES (para que el jugador compare con documentos presentados)
        compNameText.text = npc.realFullName;
        compNumberText.text = npc.realUserNumber.ToString();
        compKeyText.text = npc.realUniqueKey;
        compCityText.text = npc.realCity;
        compPaymentDateText.text = npc.realPaymentDateTime.ToString("yyyy-MM-dd");
        compPaymentTimeText.text = npc.realPaymentDateTime.ToString("HH:mm:ss");
    }

    public void ShowMarkPanel() {
        markPanel.SetActive(true);
        ClearMarkToggles();
    }

    public void HideMarkPanel() {
        markPanel.SetActive(false);
    }

    void ClearMarkToggles() {
        foreach (var t in markToggles) t.isOn = false;
    }

    public List<string> GetMarkedIssues() {
        var issues = new List<string>();
        // Orden fijo: foto, nombre, numero, clave, ciudad, fechaHora
        string[] issueNames = { "foto", "nombre", "numero", "clave", "ciudad", "fechaHora" };
        for (int i = 0; i < markToggles.Count && i < issueNames.Length; i++) {
            if (markToggles[i].isOn) issues.Add(issueNames[i]);
        }
        return issues;
    }

    public void ShowGameOver() {
        // Implementa tu UI de Game Over aquí (e.g., activa un panel)
        Debug.Log("Mostrando Game Over UI");
    }
}