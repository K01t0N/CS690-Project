namespace Program;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

class OrderData
{
    [JsonInclude] List<Order> orders = [];

    public OrderData()
    {
        this.orders = this.LoadOrders();
    }

    public List<Order> GetOrders() {
        return this.orders;
    }

    List<Order> LoadOrders() {
        string orderImport = File.ReadAllText("orders.json");
        try {
            JsonNode dom = JsonNode.Parse(orderImport)!;
            JsonArray arr = dom["orders"].AsArray()!;
            return JsonSerializer.Deserialize<List<Order>>(arr)!;
        } catch (JsonException) {
            return JsonSerializer.Deserialize<List<Order>>("[]")!;
        }
    }
    public void SaveOrders() { // fix this
        string jsonString = JsonSerializer.Serialize(this.orders);
        File.WriteAllText("orders.json", "{\"orders\":" + jsonString + "}");
    }
    public void Add(Order order) {
        this.orders.Add(order);
        this.SaveOrders();
    }
    public Order GetOne(int id) {
        return this.orders.Find(x => x.GetID() == id);

    }
    public void Remove(Order order) {
        this.orders.Remove(order);
        this.SaveOrders();
    }
    public void UpdateOrderStatus(int id, string status) {
        // int index = this.orders.FindIndex(x => x.GetID() == id);
        // this.orders[index].status = status;
        this.orders.Find(x => x.GetID() == id).SetStatus(status);
        this.SaveOrders();
    }
    public void AddEmployee(int id, Employee employee) {
        this.orders.Find(x => x.GetID() == id).AddEmployee(employee);
        this.SaveOrders();
    }
    public void RemoveEmployee(int id, Employee employee) {
        this.orders.Find(x => x.GetID() == id).RemoveEmployee(employee);
        this.SaveOrders();
    }


}