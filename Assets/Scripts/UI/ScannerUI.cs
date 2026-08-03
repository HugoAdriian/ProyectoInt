using TMPro;
using UnityEngine;

public class ScannerUI : MonoBehaviour
{
    #region REFERENCES

    [Header("Panel")]
    [SerializeField] private GameObject scannerPanel;
    [SerializeField] private GameObject actionText;

    [Header("Textos")]
    [SerializeField] private TMP_Text productText;
    [SerializeField] private TMP_Text skuText;
    [SerializeField] private TMP_Text rackText;
    [SerializeField] private TMP_Text quantityText;

    #endregion

    #region UNITY

    private void Start()
    {
        Hide();
    }

    #endregion

    #region PALLET UI

    public void Show(Pallet pallet)
    {
        if (pallet == null)
            return;

        scannerPanel.SetActive(true);

        productText.text =
            $"Producto: {pallet.productName}";

        skuText.text =
            $"SKU: {pallet.sku}";

        rackText.text =
            $"Ubicación: {pallet.rack} | Nivel: {pallet.level}";

        quantityText.text =
            $"Cantidad: {pallet.quantity}";
    }

    #endregion

    #region LOCATION UI

    public void Show(StorageLocation location)
    {
        if (location == null)
            return;

        scannerPanel.SetActive(true);

        productText.text =
            $"Ubicación: {location.locationID}";

        skuText.text =
            $"Rack: {location.rack}";

        rackText.text =
            $"Nivel: {location.level}";

        quantityText.text =
            location.CanReceivePallet()
                ? "Estado: Disponible"
                : "Estado: Ocupada";
    }

    #endregion

    #region MESSAGE UI

    public void ShowMessage(
        string title,
        string message)
    {
        scannerPanel.SetActive(true);

        productText.text = title;
        skuText.text = message;
        rackText.text = string.Empty;
        quantityText.text = string.Empty;
    }

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

    public void Hide()
    {
        if (scannerPanel != null)
            scannerPanel.SetActive(false);

        if (actionText != null)
            actionText.SetActive(false);
    }

    #endregion
}