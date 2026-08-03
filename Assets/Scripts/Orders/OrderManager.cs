using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    #region UI REFERENCES

    [Header("Interfaces")]
    [SerializeField] private OrderUI orderUI;
    [SerializeField] private ScannerUI scannerUI;
    [SerializeField] private WarehouseHUDController warehouseHUD;

    #endregion

    #region CURRENT ORDER

    [Header("Orden actual")]
    public Order currentOrder;

    [Header("Configuración de Put Away")]
    [SerializeField] private string requiredPalletID = "P001";
    [SerializeField] private string requiredProductName = "Laptop ASUS";
    [SerializeField] private string originLocationID = "RECEIVING";
    [SerializeField] private string destinationLocationID = "A1-L1";

    [Tooltip("Asigna aquí el objeto Location_A1_L1.")]
    [SerializeField] private StorageLocation destinationLocation;

    #endregion

    #region TASK STATE

    [Header("Estado de la tarea")]
    public Pallet scannedPallet;
    public StorageLocation scannedDestination;

    public bool palletValidated;
    public bool destinationValidated;
    public bool taskCompleted;

    #endregion

    #region TASK METRICS

    [Header("Métricas")]
    [SerializeField] private int scanCount;
    [SerializeField] private int errorCount;

    private float taskStartTime;

    #endregion

    #region UNITY

    private void Start()
    {
        CreatePutAwayOrder();
    }

    #endregion

    #region ORDER CREATION

    private void CreatePutAwayOrder()
    {
        ResetTaskState();

        currentOrder = new Order
        {
            orderID = "PUT-0001",
            customer = "Almacenamiento dirigido",

            items = new List<OrderItem>
            {
                new OrderItem
                {
                    palletID = requiredPalletID,
                    productName = requiredProductName,
                    scanned = false
                }
            }
        };

        currentOrder.completedProducts = 0;
        taskStartTime = Time.time;

        if (orderUI != null)
        {
            orderUI.UpdateOrder(currentOrder);
        }

        if (warehouseHUD != null)
        {
            warehouseHUD.ShowMission(
                currentOrder.orderID,
                requiredPalletID,
                requiredProductName,
                originLocationID,
                destinationLocationID);
        }

        if (destinationLocation != null)
        {
            destinationLocation.HighlightAsDestination();
        }
        else
        {
            Debug.LogWarning(
                "OrderManager: Destination Location no está asignado.",
                this);
        }

        Debug.Log(
            $"Nueva tarea: mover {requiredPalletID} " +
            $"desde {originLocationID} hacia {destinationLocationID}.");
    }

    private void ResetTaskState()
    {
        scannedPallet = null;
        scannedDestination = null;

        palletValidated = false;
        destinationValidated = false;
        taskCompleted = false;

        scanCount = 0;
        errorCount = 0;
    }

    #endregion

    #region PALLET VALIDATION

    public bool ValidatePallet(Pallet pallet)
    {
        scanCount++;

        if (taskCompleted)
        {
            RegisterError(
                "TAREA COMPLETADA",
                "La operación ya fue finalizada.");

            return false;
        }

        if (pallet == null)
        {
            RegisterError(
                "ESCANEO INVÁLIDO",
                "No se recibió información de la tarima.");

            return false;
        }

        if (pallet.palletID != requiredPalletID)
        {
            RegisterError(
                "TARIMA INCORRECTA",
                $"Esperada: {requiredPalletID} | " +
                $"Escaneada: {pallet.palletID}");

            return false;
        }

        scannedPallet = pallet;
        palletValidated = true;

        MarkOrderItemAsScanned(pallet);

        ShowSuccess(
            "TARIMA CORRECTA",
            $"{pallet.palletID} validada. " +
            $"Destino: {destinationLocationID}");

        Debug.Log(
            $"Tarima validada correctamente: {pallet.palletID}");

        return true;
    }

    private void MarkOrderItemAsScanned(Pallet pallet)
    {
        if (currentOrder == null ||
            currentOrder.items == null)
        {
            return;
        }

        foreach (OrderItem item in currentOrder.items)
        {
            if (item.palletID != pallet.palletID)
                continue;

            if (!item.scanned)
            {
                item.scanned = true;
                currentOrder.completedProducts++;
            }

            break;
        }

        if (orderUI != null)
        {
            orderUI.UpdateOrder(currentOrder);
        }
    }

    #endregion

    #region DESTINATION VALIDATION

    public bool ValidateDestination(StorageLocation location)
    {
        scanCount++;

        if (taskCompleted)
        {
            RegisterError(
                "TAREA COMPLETADA",
                "La operación ya fue finalizada.");

            return false;
        }

        if (!palletValidated ||
            scannedPallet == null)
        {
            RegisterError(
                "SECUENCIA INCORRECTA",
                "Primero debe escanear la tarima asignada.");

            return false;
        }

        if (location == null)
        {
            RegisterError(
                "UBICACIÓN INVÁLIDA",
                "No se recibió una ubicación válida.");

            return false;
        }

        if (location.locationID != destinationLocationID)
        {
            RegisterError(
                "UBICACIÓN INCORRECTA",
                $"Destino esperado: {destinationLocationID}");

            Debug.LogWarning(
                $"Ubicación escaneada: {location.locationID}");

            return false;
        }

        if (!location.CanReceivePallet())
        {
            RegisterError(
                "UBICACIÓN OCUPADA",
                $"{location.locationID} no está disponible.");

            return false;
        }

        scannedDestination = location;
        destinationValidated = true;

        scannedDestination.MarkAsValidated();

        ShowSuccess(
            "UBICACIÓN CORRECTA",
            "Puede depositar la tarima.");

        Debug.Log(
            $"Destino validado correctamente: {location.locationID}");

        return true;
    }

    #endregion

    #region DELIVERY

    public bool ConfirmDelivery(Pallet pallet)
    {
        if (taskCompleted)
        {
            return false;
        }

        if (!palletValidated ||
            scannedPallet == null)
        {
            RegisterError(
                "ENTREGA NO AUTORIZADA",
                "La tarima no ha sido validada.");

            return false;
        }

        if (!destinationValidated ||
            scannedDestination == null)
        {
            RegisterError(
                "ENTREGA NO AUTORIZADA",
                "Debe escanear la ubicación destino.");

            return false;
        }

        if (pallet == null ||
            pallet != scannedPallet)
        {
            RegisterError(
                "TARIMA INCORRECTA",
                "La carga depositada no corresponde a la tarea.");

            return false;
        }

        if (!scannedDestination.CanReceivePallet())
        {
            RegisterError(
                "UBICACIÓN OCUPADA",
                "La ubicación ya no está disponible.");

            return false;
        }

        scannedDestination.PlaceAndStorePallet(pallet);

        taskCompleted = true;

        float elapsedTime =
            Time.time - taskStartTime;

        if (warehouseHUD != null)
        {
            warehouseHUD.ShowNotification(
                "MOVIMIENTO COMPLETADO",
                $"{pallet.palletID} almacenada en " +
                $"{scannedDestination.locationID}",
                2.5f);

            warehouseHUD.ShowSummary(
                pallet.palletID,
                scannedDestination.locationID,
                elapsedTime,
                scanCount,
                errorCount);
        }

        if (scannerUI != null)
        {
            scannerUI.ShowMessage(
                "Movimiento completado",
                $"{pallet.palletID} → " +
                $"{scannedDestination.locationID}");
        }

        Debug.Log(
            "===== TAREA PUT AWAY COMPLETADA =====");

        return true;
    }

    #endregion

    #region UI MESSAGES

    private void ShowSuccess(
        string title,
        string message)
    {
        if (scannerUI != null)
        {
            scannerUI.ShowMessage(
                title,
                message);
        }

        if (warehouseHUD != null)
        {
            warehouseHUD.ShowNotification(
                title,
                message);
        }

        Debug.Log($"{title}: {message}");
    }

    private void RegisterError(
        string title,
        string message)
    {
        errorCount++;

        if (scannerUI != null)
        {
            scannerUI.ShowMessage(
                title,
                message);
        }

        if (warehouseHUD != null)
        {
            warehouseHUD.ShowNotification(
                title,
                message);
        }

        Debug.LogWarning($"{title}: {message}");
    }

    #endregion
}