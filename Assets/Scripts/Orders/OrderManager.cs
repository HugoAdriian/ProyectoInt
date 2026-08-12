using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OrderManager : MonoBehaviour
{
    #region UI REFERENCES

    [Header("Interfaces")]
    [SerializeField] private OrderUI orderUI;
    [SerializeField] private WarehouseHUDController warehouseHUD;

    #endregion

    #region PUT AWAY TASKS

    [Header("Tareas Put Away")]
    [SerializeField]
    private List<PutAwayTask> putAwayTasks =
        new List<PutAwayTask>();

    [SerializeField] private int currentTaskIndex = 0;

    private PutAwayTask currentTask;

    //--------------------------------------------------
    // Registro de tareas completadas
    //--------------------------------------------------

    private HashSet<int> completedTaskIndexes =
        new HashSet<int>();

    #endregion

    #region CURRENT ORDER

    [Header("Orden actual")]
    public Order currentOrder;

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
        StartCurrentTask();
    }

    #endregion

    #region TASK MANAGEMENT

    private void StartCurrentTask()
    {
        //--------------------------------------------------
        // Validar lista
        //--------------------------------------------------

        if (putAwayTasks == null ||
            putAwayTasks.Count == 0)
        {
            Debug.LogError(
                "OrderManager: No hay tareas " +
                "Put Away configuradas.",
                this);

            return;
        }

        //--------------------------------------------------
        // Validar índice
        //--------------------------------------------------

        if (currentTaskIndex < 0 ||
            currentTaskIndex >=
            putAwayTasks.Count)
        {
            Debug.LogError(
                "OrderManager: Current Task Index " +
                "fuera de rango.",
                this);

            return;
        }

        //--------------------------------------------------
        // Obtener tarea
        //--------------------------------------------------

        currentTask =
            putAwayTasks[currentTaskIndex];

        if (currentTask == null)
        {
            Debug.LogError(
                $"OrderManager: Tarea " +
                $"{currentTaskIndex} es NULL.",
                this);

            return;
        }

        //--------------------------------------------------
        // Reiniciar estado
        //--------------------------------------------------

        ResetTaskState();

        //--------------------------------------------------
        // Crear orden
        //--------------------------------------------------

        CreatePutAwayOrder();

        //--------------------------------------------------
        // Mostrar misión
        //--------------------------------------------------

        ShowCurrentMission();

        //--------------------------------------------------
        // Resaltar destino
        //--------------------------------------------------

        HighlightCurrentDestination();

        //--------------------------------------------------
        // Iniciar cronómetro
        //--------------------------------------------------

        taskStartTime =
            Time.time;

        //--------------------------------------------------
        // Actualizar pantalla física
        //--------------------------------------------------

        UpdateTaskDisplay();

        Debug.Log(
            $"Tarea iniciada: " +
            $"{currentTask.taskID} | " +

            $"Pallet: " +
            $"{currentTask.palletID} | " +

            $"Destino: " +
            $"{currentTask.destinationLocationID}");
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

    #region NEXT TASK

    public void StartNextTask()
    {
        //--------------------------------------------------
        // Debe terminar la tarea actual
        //--------------------------------------------------

        if (!taskCompleted)
        {
            Debug.LogWarning(
                "No se puede iniciar otra tarea " +
                "porque la actual no ha terminado.");

            return;
        }

        //--------------------------------------------------
        // Validar que exista otra
        //--------------------------------------------------

        if (!HasNextTask())
        {
            Debug.Log(
                "Todas las tareas Put Away " +
                "han sido completadas.");

            return;
        }

        //--------------------------------------------------
        // Avanzar índice
        //--------------------------------------------------

        currentTaskIndex++;

        //--------------------------------------------------
        // Iniciar nueva tarea
        //--------------------------------------------------

        StartCurrentTask();

        Debug.Log(
            $"Nueva tarea Put Away iniciada: " +
            $"{currentTask.taskID}");
    }

    #endregion

    #region RESTART SIMULATION

    public void RestartSimulation()
    {
        Debug.Log(
            "Reiniciando simulación...");

        //--------------------------------------------------
        // Recargar escena actual
        //--------------------------------------------------

        Scene currentScene =
            SceneManager.GetActiveScene();

        SceneManager.LoadScene(
            currentScene.buildIndex);
    }

    #endregion

    #region ORDER CREATION

    private void CreatePutAwayOrder()
    {
        currentOrder =
            new Order
            {
                orderID =
                    currentTask.taskID,

                customer =
                    "Almacenamiento dirigido",

                items =
                    new List<OrderItem>
                    {
                        new OrderItem
                        {
                            palletID =
                                currentTask.palletID,

                            productName =
                                currentTask.productName,

                            scanned = false
                        }
                    }
            };

        currentOrder.completedProducts =
            0;
    }

    private void ShowCurrentMission()
    {
        if (warehouseHUD == null)
            return;

        warehouseHUD.ShowMission(
            currentTask.taskID,
            currentTask.palletID,
            currentTask.productName,
            currentTask.originLocationID,
            currentTask.destinationLocationID);
    }

    private void HighlightCurrentDestination()
    {
        if (currentTask.destinationLocation ==
            null)
        {
            Debug.LogWarning(
                $"Tarea {currentTask.taskID}: " +
                "Destination Location no asignado.",
                this);

            return;
        }

        currentTask.destinationLocation
            .HighlightAsDestination();
    }

    #endregion

    #region PALLET VALIDATION

    public bool ValidatePallet(
        Pallet pallet)
    {
        scanCount++;

        //--------------------------------------------------
        // Tarea completada
        //--------------------------------------------------

        if (taskCompleted)
        {
            RegisterError(
                "TAREA COMPLETADA",
                "La operación ya fue finalizada.");

            return false;
        }

        //--------------------------------------------------
        // Pallet NULL
        //--------------------------------------------------

        if (pallet == null)
        {
            RegisterError(
                "ESCANEO INVÁLIDO",
                "No se recibió información " +
                "de la tarima.");

            return false;
        }

        //--------------------------------------------------
        // Pallet incorrecto
        //--------------------------------------------------

        if (pallet.palletID !=
            currentTask.palletID)
        {
            RegisterError(
                "TARIMA INCORRECTA",

                $"Esperada: " +
                $"{currentTask.palletID} | " +

                $"Escaneada: " +
                $"{pallet.palletID}");

            return false;
        }

        //--------------------------------------------------
        // Pallet correcto
        //--------------------------------------------------

        scannedPallet =
            pallet;

        palletValidated =
            true;

        MarkOrderItemAsScanned(
            pallet);

        ShowSuccess(
            "TARIMA CORRECTA",

            $"{pallet.palletID} validada. " +
            $"Destino: " +
            $"{currentTask.destinationLocationID}");

        Debug.Log(
            $"Tarima validada correctamente: " +
            $"{pallet.palletID}");

        return true;
    }

    private void MarkOrderItemAsScanned(
        Pallet pallet)
    {
        if (currentOrder == null ||
            currentOrder.items == null)
        {
            return;
        }

        foreach (
            OrderItem item
            in currentOrder.items)
        {
            if (item.palletID !=
                pallet.palletID)
            {
                continue;
            }

            if (!item.scanned)
            {
                item.scanned =
                    true;

                currentOrder
                    .completedProducts++;
            }

            break;
        }
    }

    #endregion

    #region DESTINATION VALIDATION

    public bool ValidateDestination(
        StorageLocation location)
    {
        scanCount++;

        //--------------------------------------------------
        // Tarea completada
        //--------------------------------------------------

        if (taskCompleted)
        {
            RegisterError(
                "TAREA COMPLETADA",
                "La operación ya fue finalizada.");

            return false;
        }

        //--------------------------------------------------
        // Pallet no validado
        //--------------------------------------------------

        if (!palletValidated ||
            scannedPallet == null)
        {
            RegisterError(
                "SECUENCIA INCORRECTA",

                "Primero debe escanear " +
                "la tarima asignada.");

            return false;
        }

        //--------------------------------------------------
        // Location NULL
        //--------------------------------------------------

        if (location == null)
        {
            RegisterError(
                "UBICACIÓN INVÁLIDA",

                "No se recibió una " +
                "ubicación válida.");

            return false;
        }

        //--------------------------------------------------
        // Ubicación incorrecta
        //--------------------------------------------------

        if (location.locationID !=
            currentTask.destinationLocationID)
        {
            RegisterError(
                "UBICACIÓN INCORRECTA",

                $"Destino esperado: " +
                $"{currentTask.destinationLocationID}");

            Debug.LogWarning(
                $"Ubicación escaneada: " +
                $"{location.locationID}");

            return false;
        }

        //--------------------------------------------------
        // Ubicación ocupada
        //--------------------------------------------------

        if (!location.CanReceivePallet())
        {
            RegisterError(
                "UBICACIÓN OCUPADA",

                $"{location.locationID} " +
                "no está disponible.");

            return false;
        }

        //--------------------------------------------------
        // Ubicación correcta
        //--------------------------------------------------

        scannedDestination =
            location;

        destinationValidated =
            true;

        scannedDestination
            .MarkAsValidated();

        ShowSuccess(
            "UBICACIÓN CORRECTA",
            "Puede depositar la tarima.");

        Debug.Log(
            $"Destino validado correctamente: " +
            $"{location.locationID}");

        return true;
    }

    #endregion

    #region DELIVERY

    public bool ConfirmDelivery(
        Pallet pallet)
    {
        //--------------------------------------------------
        // Tarea ya completa
        //--------------------------------------------------

        if (taskCompleted)
            return false;

        //--------------------------------------------------
        // Pallet sin validar
        //--------------------------------------------------

        if (!palletValidated ||
            scannedPallet == null)
        {
            RegisterError(
                "ENTREGA NO AUTORIZADA",

                "La tarima no ha sido validada.");

            return false;
        }

        //--------------------------------------------------
        // Destino sin validar
        //--------------------------------------------------

        if (!destinationValidated ||
            scannedDestination == null)
        {
            RegisterError(
                "ENTREGA NO AUTORIZADA",

                "Debe escanear la " +
                "ubicación destino.");

            return false;
        }

        //--------------------------------------------------
        // Pallet incorrecto
        //--------------------------------------------------

        if (pallet == null ||
            pallet != scannedPallet)
        {
            RegisterError(
                "TARIMA INCORRECTA",

                "La carga depositada no " +
                "corresponde a la tarea.");

            return false;
        }

        //--------------------------------------------------
        // Destino ocupado
        //--------------------------------------------------

        if (!scannedDestination
            .CanReceivePallet())
        {
            RegisterError(
                "UBICACIÓN OCUPADA",

                "La ubicación ya no está " +
                "disponible.");

            return false;
        }

        //--------------------------------------------------
        // ALMACENAR
        //--------------------------------------------------

        scannedDestination
            .PlaceAndStorePallet(
                pallet);

        //--------------------------------------------------
        // Tarea completa
        //--------------------------------------------------

        taskCompleted =
            true;

        //--------------------------------------------------
        // Registrar historial
        //--------------------------------------------------

        completedTaskIndexes.Add(
            currentTaskIndex);

        //--------------------------------------------------
        // Actualizar monitor físico
        //--------------------------------------------------

        UpdateTaskDisplay();

        //--------------------------------------------------
        // Métricas
        //--------------------------------------------------

        float elapsedTime =
            Time.time -
            taskStartTime;

        //--------------------------------------------------
        // HUD
        //--------------------------------------------------

        if (warehouseHUD != null)
        {
            warehouseHUD.ShowNotification(
                "MOVIMIENTO COMPLETADO",

                $"{pallet.palletID} " +
                $"almacenada en " +
                $"{scannedDestination.locationID}",

                2.5f);

            warehouseHUD.ShowSummary(
                pallet.palletID,
                scannedDestination.locationID,
                elapsedTime,
                scanCount,
                errorCount);
        }

        Debug.Log(
            $"===== TAREA " +
            $"{currentTask.taskID} " +
            $"COMPLETADA =====");

        return true;
    }

    #endregion

    #region TASK DISPLAY

    private void UpdateTaskDisplay()
    {
        if (orderUI == null)
            return;

        orderUI.UpdateTaskBoard(
            putAwayTasks,
            currentTaskIndex,
            completedTaskIndexes,
            taskCompleted);
    }

    #endregion

    #region PUBLIC TASK INFO

    public PutAwayTask GetCurrentTask()
    {
        return currentTask;
    }

    public int GetCurrentTaskIndex()
    {
        return currentTaskIndex;
    }

    public int GetTaskCount()
    {
        return putAwayTasks.Count;
    }

    public bool HasNextTask()
    {
        return currentTaskIndex <
               putAwayTasks.Count - 1;
    }

    #endregion

    #region NOTIFICATIONS

    private void ShowSuccess(
        string title,
        string message)
    {
        if (warehouseHUD != null)
        {
            warehouseHUD.ShowNotification(
                title,
                message);
        }

        Debug.Log(
            $"{title}: {message}");
    }

    private void RegisterError(
        string title,
        string message)
    {
        errorCount++;

        if (warehouseHUD != null)
        {
            warehouseHUD.ShowNotification(
                title,
                message);
        }

        Debug.LogWarning(
            $"{title}: {message}");
    }

    #endregion
}