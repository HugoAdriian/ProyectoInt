using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PalletLoadController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform palletHoldPoint;

    [Header("Ajuste de carga")]
    [Tooltip("Desplazamiento local adicional respecto a PalletHoldPoint.")]
    [SerializeField] private Vector3 carriedLocalPosition = Vector3.zero;

    [Tooltip("Rotación local adicional respecto a PalletHoldPoint.")]
    [SerializeField] private Vector3 carriedLocalRotation = Vector3.zero;

    private Rigidbody detectedPallet;
    private Rigidbody carriedPallet;

    // Configuración original del Rigidbody.
    private bool originalUseGravity;
    private bool originalIsKinematic;
    private bool originalDetectCollisions;
    private RigidbodyInterpolation originalInterpolation;
    private CollisionDetectionMode originalCollisionDetection;
    private RigidbodyConstraints originalConstraints;

    private void Update()
    {
        HandleGrabInput();
    }

    private void LateUpdate()
    {
        KeepPalletLockedToForks();
    }

    private void HandleGrabInput()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current == null)
            return;

        if (!Keyboard.current.gKey.wasPressedThisFrame)
            return;

        if (carriedPallet == null)
            TryGrabPallet();
        else
            ReleasePallet();
#endif
    }

    private void TryGrabPallet()
    {
        if (detectedPallet == null)
        {
            Debug.Log(
                "No hay una tarima dentro del detector.",
                this);

            return;
        }

        if (palletHoldPoint == null)
        {
            Debug.LogError(
                "No se asignó Pallet Hold Point.",
                this);

            return;
        }

        carriedPallet = detectedPallet;

        SaveOriginalRigidbodySettings();

        // Detener cualquier movimiento físico anterior.
        carriedPallet.linearVelocity = Vector3.zero;
        carriedPallet.angularVelocity = Vector3.zero;

        // Mientras se transporta, la física no debe desplazarla.
        carriedPallet.useGravity = false;
        carriedPallet.isKinematic = true;
        carriedPallet.detectCollisions = false;
        carriedPallet.interpolation = RigidbodyInterpolation.None;
        carriedPallet.collisionDetectionMode =
            CollisionDetectionMode.Discrete;

        // Hacerla hija del punto de carga.
        carriedPallet.transform.SetParent(
            palletHoldPoint,
            false);

        SetCarriedTransform();

        Debug.Log(
            $"Tarima tomada y asegurada: {carriedPallet.name}",
            carriedPallet);
    }

    private void KeepPalletLockedToForks()
    {
        if (carriedPallet == null || palletHoldPoint == null)
            return;

        // Por seguridad, confirmar que siga dentro de PalletHoldPoint.
        if (carriedPallet.transform.parent != palletHoldPoint)
        {
            carriedPallet.transform.SetParent(
                palletHoldPoint,
                false);
        }

        // Impedir cualquier desplazamiento o giro durante el transporte.
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

    private void ReleasePallet()
    {
        if (carriedPallet == null)
            return;

        Rigidbody releasedPallet = carriedPallet;

        // Mantener la posición mundial actual al retirarla del Reachlift.
        releasedPallet.transform.SetParent(null, true);

        releasedPallet.linearVelocity = Vector3.zero;
        releasedPallet.angularVelocity = Vector3.zero;

        RestoreOriginalRigidbodySettings(releasedPallet);

        // Al liberarla queremos que vuelva a tener física.
        releasedPallet.isKinematic = false;
        releasedPallet.useGravity = true;
        releasedPallet.detectCollisions = true;

        carriedPallet = null;

        Debug.Log(
            $"Tarima liberada: {releasedPallet.name}",
            releasedPallet);
    }

    private void SaveOriginalRigidbodySettings()
    {
        originalUseGravity = carriedPallet.useGravity;
        originalIsKinematic = carriedPallet.isKinematic;
        originalDetectCollisions = carriedPallet.detectCollisions;
        originalInterpolation = carriedPallet.interpolation;
        originalCollisionDetection =
            carriedPallet.collisionDetectionMode;
        originalConstraints = carriedPallet.constraints;
    }

    private void RestoreOriginalRigidbodySettings(
        Rigidbody palletRigidbody)
    {
        palletRigidbody.useGravity = originalUseGravity;
        palletRigidbody.isKinematic = originalIsKinematic;
        palletRigidbody.detectCollisions =
            originalDetectCollisions;
        palletRigidbody.interpolation =
            originalInterpolation;
        palletRigidbody.collisionDetectionMode =
            originalCollisionDetection;
        palletRigidbody.constraints =
            originalConstraints;
    }

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
            detectedPallet = palletRigidbody;
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
            Debug.Log("Tarima fuera del detector.");
        }
    }

    private Rigidbody GetPalletRigidbody(Collider other)
    {
        Rigidbody palletRigidbody =
            other.attachedRigidbody;

        if (palletRigidbody == null)
            return null;

        if (!palletRigidbody.CompareTag("Pallet"))
            return null;

        return palletRigidbody;
    }
}