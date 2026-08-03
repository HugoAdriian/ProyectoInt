using UnityEngine;

public class StorageLocation : MonoBehaviour
{
    #region DATA

    [Header("Identificación")]
    public string locationID = "A1-L1";
    public string rack = "A1";
    public int level = 1;

    [Header("Estado")]
    public bool isOccupied;
    public Pallet currentPallet;

    [Header("Punto de colocación")]
    [SerializeField] private Transform placementPoint;

    [Header("Resaltado visual")]
    [SerializeField] private DestinationHighlighter highlighter;

    #endregion

    #region PUBLIC METHODS

    public bool CanReceivePallet()
    {
        return !isOccupied && currentPallet == null;
    }

    public void HighlightAsDestination()
    {
        if (highlighter != null)
            highlighter.ShowPending();
    }

    public void MarkAsValidated()
    {
        if (highlighter != null)
            highlighter.ShowValidated();
    }

    public void HideHighlight()
    {
        if (highlighter != null)
            highlighter.Hide();
    }

    public void PlaceAndStorePallet(Pallet pallet)
    {
        if (pallet == null)
        {
            Debug.LogError(
                $"StorageLocation {locationID}: la tarima recibida es nula.",
                this);

            return;
        }

        if (placementPoint == null)
        {
            Debug.LogError(
                $"StorageLocation {locationID}: PlacementPoint no está asignado.",
                this);

            return;
        }

        Rigidbody palletRigidbody =
            pallet.GetComponent<Rigidbody>();

        if (palletRigidbody != null)
        {
            palletRigidbody.linearVelocity = Vector3.zero;
            palletRigidbody.angularVelocity = Vector3.zero;

            palletRigidbody.useGravity = false;
            palletRigidbody.isKinematic = true;
            palletRigidbody.detectCollisions = true;
        }

        pallet.transform.SetParent(
            placementPoint,
            false);

        pallet.transform.localPosition =
            Vector3.zero;

        pallet.transform.localRotation =
            Quaternion.identity;

        currentPallet = pallet;
        isOccupied = true;

        pallet.rack = rack;
        pallet.level = level;
        pallet.delivered = true;
        pallet.isPicked = false;

        HideHighlight();

        Debug.Log(
            $"Tarima {pallet.palletID} almacenada y asegurada en {locationID}.",
            this);
    }

    public Pallet RemovePallet()
    {
        Pallet removedPallet = currentPallet;

        if (removedPallet != null)
        {
            removedPallet.transform.SetParent(
                null,
                true);

            removedPallet.delivered = false;

            Debug.Log(
                $"Tarima {removedPallet.palletID} retirada de {locationID}.",
                this);
        }

        currentPallet = null;
        isOccupied = false;

        return removedPallet;
    }

    #endregion
}