using UnityEngine;

public class VehicleSeatController : MonoBehaviour
{
    #region REFERENCES

    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private CharacterController playerCharacterController;

    [Header("Interaction Points")]
    [SerializeField] private Transform playerPivot;
    [SerializeField] private Transform exitPoint;

    #endregion

    #region STATE

    private Transform originalParent;
    private bool playerIsSeated;

    #endregion

    #region UNITY

    private void Awake()
    {
        if (player != null)
        {
            originalParent = player.parent;
        }
    }

    #endregion

    #region PUBLIC METHODS

    public void SitPlayer()
    {
        if (playerIsSeated)
            return;

        playerIsSeated = true;

        // Evita conflictos al colocar un CharacterController
        // dentro del vehículo.
        playerCharacterController.enabled = false;

        // El PlayerCapsule queda anclado al PlayerPivot.
        player.SetParent(playerPivot, false);
        player.localPosition = Vector3.zero;
        player.localRotation = Quaternion.identity;
    }

    public void ExitPlayer()
    {
        if (!playerIsSeated)
            return;

        playerIsSeated = false;

        // Recuperar la jerarquía original del Player.
        player.SetParent(originalParent, true);

        // Colocarlo junto al Reachlift.
        player.position = exitPoint.position;
        player.rotation = exitPoint.rotation;

        // El CharacterController solo se reactiva al bajar.
        playerCharacterController.enabled = true;
    }

    #endregion
}