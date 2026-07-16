using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class VehicleInteraction : MonoBehaviour
{
    #region PLAYER REFERENCES

    [Header("PLAYER")]
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private StarterAssetsInputs playerInputs;
    [SerializeField] private PlayerInput playerInput;

    #endregion

    #region REACHLIFT REFERENCES

    [Header("REACHLIFT")]
    [SerializeField] private ReachliftController reachliftController;
    [SerializeField] private ReachliftInput reachliftInput;
    [SerializeField] private PlayerInput reachliftPlayerInput;
    [SerializeField] private ReachliftCameraLook reachliftCameraLook;

    #endregion

    #region VEHICLE SYSTEMS

    [Header("VEHICLE SYSTEMS")]
    [SerializeField] private VehicleSeatController seatController;

    #endregion

    #region STATE

    private bool playerInsideTrigger;
    private bool driving;

    private bool waitForPlayerInteractRelease;
    private bool waitForReachliftInteractRelease;

    #endregion

    #region UNITY

    private void Update()
    {
        if (driving)
        {
            CheckVehicleInteraction();
        }
        else
        {
            CheckPlayerInteraction();
        }
    }

    #endregion

    #region INPUT

    private void CheckPlayerInteraction()
    {
        if (!playerInsideTrigger)
            return;

        if (waitForPlayerInteractRelease)
        {
            if (!playerInputs.interact)
            {
                waitForPlayerInteractRelease = false;
            }

            return;
        }

        if (!playerInputs.interact)
            return;

        playerInputs.interact = false;
        EnterVehicle();
    }

    private void CheckVehicleInteraction()
    {
        if (waitForReachliftInteractRelease)
        {
            if (!reachliftInput.interact)
            {
                waitForReachliftInteractRelease = false;
            }

            return;
        }

        if (!reachliftInput.interact)
            return;

        reachliftInput.interact = false;
        ExitVehicle();
    }

    #endregion

    #region ENTER VEHICLE

    private void EnterVehicle()
    {
        driving = true;

        playerInputs.interact = false;

        // Desactivar control del Player.
        playerController.enabled = false;
        playerInputs.enabled = false;
        playerInput.enabled = false;

        // Mover el Player completo a PlayerPivot.
        // Su PlayerCameraRoot permanece intacto.
        seatController.SitPlayer();

        // Activar control del Reachlift.
        reachliftInput.enabled = true;
        reachliftPlayerInput.enabled = true;
        reachliftController.enabled = true;
        reachliftCameraLook.enabled = true;

        waitForReachliftInteractRelease = true;

        Debug.Log("Entraste al Reachlift");
    }

    #endregion

    #region EXIT VEHICLE

    private void ExitVehicle()
    {
        driving = false;

        reachliftInput.interact = false;

        // Desactivar control del Reachlift.
        reachliftController.enabled = false;
        reachliftPlayerInput.enabled = false;
        reachliftInput.enabled = false;
        reachliftCameraLook.enabled = false;

        // Mover al Player completo hacia ExitPoint.
        seatController.ExitPlayer();

        // Reactivar control del Player.
        playerInputs.enabled = true;
        playerInput.enabled = true;
        playerController.enabled = true;

        waitForPlayerInteractRelease = true;

        Debug.Log("Saliste del Reachlift");
    }

    #endregion

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