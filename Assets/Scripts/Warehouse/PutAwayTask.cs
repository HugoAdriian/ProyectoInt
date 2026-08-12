using UnityEngine;

[System.Serializable]
public class PutAwayTask
{
    [Header("Identificación de la tarea")]
    public string taskID = "PUT-001";

    [Header("Tarima")]
    public string palletID = "P001";
    public string productName = "Laptop ASUS";

    [Header("Ubicaciones")]
    public string originLocationID = "RECEIVING";
    public string destinationLocationID = "A1-L1";

    [Header("Referencia de destino")]
    public StorageLocation destinationLocation;
}