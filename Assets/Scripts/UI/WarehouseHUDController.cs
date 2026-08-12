using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarehouseHUDController : MonoBehaviour
{
    #region REFERENCES

    [Header("Order Manager")]
    [SerializeField] private OrderManager orderManager;

    [Header("Mission Panel")]
    [SerializeField] private GameObject missionPanel;
    [SerializeField] private TMP_Text missionTitle;
    [SerializeField] private TMP_Text missionDescription;
    [SerializeField] private TMP_Text missionPallet;
    [SerializeField] private TMP_Text missionOrigin;
    [SerializeField] private TMP_Text missionDestination;

    [Header("Notification Panel")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private TMP_Text notificationTitle;
    [SerializeField] private TMP_Text notificationMessage;

    [Header("Summary Panel")]
    [SerializeField] private GameObject summaryPanel;
    [SerializeField] private TMP_Text summaryTitle;
    [SerializeField] private TMP_Text summaryBody;

    [Header("Summary Buttons")]
    [SerializeField] private Button newTaskButton;
    [SerializeField] private Button restartButton;

    [Header("Camera Control")]
    [SerializeField] private MonoBehaviour firstPersonController;
    [SerializeField] private MonoBehaviour reachliftCameraLook;

    #endregion

    #region PRIVATE

    private Coroutine notificationCoroutine;

    private bool firstPersonWasActive;
    private bool reachliftCameraWasActive;

    #endregion

    #region UNITY

    private void Awake()
    {
        if (newTaskButton != null)
        {
            newTaskButton.onClick.RemoveAllListeners();
            newTaskButton.onClick.AddListener(
                OnNewTaskPressed);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(
                OnRestartPressed);
        }
    }

    private void Start()
    {
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }

        if (summaryPanel != null)
        {
            summaryPanel.SetActive(false);
        }

        LockCursor();
    }

    #endregion

    #region MISSION

    public void ShowMission(
        string taskID,
        string palletID,
        string productName,
        string origin,
        string destination)
    {
        if (missionPanel != null)
        {
            missionPanel.SetActive(true);
        }

        if (missionTitle != null)
        {
            missionTitle.text =
                $"TAREA {taskID}";
        }

        if (missionDescription != null)
        {
            missionDescription.text =
                "Realizar operación Put Away";
        }

        if (missionPallet != null)
        {
            missionPallet.text =
                $"Pallet: {palletID}\n" +
                $"Producto: {productName}";
        }

        if (missionOrigin != null)
        {
            missionOrigin.text =
                $"Origen: {origin}";
        }

        if (missionDestination != null)
        {
            missionDestination.text =
                $"Destino: {destination}";
        }
    }

    #endregion

    #region NOTIFICATION

    public void ShowNotification(
        string title,
        string message,
        float duration = 2.5f)
    {
        if (notificationPanel == null)
        {
            return;
        }

        if (notificationCoroutine != null)
        {
            StopCoroutine(
                notificationCoroutine);
        }

        if (notificationTitle != null)
        {
            notificationTitle.text = title;
        }

        if (notificationMessage != null)
        {
            notificationMessage.text = message;
        }

        notificationPanel.SetActive(true);

        notificationCoroutine =
            StartCoroutine(
                HideNotificationAfterDelay(
                    duration));
    }

    private IEnumerator HideNotificationAfterDelay(
        float duration)
    {
        yield return new WaitForSeconds(
            duration);

        HideNotification();
    }

    public void HideNotification()
    {
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }

        notificationCoroutine = null;
    }

    #endregion

    #region SUMMARY

    public void ShowSummary(
        string palletID,
        string destination,
        float elapsedTime,
        int scanCount,
        int errorCount)
    {
        if (summaryPanel == null)
        {
            return;
        }

        summaryPanel.SetActive(true);

        if (summaryTitle != null)
        {
            summaryTitle.text =
                "PUT AWAY COMPLETADO";
        }

        int minutes =
            Mathf.FloorToInt(
                elapsedTime / 60f);

        int seconds =
            Mathf.FloorToInt(
                elapsedTime % 60f);

        int successfulScans =
            Mathf.Max(
                scanCount - errorCount,
                0);

        float accuracy = 100f;

        if (scanCount > 0)
        {
            accuracy =
                ((float)successfulScans /
                 scanCount) * 100f;
        }

        if (summaryBody != null)
        {
            summaryBody.text =
                $"Pallet: {palletID}\n" +
                $"Destino: {destination}\n\n" +
                $"Tiempo: {minutes:00}:{seconds:00}\n" +
                $"Escaneos: {scanCount}\n" +
                $"Errores: {errorCount}\n" +
                $"Precisión: {accuracy:0}%";
        }

        UpdateNewTaskButton();

        // Detener la cámara y liberar el mouse.
        EnterSummaryMode();
    }

    public void HideSummary()
    {
        if (summaryPanel != null)
        {
            summaryPanel.SetActive(false);
        }

        // Recuperar el controlador que estaba activo.
        ExitSummaryMode();
    }

    private void UpdateNewTaskButton()
    {
        if (newTaskButton == null)
        {
            return;
        }

        if (orderManager == null)
        {
            newTaskButton.interactable = false;
            return;
        }

        bool hasNextTask =
            orderManager.HasNextTask();

        newTaskButton.interactable =
            hasNextTask;

        TMP_Text buttonText =
            newTaskButton
                .GetComponentInChildren<TMP_Text>();

        if (buttonText != null)
        {
            buttonText.text =
                hasNextTask
                    ? "NUEVA TAREA"
                    : "SIN TAREAS PENDIENTES";
        }
    }

    #endregion

    #region BUTTONS

    private void OnNewTaskPressed()
    {
        if (orderManager == null)
        {
            Debug.LogError(
                "WarehouseHUDController: " +
                "OrderManager no está asignado.",
                this);

            return;
        }

        HideSummary();
        HideNotification();

        orderManager.StartNextTask();
    }

    private void OnRestartPressed()
    {
        if (orderManager == null)
        {
            Debug.LogError(
                "WarehouseHUDController: " +
                "OrderManager no está asignado.",
                this);

            return;
        }

        orderManager.RestartSimulation();
    }

    #endregion

    #region SUMMARY MODE

    private void EnterSummaryMode()
    {
        //--------------------------------------------------
        // Recordar qué controlador estaba activo
        //--------------------------------------------------

        firstPersonWasActive =
            firstPersonController != null &&
            firstPersonController.enabled;

        reachliftCameraWasActive =
            reachliftCameraLook != null &&
            reachliftCameraLook.enabled;

        //--------------------------------------------------
        // Desactivar temporalmente ambos
        //--------------------------------------------------

        if (firstPersonController != null)
        {
            firstPersonController.enabled =
                false;
        }

        if (reachliftCameraLook != null)
        {
            reachliftCameraLook.enabled =
                false;
        }

        //--------------------------------------------------
        // Liberar cursor
        //--------------------------------------------------

        UnlockCursor();
    }

    private void ExitSummaryMode()
    {
        //--------------------------------------------------
        // Restaurar solamente lo que estaba activo
        //--------------------------------------------------

        if (firstPersonController != null)
        {
            firstPersonController.enabled =
                firstPersonWasActive;
        }

        if (reachliftCameraLook != null)
        {
            reachliftCameraLook.enabled =
                reachliftCameraWasActive;
        }

        //--------------------------------------------------
        // Recuperar control del mouse
        //--------------------------------------------------

        LockCursor();
    }

    #endregion

    #region CURSOR CONTROL

    private void UnlockCursor()
    {
        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible =
            true;
    }

    private void LockCursor()
    {
        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible =
            false;
    }

    #endregion
}