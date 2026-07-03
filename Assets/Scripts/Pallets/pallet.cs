using UnityEngine;

public class Pallet : MonoBehaviour
{
    [Header("Información")]
    public string palletID = "P001";

    public string sku = "ELC-LAP-0041";

    public string productName = "Laptop ASUS";

    public int quantity = 20;

    public float weight = 350f;

    [Header("Ubicación")]

    public string rack = "A1";

    public int level = 1;

    [Header("Estado")]

    public bool isPicked = false;

    public bool delivered = false;
}