using UnityEngine;
using UnityEngine.InputSystem;

public class Scanner : MonoBehaviour
{
    [Header("Configuración")]
    public float scanDistance = 5f;


    [Header("Interfaz")]
    public ScannerUI scannerUI;


    [Header("Orden")]
    public OrderManager orderManager;


    private Pallet currentPallet;



    void Update()
    {
        DetectPallet();


        if (currentPallet != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ScanPallet();
        }
    }



    void DetectPallet()
    {
        Ray ray = new Ray(
            transform.position,
            transform.forward
        );


        Debug.DrawRay(
            transform.position,
            transform.forward * scanDistance,
            Color.green
        );


        RaycastHit hit;


        if (Physics.Raycast(ray, out hit, scanDistance))
        {
            Pallet pallet = hit.collider.GetComponentInParent<Pallet>();


            if (pallet != null)
            {
                currentPallet = pallet;


                scannerUI.ShowAction();


                return;
            }
        }


        currentPallet = null;


        scannerUI.Hide();
    }




    void ScanPallet()
    {
        scannerUI.HideAction();


        scannerUI.Show(currentPallet);


        Debug.Log(
            "Pallet escaneado: " 
            + currentPallet.productName
        );


        CompleteOrderItem();
    }




    void CompleteOrderItem()
    {
        if(orderManager == null)
        {
            Debug.LogError("OrderManager no asignado en Scanner");
            return;
        }



        foreach(OrderItem item in orderManager.currentOrder.items)
        {
            if(item.palletID == currentPallet.palletID)
            {
                if(item.scanned)
                {
                    Debug.Log("Este pallet ya fue escaneado");
                    return;
                }


                item.scanned = true;


                orderManager.currentOrder.completedProducts++;


                orderManager.orderUI.UpdateOrder(
                    orderManager.currentOrder
                );


                Debug.Log(
                    "Orden actualizada: "
                    + item.productName
                );


                return;
            }
        }



        Debug.Log(
            "El pallet " 
            + currentPallet.palletID 
            + " no pertenece a la orden"
        );
    }
}