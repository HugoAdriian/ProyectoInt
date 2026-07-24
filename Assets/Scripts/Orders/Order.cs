using System.Collections.Generic;

[System.Serializable]
public class Order
{
    public string orderID;

    public string customer;

    // Ahora la orden contiene objetos OrderItem
    public List<OrderItem> items = new List<OrderItem>();

    public int completedProducts = 0;

    public bool completed = false;
}