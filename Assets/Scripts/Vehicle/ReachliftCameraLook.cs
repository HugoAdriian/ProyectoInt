using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ReachliftCameraLook : MonoBehaviour
{
    #region REFERENCES

    [Header("Referencias")]
    [SerializeField] private ReachliftInput input;

    [Tooltip("Asigna PlayerCapsule.")]
    [SerializeField] private Transform playerRoot;

    [Tooltip("Asigna PlayerCapsule/PlayerCameraRoot.")]
    [SerializeField] private Transform playerCameraRoot;

    #endregion

    #region SETTINGS

    [Header("Configuración")]
    [SerializeField] private float lookSensitivity = 1f;

    [Tooltip("Límite para mirar hacia arriba.")]
    [SerializeField] private float topClamp = 80f;

    [Tooltip("Límite para mirar hacia abajo.")]
    [SerializeField] private float bottomClamp = -70f;

    #endregion

    #region STATE

    private float pitch;
    private float yaw;

    #endregion

    #region UNITY

    private void OnEnable()
    {
        if (input == null)
            input = GetComponent<ReachliftInput>();

        pitch = NormalizeAngle(playerCameraRoot.localEulerAngles.x);
        yaw = NormalizeAngle(playerRoot.localEulerAngles.y);

        input?.ResetLookInput();
    }

    private void LateUpdate()
    {
        RotateCamera();
    }

    #endregion

    #region CAMERA

    private void RotateCamera()
    {
        if (input == null || playerRoot == null || playerCameraRoot == null)
            return;

        if (input.look.sqrMagnitude < 0.01f)
            return;

        float multiplier = IsMouseProvidingInput()
            ? 1f
            : Time.deltaTime;

        yaw += input.look.x * lookSensitivity * multiplier;
        pitch -= input.look.y * lookSensitivity * multiplier;

        pitch = Mathf.Clamp(pitch, bottomClamp, topClamp);

        // Giro horizontal del operador respecto al vehículo.
        playerRoot.localRotation = Quaternion.Euler(0f, yaw, 0f);

        // Giro vertical de la vista.
        playerCameraRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private static float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }

    private static bool IsMouseProvidingInput()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null &&
               Mouse.current.delta.ReadValue().sqrMagnitude > 0.01f;
#else
        return false;
#endif
    }

    #endregion
}