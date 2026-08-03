using UnityEngine;
using UnityEngine.InputSystem;

public class PalletLoadController : MonoBehaviour
{
    #region REFERENCES

    [Header("Referencias")]
    [SerializeField] private Transform palletHoldPoint;

    [Header("Operación de almacén")]
    [SerializeField] private OrderManager orderManager;

    #endregion

    #region CARRIED TRANSFORM

    [Header("Ajuste de carga")]
    [SerializeField]
    private Vector3 carriedLocalPosition =
        Vector3.zero;

    [SerializeField]
    private Vector3 carriedLocalRotation =
        Vector3.zero;

    #endregion

    #region STATE

    private Rigidbody detectedPallet;
    private Rigidbody carriedPallet;

    private bool originalUseGravity;
    private bool originalIsKinematic;
    private bool originalDetectCollisions;

    private RigidbodyInterpolation originalInterpolation;
    private CollisionDetectionMode originalCollisionDetection;
    private RigidbodyConstraints originalConstraints;

    #endregion

    #region UNITY

    private void Update()
    {
        HandleGrabInput();
    }

    private void LateUpdate()
    {
        KeepPalletLockedToForks();
    }

    #endregion

    #region INPUT

    private void HandleGrabInput()
    {
        if (Keyboard.current == null)
            return;

        if (!Keyboard.current.gKey.wasPressedThisFrame)
            return;

        if (carriedPallet == null)
        {
            TryGrabPallet();
        }
        else
        {
            ReleasePallet();
        }
    }

    #endregion

    #region GRAB

    private void TryGrabPallet()
    {
        if (detectedPallet == null)
        {
            Debug.Log(
                "No existe una tarima dentro del detector.",
                this);

            return;
        }

        if (palletHoldPoint == null)
        {
            Debug.LogError(
                "PalletLoadController: PalletHoldPoint no está asignado.",
                this);

            return;
        }

        carriedPallet = detectedPallet;

        SaveOriginalRigidbodySettings();

        carriedPallet.linearVelocity = Vector3.zero;
        carriedPallet.angularVelocity = Vector3.zero;

        carriedPallet.useGravity = false;
        carriedPallet.isKinematic = true;
        carriedPallet.detectCollisions = false;

        carriedPallet.interpolation =
            RigidbodyInterpolation.None;

        carriedPallet.collisionDetectionMode =
            CollisionDetectionMode.Discrete;

        carriedPallet.transform.SetParent(
            palletHoldPoint,
            false);

        SetCarriedTransform();

        Pallet palletData =
            carriedPallet.GetComponent<Pallet>();

        if (palletData != null)
        {
            palletData.isPicked = true;
            palletData.delivered = false;
        }

        Debug.Log(
            $"Tarima tomada: {carriedPallet.name}",
            carriedPallet);
    }

    private void KeepPalletLockedToForks()
    {
        if (carriedPallet == null ||
            palletHoldPoint == null)
        {
            return;
        }

        if (carriedPallet.transform.parent !=
            palletHoldPoint)
        {
            carriedPallet.transform.SetParent(
                palletHoldPoint,
                false);
        }

        SetCarriedTransform();

        carriedPallet.linearVelocity = Vector3.zero;
        carriedPallet.angularVelocity = Vector3.zero;
    }

    private void SetCarriedTransform()
    {
        carriedPallet.transform.localPosition =
            carriedLocalPosition;

        carriedPallet.transform.localRotation =
            Quaternion.Euler(carriedLocalRotation);
    }

    #endregion

    #region RELEASE

    private void ReleasePallet()
    {
        if (carriedPallet == null)
            return;

        Rigidbody releasedRigidbody =
            carriedPallet;

        Pallet releasedPallet =
            releasedRigidbody.GetComponent<Pallet>();

        // Quitarla temporalmente de las cuchillas.
        releasedRigidbody.transform.SetParent(
            null,
            true);

        releasedRigidbody.linearVelocity =
            Vector3.zero;

        releasedRigidbody.angularVelocity =
            Vector3.zero;

        carriedPallet = null;

        //--------------------------------------------------
        // Intentar completar entrega validada
        //--------------------------------------------------

        if (orderManager != null &&
            releasedPallet != null &&
            orderManager.ConfirmDelivery(releasedPallet))
        {
            Debug.Log(
                $"Tarima depositada correctamente: " +
                $"{releasedPallet.palletID}");

            return;
        }

        //--------------------------------------------------
        // Liberación normal si no hay destino válido
        //--------------------------------------------------

        RestoreOriginalRigidbodySettings(
            releasedRigidbody);

        releasedRigidbody.isKinematic = false;
        releasedRigidbody.useGravity = true;
        releasedRigidbody.detectCollisions = true;

        if (releasedPallet != null)
        {
            releasedPallet.isPicked = false;
            releasedPallet.delivered = false;
        }

        Debug.Log(
            $"Tarima liberada sin destino validado: " +
            $"{releasedRigidbody.name}",
            releasedRigidbody);
    }

    #endregion

    #region RIGIDBODY SETTINGS

    private void SaveOriginalRigidbodySettings()
    {
        originalUseGravity =
            carriedPallet.useGravity;

        originalIsKinematic =
            carriedPallet.isKinematic;

        originalDetectCollisions =
            carriedPallet.detectCollisions;

        originalInterpolation =
            carriedPallet.interpolation;

        originalCollisionDetection =
            carriedPallet.collisionDetectionMode;

        originalConstraints =
            carriedPallet.constraints;
    }

    private void RestoreOriginalRigidbodySettings(
        Rigidbody palletRigidbody)
    {
        palletRigidbody.useGravity =
            originalUseGravity;

        palletRigidbody.isKinematic =
            originalIsKinematic;

        palletRigidbody.detectCollisions =
            originalDetectCollisions;

        palletRigidbody.interpolation =
            originalInterpolation;

        palletRigidbody.collisionDetectionMode =
            originalCollisionDetection;

        palletRigidbody.constraints =
            originalConstraints;
    }

    #endregion

    #region DETECTOR

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody palletRigidbody =
            GetPalletRigidbody(other);

        if (palletRigidbody == null)
            return;

        detectedPallet = palletRigidbody;

        Debug.Log(
            $"Tarima detectada: {detectedPallet.name}",
            detectedPallet);
    }

    private void OnTriggerStay(Collider other)
    {
        if (detectedPallet != null)
            return;

        Rigidbody palletRigidbody =
            GetPalletRigidbody(other);

        if (palletRigidbody != null)
        {
            detectedPallet = palletRigidbody;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody exitingPallet =
            GetPalletRigidbody(other);

        if (exitingPallet == null)
            return;

        if (exitingPallet == carriedPallet)
            return;

        if (exitingPallet == detectedPallet)
        {
            detectedPallet = null;

            Debug.Log(
                "Tarima fuera del detector.");
        }
    }

    private Rigidbody GetPalletRigidbody(
        Collider other)
    {
        Rigidbody palletRigidbody =
            other.attachedRigidbody;

        if (palletRigidbody == null)
            return null;

        if (!palletRigidbody.CompareTag("Pallet"))
            return null;

        return palletRigidbody;
    }

    #endregion
}