namespace Program;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

class OrderData
{
    [JsonInclude] List<Order> orders;

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
            JsonArray arr = dom!["orders"]!.AsArray()!;
            return JsonSerializer.Deserialize<List<Order>>(arr)!;
        } catch (JsonException e) {
            return JsonSerializer.Deserialize<List<Order>>("[]")!;
        }
    }
    public void SaveOrders() { // fix this
        string jsonString = JsonSerializer.Serialize(this.orders);
        File.WriteAllText("orders.json", "{\"orders\":" + jsonString + "}");
    }
    public void Add(Order order) {
        this.orders.Add(order);
    }
    public Order GetOne(int id) {
        return this.orders.Find(x => x.GetID() == id);
    }
    public void Remove(int id) {
        this.orders.Remove(this.GetOne(id));
    }


}