using TMPro;
using UnityEngine;

public class ScannerUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject scannerPanel;

    public GameObject actionText;

    [Header("Textos")]
    public TMP_Text productText;
    public TMP_Text skuText;
    public TMP_Text rackText;
    public TMP_Text quantityText;

    public void Show(Pallet pallet)
    {
        scannerPanel.SetActive(true);

        productText.text = "Producto: " + pallet.productName;
        skuText.text = "SKU: " + pallet.sku;
        rackText.text = "Rack: " + pallet.rack + "   Nivel: " + pallet.level;
        quantityText.text = "Cantidad: " + pallet.quantity;
    }

    public void Hide()
    {
        scannerPanel.SetActive(false);
        actionText.SetActive(false);
    }

    public void ShowAction()
    {
        actionText.SetActive(true);
    }

    public void HideAction()
    {
        actionText.SetActive(false);
    }
    void Start()
    {
        scannerPanel.SetActive(false);
        actionText.SetActive(false);
    }
}