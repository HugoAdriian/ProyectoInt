using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class VehicleInteraction : MonoBehaviour
{
    #region PLAYER

    [Header("PLAYER")]
    [SerializeField] private Transform player;
    [SerializeField] private CharacterController playerCharacterController;
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private StarterAssetsInputs playerInputs;
    [SerializeField] private PlayerInput playerInput;

    #endregion

    #region REACHLIFT

    [Header("REACHLIFT")]
    [SerializeField] private ReachliftController reachliftController;
    [SerializeField] private ReachliftInput reachliftInput;
    [SerializeField] private PlayerInput reachliftPlayerInput;

    #endregion

    #region INTERACTION

    [Header("Interaction Points")]
    [SerializeField] private Transform seat;
    [SerializeField] private Transform exitPoint;

    #endregion

    #region STATE

    private bool playerInsideTrigger = false;
    private bool driving = false;

    private Transform originalParent;

    #endregion

    //========================================================

    #region UNITY

    private void Start()
    {
        originalParent = player.parent;
    }

    private void Update()
    {
        CheckPlayerInteraction();
        CheckVehicleInteraction();
    }

    #endregion

    //========================================================

    #region INPUT

    private void CheckPlayerInteraction()
    {
        if (driving)
            return;

        if (!playerInsideTrigger)
            return;

        if (playerInputs.interact)
        {
            playerInputs.interact = false;
            EnterVehicle();
        }
    }

    private void CheckVehicleInteraction()
    {
        if (!driving)
            return;

        if (reachliftInput.interact)
        {
            reachliftInput.interact = false;
            ExitVehicle();
        }
    }

    #endregion

    //========================================================

    #region ENTER VEHICLE

    private void EnterVehicle()
    {
        driving = true;

        //--------------------------------------------------
        // Desactivar CharacterController para mover Player
        //--------------------------------------------------

        playerCharacterController.enabled = false;

        //--------------------------------------------------
        // Mover al asiento
        //--------------------------------------------------

        player.position = seat.position;
        player.rotation = seat.rotation;

        //--------------------------------------------------
        // Hacer hijo del Seat
        //--------------------------------------------------

        player.SetParent(seat);

        //--------------------------------------------------
        // Reactivar CharacterController
        //--------------------------------------------------

        playerCharacterController.enabled = true;

        //--------------------------------------------------
        // Desactivar controles Player
        //--------------------------------------------------

        playerController.enabled = false;
        playerInputs.enabled = false;
        playerInput.enabled = false;

        //--------------------------------------------------
        // Activar Reachlift
        //--------------------------------------------------

        reachliftPlayerInput.enabled = true;
        reachliftInput.enabled = true;
        reachliftController.enabled = true;

        Debug.Log("Entraste al Reachlift");
    }

    #endregion

    //========================================================

    #region EXIT VEHICLE

    private void ExitVehicle()
    {
        driving = false;

        //--------------------------------------------------
        // Desactivar Reachlift
        //--------------------------------------------------

        reachliftController.enabled = false;
        reachliftInput.enabled = false;
        reachliftPlayerInput.enabled = false;

        //--------------------------------------------------
        // Desactivar CharacterController
        //--------------------------------------------------

        playerCharacterController.enabled = false;

        //--------------------------------------------------
        // Restaurar padre original
        //--------------------------------------------------

        player.SetParent(originalParent);

        //--------------------------------------------------
        // Colocar en punto de salida
        //--------------------------------------------------

        player.position = exitPoint.position;
        player.rotation = exitPoint.rotation;

        //--------------------------------------------------
        // Reactivar CharacterController
        //--------------------------------------------------

        playerCharacterController.enabled = true;

        //--------------------------------------------------
        // Reactivar controles Player
        //--------------------------------------------------

        playerInput.enabled = true;
        playerInputs.enabled = true;
        playerController.enabled = true;

        Debug.Log("Saliste del Reachlift");
    }

    #endregion

    //========================================================

    #region TRIGGER

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInsideTrigger = true;

        Debug.Log("Presiona E para conducir.");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInsideTrigger = false;
    }

    #endregion
}