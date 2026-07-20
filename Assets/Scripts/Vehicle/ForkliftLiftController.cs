using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ForkliftLiftController : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Objeto que contiene el sistema móvil de las horquillas.")]
    [SerializeField] private Transform liftSystem;

    [Header("Configuración")]
    [SerializeField] private float liftSpeed = 0.8f;

    [Tooltip("Cuánto puede bajar desde su posición inicial.")]
    [SerializeField] private float lowerTravel = 0f;

    [Tooltip("Cuánto puede subir desde su posición inicial.")]
    [SerializeField] private float upperTravel = 1.2f;

    private float initialLocalY;
    private float minimumLocalY;
    private float maximumLocalY;

    private void Awake()
    {
        if (liftSystem == null)
        {
            Debug.LogError(
                "ForkliftLiftController: no se asignó Lift System.",
                this);

            enabled = false;
            return;
        }

        // Guardar la posición inicial real del modelo.
        initialLocalY = liftSystem.localPosition.y;

        minimumLocalY = initialLocalY - lowerTravel;
        maximumLocalY = initialLocalY + upperTravel;
    }

    private void Update()
    {
        MoveLift();
    }

    private void MoveLift()
    {
        float direction = ReadLiftDirection();

        if (Mathf.Approximately(direction, 0f))
            return;

        Vector3 position = liftSystem.localPosition;

        float previousY = position.y;

        position.y += direction * liftSpeed * Time.deltaTime;
        position.y = Mathf.Clamp(
            position.y,
            minimumLocalY,
            maximumLocalY);

        liftSystem.localPosition = position;

        // Mostrar mensaje solamente cuando realmente cambió la posición.
        if (!Mathf.Approximately(previousY, position.y))
        {
            if (direction > 0f)
                Debug.Log($"Subiendo horquillas. Y: {position.y:F2}");
            else
                Debug.Log($"Bajando horquillas. Y: {position.y:F2}");
        }
    }

    private float ReadLiftDirection()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current == null)
            return 0f;

        bool liftUpPressed = Keyboard.current.rKey.isPressed;
        bool liftDownPressed = Keyboard.current.fKey.isPressed;

        if (liftUpPressed && !liftDownPressed)
            return 1f;

        if (liftDownPressed && !liftUpPressed)
            return -1f;
#endif

        return 0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (liftSystem == null || !Application.isPlaying)
            return;

        Gizmos.DrawWireSphere(
            liftSystem.parent.TransformPoint(
                new Vector3(
                    liftSystem.localPosition.x,
                    minimumLocalY,
                    liftSystem.localPosition.z)),
            0.08f);

        Gizmos.DrawWireSphere(
            liftSystem.parent.TransformPoint(
                new Vector3(
                    liftSystem.localPosition.x,
                    maximumLocalY,
                    liftSystem.localPosition.z)),
            0.08f);
    }
}