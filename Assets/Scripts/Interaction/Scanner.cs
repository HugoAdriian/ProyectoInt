using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Scanner : MonoBehaviour
{
    #region REFERENCES

    [Header("Interfaz")]
    [SerializeField] private ScannerUI scannerUI;

    [Header("Orden")]
    [SerializeField] private OrderManager orderManager;

    #endregion

    #region SETTINGS

    [Header("Configuración")]
    [SerializeField] private float scanDistance = 10f;

    #endregion

    #region STATE

    private Pallet currentPallet;
    private StorageLocation currentLocation;

    #endregion

    #region UNITY

    private void Update()
    {
        DetectScannableObject();

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ScanCurrentObject();
        }
    }

    #endregion

    #region DETECTION

    private void DetectScannableObject()
    {
        Ray ray = new Ray(
            transform.position,
            transform.forward);

        Debug.DrawRay(
            transform.position,
            transform.forward * scanDistance,
            Color.green);

        RaycastHit[] hits =
            Physics.RaycastAll(
                ray,
                scanDistance);

        Array.Sort(
            hits,
            (firstHit, secondHit) =>
                firstHit.distance.CompareTo(
                    secondHit.distance));

        currentPallet = null;
        currentLocation = null;

        foreach (RaycastHit hit in hits)
        {
            //------------------------------------------
            // PALLET
            //------------------------------------------

            Pallet pallet =
                hit.collider.GetComponentInParent<Pallet>();

            if (pallet != null)
            {
                currentPallet = pallet;

                // Mostrar información inmediatamente.
                scannerUI?.ShowPalletInfo(
                    currentPallet);

                // Mostrar opción de escaneo.
                scannerUI?.ShowAction();

                return;
            }

            //------------------------------------------
            // STORAGE LOCATION
            //------------------------------------------

            StorageLocation location =
                hit.collider
                    .GetComponentInParent<StorageLocation>();

            if (location != null)
            {
                currentLocation = location;

                // No mostrar datos de ubicación
                // dentro del ScanPanel.
                scannerUI?.HidePalletInfo();

                // Pero sí permitir escanear.
                scannerUI?.ShowAction();

                return;
            }
        }

        //----------------------------------------------
        // Nada detectable
        //----------------------------------------------

        scannerUI?.HidePalletInfo();
        scannerUI?.HideAction();
    }

    #endregion

    #region SCANNING

    private void ScanCurrentObject()
    {
        if (currentPallet != null)
        {
            ScanPallet();
            return;
        }

        if (currentLocation != null)
        {
            ScanLocation();
            return;
        }

        Debug.Log(
            "No existe un objeto válido frente al escáner.");
    }

    private void ScanPallet()
    {
        if (currentPallet == null)
            return;

        // La información ya está visible.
        // No necesitamos volver a modificar el HUD.

        Debug.Log(
            $"Tarima escaneada: {currentPallet.palletID}");

        if (orderManager == null)
        {
            Debug.LogError(
                "Scanner: OrderManager no está asignado.",
                this);

            return;
        }

        //--------------------------------------------------
        // OrderManager decidirá si es correcta o incorrecta
        // y enviará el resultado a NotificationPanel.
        //--------------------------------------------------

        orderManager.ValidatePallet(
            currentPallet);
    }

    private void ScanLocation()
    {
        if (currentLocation == null)
            return;

        Debug.Log(
            $"Ubicación escaneada: " +
            $"{currentLocation.locationID}");

        if (orderManager == null)
        {
            Debug.LogError(
                "Scanner: OrderManager no está asignado.",
                this);

            return;
        }

        orderManager.ValidateDestination(
            currentLocation);
    }

    #endregion
}