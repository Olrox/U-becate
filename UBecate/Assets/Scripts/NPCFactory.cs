// --- File: NPCFactory.cs ---
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class NPCFactory : MonoBehaviour {
    [Header("Sprites de apariencia disponibles")] public List<Sprite> appearances;
    [Header("Fotos para ID Cards")] public List<Sprite> idPhotos;
    [Header("Sprites para Payment Proof")] public List<Sprite> paymentSprites; // Agrega sprites válidos/inválidos

    [Header("Ciudades posibles")] public List<string> cities;

    [Header("Rango de fechas válidas (días)")] public int validDaysRange = 5;

    // Genera un NPC completo
    public NPC GenerateNPC(bool forceInvalid = false) {
        NPC npc = new NPC();

        // Generar datos REALES primero
        npc.realFullName = GenerateName();
        npc.realUserNumber = UnityEngine.Random.Range(1000, 9999);
        npc.realUniqueKey = GenerateUniqueKey();
        npc.realCity = cities[UnityEngine.Random.Range(0, cities.Count)];
        npc.realPaymentDateTime = DateTime.Now; // Fecha real válida (puedes alterar)
        npc.realAppearanceSprite = appearances[UnityEngine.Random.Range(0, appearances.Count)];

        // Apariencia visible (por default coincide)
        npc.appearanceSprite = npc.realAppearanceSprite;

        // Crear ID Card (copia de reales por default)
        IDCard card = new IDCard();
        card.photo = idPhotos[UnityEngine.Random.Range(0, idPhotos.Count)]; // Por default, usa una que coincida si posible
        card.fullName = npc.realFullName;
        card.userNumber = npc.realUserNumber;
        card.uniqueKey = npc.realUniqueKey;
        card.city = npc.realCity;
        npc.idCard = card;

        // Payment proof (copia de real)
        PaymentProof pp = new PaymentProof();
        pp.dateTime = npc.realPaymentDateTime;
        pp.image = paymentSprites[UnityEngine.Random.Range(0, paymentSprites.Count)]; // Asigna sprite
        npc.paymentProof = pp;

        // Carta (null por default)
        npc.letter = null;
        npc.isSpecialCase = false;

        // Forzar errores si es inválido (aleatorio)
        if (forceInvalid || UnityEngine.Random.value < 0.5f) { // 50% chance natural de inválido
            int numErrors = UnityEngine.Random.Range(1, 4); // 1-3 errores
            for (int i = 0; i < numErrors; i++) {
                int errorType = UnityEngine.Random.Range(0, 6);
                switch (errorType) {
                    case 0: card.fullName = GenerateName(); break; // Nombre falso
                    case 1: card.userNumber = UnityEngine.Random.Range(1000, 9999); break; // Número falso
                    case 2: card.uniqueKey = GenerateUniqueKey(); break; // Clave falsa
                    case 3: card.city = cities[UnityEngine.Random.Range(0, cities.Count)]; break; // Ciudad falsa
                    case 4: card.photo = idPhotos[UnityEngine.Random.Range(0, idPhotos.Count)]; break; // Foto falsa
                    case 5: pp.dateTime = DateTime.Now.AddDays(UnityEngine.Random.Range(-10, -validDaysRange - 1)); break; // Fecha inválida
                }
            }
        }

        // Caso especial: Si foto no coincide, 50% chance de carta válida
        if (card.photo != npc.realAppearanceSprite && UnityEngine.Random.value < 0.5f) {
            AuthorizationLetter letter = new AuthorizationLetter();
            letter.image = null; // Asigna sprite si tienes
            letter.councilReference = "REF-" + GenerateUniqueKey(); // Referencia válida
            npc.letter = letter;
            npc.isSpecialCase = true;
        }

        // Calcular flags de validez (para GameManager)
        npc.idMatchesAppearance = (card.photo == npc.realAppearanceSprite);
        npc.idDataValid = (card.fullName == npc.realFullName &&
                           card.userNumber == npc.realUserNumber &&
                           card.uniqueKey == npc.realUniqueKey &&
                           card.city == npc.realCity);
        npc.paymentValid = Math.Abs((pp.dateTime - npc.realPaymentDateTime).TotalDays) <= validDaysRange;

        return npc;
    }

    string GenerateName() {
        string[] first = { "Ana", "Luis", "Pedro", "Mara", "Joel", "Rina", "Carlos" };
        string[] last = { "Torres", "Gomez", "Perez", "Rojas", "Lopez", "Sierra" };
        return first[UnityEngine.Random.Range(0, first.Length)] + " " + last[UnityEngine.Random.Range(0, last.Length)];
    }

    string GenerateUniqueKey() {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string k = "";
        for (int i = 0; i < 8; i++) k += chars[UnityEngine.Random.Range(0, chars.Length)];
        return k;
    }
}