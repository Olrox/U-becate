// --- File: NPCData.cs ---
using UnityEngine;

[System.Serializable]
public class IDCard {
    public Sprite photo;
    public string fullName;
    public int userNumber;
    public string uniqueKey; // alfanumérica
    public string city;
}

[System.Serializable]
public class PaymentProof {
    public Sprite image;
    public System.DateTime dateTime;
}

[System.Serializable]
public class AuthorizationLetter {
    public Sprite image;
    public string councilReference;
}

[System.Serializable]
public class NPC {
    // === DATOS REALES (para computadora) ===
    public string realFullName;
    public int realUserNumber;
    public string realUniqueKey;
    public string realCity;
    public System.DateTime realPaymentDateTime;
    public Sprite realAppearanceSprite;

    // === DATOS PRESENTADOS ===
    public Sprite appearanceSprite;
    public IDCard idCard;
    public PaymentProof paymentProof;
    public AuthorizationLetter letter;

    // === BANDERAS ===
    public bool idMatchesAppearance;
    public bool idDataValid;
    public bool paymentValid;
    public bool isSpecialCase;
}