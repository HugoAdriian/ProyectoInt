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
            Pallet pallet =
                hit.collider
                    .GetComponentInParent<Pallet>();

            if (pallet != null)
            {
                currentPallet = pallet;

                scannerUI?.ShowAction();
                return;
            }

            StorageLocation location =
                hit.collider
                    .GetComponentInParent<StorageLocation>();

            if (location != null)
            {
                currentLocation = location;

                scannerUI?.ShowAction();
                return;
            }
        }

        scannerUI?.Hide();
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

        scannerUI?.ShowMessage(
            "Escaneo",
            "No se detectó un objeto válido.");

        Debug.Log(
            "No existe un objeto válido frente al escáner.");
    }

    private void ScanPallet()
    {
        scannerUI?.HideAction();
        scannerUI?.Show(currentPallet);

        Debug.Log(
            $"Tarima escaneada: {currentPallet.palletID}");

        if (orderManager == null)
        {
            Debug.LogError(
                "Scanner: OrderManager no está asignado.",
                this);

            return;
        }

        orderManager.ValidatePallet(
            currentPallet);
    }

    private void ScanLocation()
    {
        scannerUI?.HideAction();
        scannerUI?.Show(currentLocation);

        Debug.Log(
            $"Ubicación escaneada: {currentLocation.locationID}");

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