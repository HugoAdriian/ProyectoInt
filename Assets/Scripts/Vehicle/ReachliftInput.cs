using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ReachliftInput : MonoBehaviour
{
    [Header("Movimiento")]
    public float move;
    public float turn;

    [Header("Cámara")]
    public Vector2 look;

    [Header("Acciones")]
    public bool liftUp;
    public bool liftDown;
    public bool scan;
    public bool horn;
    public bool interact;

#if ENABLE_INPUT_SYSTEM

    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<float>());
    }

    public void OnTurn(InputValue value)
    {
        TurnInput(value.Get<float>());
    }

    public void OnLook(InputValue value)
    {
        LookInput(value.Get<Vector2>());
    }

    public void OnLiftUp(InputValue value)
    {
        LiftUpInput(value.isPressed);
    }

    public void OnLiftDown(InputValue value)
    {
        LiftDownInput(value.isPressed);
    }

    public void OnScan(InputValue value)
    {
        ScanInput(value.isPressed);
    }

    public void OnHorn(InputValue value)
    {
        HornInput(value.isPressed);
    }

    public void OnInteract(InputValue value)
    {
        InteractInput(value.isPressed);
    }

#endif

    public void MoveInput(float value)
    {
        move = value;
    }

    public void TurnInput(float value)
    {
        turn = value;
    }

    public void LookInput(Vector2 value)
    {
        look = value;
    }

    public void LiftUpInput(bool value)
    {
        liftUp = value;
    }

    public void LiftDownInput(bool value)
    {
        liftDown = value;
    }

    public void ScanInput(bool value)
    {
        scan = value;
    }

    public void HornInput(bool value)
    {
        horn = value;
    }

    public void InteractInput(bool value)
    {
        interact = value;
    }

    public void ResetLookInput()
    {
        look = Vector2.zero;
    }
}