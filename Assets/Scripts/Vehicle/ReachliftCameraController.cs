using UnityEngine;

public class ReachliftCameraController : MonoBehaviour
{
    #region REFERENCES

    [Header("Camera Point")]
    [SerializeField] private Transform cameraPoint;

    #endregion

    #region STATE

    private Transform playerCameraRoot;
    private Transform originalParent;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private bool cameraMounted;

    #endregion

    #region PUBLIC METHODS

    public void EnterVehicle(Transform cameraRoot)
    {
        if (cameraMounted || cameraRoot == null)
            return;

        cameraMounted = true;
        playerCameraRoot = cameraRoot;

        // Guardar la jerarquía y transformación originales.
        originalParent = playerCameraRoot.parent;
        originalLocalPosition = playerCameraRoot.localPosition;
        originalLocalRotation = playerCameraRoot.localRotation;

        // Montar todo el rig visual en CameraPoint.
        playerCameraRoot.SetParent(cameraPoint, false);
        playerCameraRoot.localPosition = Vector3.zero;
        playerCameraRoot.localRotation = Quaternion.identity;
    }

    public void ExitVehicle()
    {
        if (!cameraMounted || playerCameraRoot == null)
            return;

        cameraMounted = false;

        // Regresar PlayerCameraRoot exactamente a su lugar original.
        playerCameraRoot.SetParent(originalParent, false);
        playerCameraRoot.localPosition = originalLocalPosition;
        playerCameraRoot.localRotation = originalLocalRotation;

        playerCameraRoot = null;
    }

    #endregion
}