using TMPro;
using UnityEngine;

public class ScannerUI : MonoBehaviour
{
    #region REFERENCES

    [Header("Panel")]
    [SerializeField] private GameObject scannerPanel;
    [SerializeField] private GameObject actionText;

    [Header("Información del Pallet")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text palletIDText;
    [SerializeField] private TMP_Text productText;
    [SerializeField] private TMP_Text skuText;
    [SerializeField] private TMP_Text rackText;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private TMP_Text weightText;

    #endregion

    #region UNITY

    private void Start()
    {
        HidePalletInfo();
        HideAction();
    }

    #endregion

    #region PALLET INFORMATION

    public void ShowPalletInfo(Pallet pallet)
    {
        if (pallet == null)
            return;

        if (scannerPanel != null)
            scannerPanel.SetActive(true);

        if (statusText != null)
            statusText.text = "INFORMACIÓN DEL PALLET";

        if (palletIDText != null)
            palletIDText.text =
                $"Pallet ID: {pallet.palletID}";

        if (skuText != null)
            skuText.text =
                $"SKU: {pallet.sku}";

        if (productText != null)
            productText.text =
                $"Product Name: {pallet.productName}";

        if (rackText != null)
            rackText.text =
                $"Rack: {pallet.rack}";

        if (quantityText != null)
            quantityText.text =
                $"Quantity: {pallet.quantity}";

        if (weightText != null)
            weightText.text =
                $"Weight: {pallet.weight}";
    }

    public void HidePalletInfo()
    {
        if (scannerPanel != null)
            scannerPanel.SetActive(false);
    }

    #endregion

    #region ACTION

    public void ShowAction()
    {
        if (actionText != null)
            actionText.SetActive(true);
    }

    public void HideAction()
    {
        if (actionText != null)
            actionText.SetActive(false);
    }

    #endregion
}