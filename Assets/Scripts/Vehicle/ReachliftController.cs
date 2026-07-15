using UnityEngine;

public class ReachliftController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private ReachliftInput input;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 90f;

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (input == null)
            input = GetComponent<ReachliftInput>();
    }

    private void Update()
    {
        Move();
        Rotate();
        Actions();
    }

    //==========================
    // Movimiento
    //==========================

    private void Move()
    {
        Vector3 direction = transform.forward * input.move;

        controller.Move(direction * moveSpeed * Time.deltaTime);
    }

    private void Rotate()
    {
        transform.Rotate(
            Vector3.up,
            input.turn * turnSpeed * Time.deltaTime
        );
    }

    //==========================
    // Acciones
    //==========================

    private void Actions()
    {
        if (input.scan)
        {
            Debug.Log("Escaneando pallet...");
        }

        if (input.horn)
        {
            Debug.Log("Bocina");
        }

        if (input.liftUp)
        {
            Debug.Log("Subiendo horquillas");
        }

        if (input.liftDown)
        {
            Debug.Log("Bajando horquillas");
        }
    }
}