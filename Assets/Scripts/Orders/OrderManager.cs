using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public Order currentOrder;

    public OrderUI orderUI;


    void Start()
    {
        Debug.Log("===== INICIANDO ORDER MANAGER =====");


        if (orderUI == null)
        {
            Debug.LogError("orderUI es NULL");
            return;
        }


        Debug.Log("orderUI encontrado");


        if (orderUI.orderText == null)
        {
            Debug.LogError("orderText es NULL");
        }
        else
        {
            Debug.Log("orderText encontrado");
        }


        CreateTestOrder();
    }



    void CreateTestOrder()
    {
        currentOrder = new Order();


        currentOrder.orderID = "0001";
        currentOrder.customer = "Amazon";


        currentOrder.items = new List<OrderItem>()
        {
            new OrderItem()
            {
                palletID = "P001",
                productName = "Laptop ASUS",
                scanned = false
            },


            new OrderItem()
            {
                palletID = "P002",
                productName = "Mouse Logitech",
                scanned = false
            },


            new OrderItem()
            {
                palletID = "P003",
                productName = "Monitor LG",
                scanned = false
            }
        };


        Debug.Log("Orden creada correctamente");

        Debug.Log(
            "Cantidad de productos: " + currentOrder.items.Count
        );


        if (orderUI != null)
        {
            Debug.Log("Enviando orden a OrderUI");

            orderUI.UpdateOrder(currentOrder);

            Debug.Log("Orden enviada correctamente");
        }
        else
        {
            Debug.LogError("No se puede actualizar UI porque orderUI es NULL");
        }
    }
}