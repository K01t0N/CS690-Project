namespace Program;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

public class OrderData
{
    [JsonInclude] private List<Order> orders;
    [JsonInclude] private double defaultDays;

    public OrderData() {
        this.LoadOrderData();
    }
    
    public List<Order> GetOrders() {
        return this.orders;
    }

    public Order GetOne(int id) {
        return this.orders.Find(x => x.GetID() == id);
    }

    public double GetDefaultDays() {
        return this.defaultDays;
    }

    public void Add(Order order) {
        this.orders.Add(order);
        this.SaveOrderData();
    }
    
    public void Remove(Order order) {
        this.orders.Remove(order);
        this.SaveOrderData();
    }

    public void UpdateOrderStatus(int id, string status) {
        this.orders.Find(x => x.GetID() == id).SetStatus(status);
        this.SaveOrderData();
    }

    public void UpdateTaskStatus(int id, int index, string status) {
        this.orders.Find(x => x.GetID() == id).GetOneTask(index).SetStatus(status);
        this.SaveOrderData();
    }

    public void AdjustDate(int id, DateTime date) {
        this.orders.Find(x => x.GetID() == id).SetDate(date);
        this.SaveOrderData();
    }

    public void AddTask(int id, int index, string text) {
        Task task = new Task(index, text);
        this.orders.Find(x => x.GetID() == id).AddTask(task);
        this.SaveOrderData();
    }

    public void SetNotes(int id, string notes) {
        this.orders.Find(x => x.GetID() == id).SetNotes(notes);
        this.SaveOrderData();
    } 

    public void AddEmployee(int id, Employee employee) {
        this.orders.Find(x => x.GetID() == id).AddEmployee(employee);
        this.SaveOrderData();
    }

    public void RemoveEmployee(int id, Employee employee) {
        this.orders.Find(x => x.GetID() == id).RemoveEmployee(employee);
        this.SaveOrderData();
    }

    public void LoadOrderData() {
        if (!File.Exists("orders.json")) {
            File.WriteAllText("orders.json", "{\"orders\":[],\"defaultDays\":3.0}");
        }
        string orderImport = File.ReadAllText("orders.json");
        try {
            JsonNode dom = JsonNode.Parse(orderImport)!;
            JsonArray newOrders = dom["orders"].AsArray()!;
            this.orders = JsonSerializer.Deserialize<List<Order>>(newOrders)!;
            JsonValue newDefaultDays = dom["defaultDays"].AsValue()!;
            this.defaultDays = JsonSerializer.Deserialize<Double>(newDefaultDays)!;
        } catch (JsonException) {
            this.orders = [];
            this.defaultDays = 3.0;
        }
    }
    
    public void SaveOrderData() {
        string ordersString = JsonSerializer.Serialize(this.orders);
        string defaultDaysString = JsonSerializer.Serialize(this.defaultDays);
        File.WriteAllText("orders.json", "{\"orders\":" + ordersString + ",\"defaultDays\":" + defaultDaysString +"}");
    }
}