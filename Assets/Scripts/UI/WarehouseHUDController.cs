using System.Collections;
using TMPro;
using UnityEngine;

public class WarehouseHUDController : MonoBehaviour
{
    #region MISSION PANEL

    [Header("Mission Panel")]
    [SerializeField] private CanvasGroup missionPanel;
    [SerializeField] private TMP_Text missionTitle;
    [SerializeField] private TMP_Text missionDescription;
    [SerializeField] private TMP_Text missionPallet;
    [SerializeField] private TMP_Text missionOrigin;
    [SerializeField] private TMP_Text missionDestination;

    #endregion

    #region NOTIFICATION PANEL

    [Header("Notification Panel")]
    [SerializeField] private CanvasGroup notificationPanel;
    [SerializeField] private TMP_Text notificationTitle;
    [SerializeField] private TMP_Text notificationMessage;

    #endregion

    #region SUMMARY PANEL

    [Header("Summary Panel")]
    [SerializeField] private CanvasGroup summaryPanel;
    [SerializeField] private TMP_Text summaryTitle;
    [SerializeField] private TMP_Text summaryBody;

    #endregion

    #region ANIMATION SETTINGS

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.35f;
    [SerializeField] private float missionVisibleDuration = 5f;
    [SerializeField] private float notificationVisibleDuration = 3f;

    #endregion

    #region COROUTINES

    private Coroutine missionCoroutine;
    private Coroutine notificationCoroutine;
    private Coroutine summaryCoroutine;

    #endregion

    #region UNITY

    private void Awake()
    {
        HideImmediately(missionPanel);
        HideImmediately(notificationPanel);
        HideImmediately(summaryPanel);
    }

    #endregion

    #region MISSION

    public void ShowMission(
        string orderID,
        string palletID,
        string productName,
        string origin,
        string destination)
    {
        if (missionTitle != null)
            missionTitle.text = "NUEVA TAREA";

        if (missionDescription != null)
        {
            missionDescription.text =
                $"Orden: {orderID}\n{productName}";
        }

        if (missionPallet != null)
            missionPallet.text = $"Tarima: {palletID}";

        if (missionOrigin != null)
            missionOrigin.text = $"Origen: {origin}";

        if (missionDestination != null)
            missionDestination.text = $"Destino: {destination}";

        if (missionCoroutine != null)
            StopCoroutine(missionCoroutine);

        missionCoroutine = StartCoroutine(
            ShowTemporarily(
                missionPanel,
                missionVisibleDuration));
    }

    #endregion

    #region NOTIFICATIONS

    public void ShowNotification(
        string title,
        string message)
    {
        ShowNotification(
            title,
            message,
            notificationVisibleDuration);
    }

    public void ShowNotification(
        string title,
        string message,
        float duration)
    {
        if (notificationTitle != null)
            notificationTitle.text = title;

        if (notificationMessage != null)
            notificationMessage.text = message;

        if (notificationCoroutine != null)
            StopCoroutine(notificationCoroutine);

        notificationCoroutine = StartCoroutine(
            ShowTemporarily(
                notificationPanel,
                duration));
    }

    #endregion

    #region SUMMARY

    public void ShowSummary(
        string palletID,
        string destination,
        float elapsedSeconds,
        int scanCount,
        int errorCount)
    {
        int minutes =
            Mathf.FloorToInt(elapsedSeconds / 60f);

        int seconds =
            Mathf.FloorToInt(elapsedSeconds % 60f);

        if (summaryTitle != null)
            summaryTitle.text = "OPERACIÓN COMPLETADA";

        if (summaryBody != null)
        {
            summaryBody.text =
                $"Tarima: {palletID}\n" +
                $"Destino: {destination}\n" +
                $"Tiempo: {minutes:00}:{seconds:00}\n" +
                $"Escaneos: {scanCount}\n" +
                $"Errores: {errorCount}";
        }

        if (summaryCoroutine != null)
            StopCoroutine(summaryCoroutine);

        summaryCoroutine = StartCoroutine(
            ShowPanel(summaryPanel));
    }

    public void HideSummary()
    {
        if (summaryCoroutine != null)
            StopCoroutine(summaryCoroutine);

        summaryCoroutine = StartCoroutine(
            HidePanel(summaryPanel));
    }

    #endregion

    #region ANIMATION

    private IEnumerator ShowTemporarily(
        CanvasGroup panel,
        float visibleDuration)
    {
        yield return ShowPanel(panel);

        yield return new WaitForSecondsRealtime(
            visibleDuration);

        yield return HidePanel(panel);
    }

    private IEnumerator ShowPanel(
        CanvasGroup panel)
    {
        if (panel == null)
            yield break;

        panel.gameObject.SetActive(true);
        panel.interactable = true;
        panel.blocksRaycasts = true;

        float elapsed = 0f;
        float initialAlpha = panel.alpha;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            panel.alpha = Mathf.Lerp(
                initialAlpha,
                1f,
                elapsed / fadeDuration);

            yield return null;
        }

        panel.alpha = 1f;
    }

    private IEnumerator HidePanel(
        CanvasGroup panel)
    {
        if (panel == null)
            yield break;

        float elapsed = 0f;
        float initialAlpha = panel.alpha;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            panel.alpha = Mathf.Lerp(
                initialAlpha,
                0f,
                elapsed / fadeDuration);

            yield return null;
        }

        HideImmediately(panel);
    }

    private void HideImmediately(
        CanvasGroup panel)
    {
        if (panel == null)
            return;

        panel.alpha = 0f;
        panel.interactable = false;
        panel.blocksRaycasts = false;
        panel.gameObject.SetActive(false);
    }

    #endregion
}