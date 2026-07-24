using TMPro;
using UnityEngine;

public class OrderUI : MonoBehaviour
{
    public TMP_Text orderText;


    public void UpdateOrder(Order order)
    {
        if (order == null)
        {
            Debug.LogError("La orden recibida es NULL");
            return;
        }


        if (orderText == null)
        {
            Debug.LogError("orderText es NULL");
            return;
        }


        string text = "";


        text += "ORDEN #" + order.orderID + "\n\n";


        text += "Cliente: " + order.customer + "\n\n";


        text += "Productos\n\n";


        foreach (OrderItem item in order.items)
        {
            string check = item.scanned ? "[X] " : "[ ] ";

            text += check + item.productName + "\n";
        }


        text += "\n";


        text += "Escaneados: " +
                order.completedProducts +
                " / " +
                order.items.Count;
        if(order.completed)
        {
            text += "\n\nORDEN LISTA PARA ENTREGA";
        }

        // Aquí se actualiza el texto en pantalla
        orderText.text = text;


        Debug.Log("UI ACTUALIZADA:");
        Debug.Log(text);
    }
}