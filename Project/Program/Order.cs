namespace Program;

using System.Text.Json;
using System.Text.Json.Serialization;

public class Order
{
    
    [JsonInclude] private int id;
    [JsonInclude] private string type;
    [JsonInclude] private string device;
    [JsonInclude] private string name;
    [JsonInclude] private List<Employee> employees;
    [JsonInclude] private string status;

    public Order(int id, string type, string device, string name, string status) {
        this.id = id;
        this.type = type;
        this.device = device;
        this.name = name;
        this.employees = [];
        this.status = status;
    }

    // getters and setters

    public int GetID() {
        return this.id;
    }
    public string GetTypeString() {
        return this.type;
    }
    public string GetDevice() {
        return this.device;
    }
    public string GetName() {
        return this.name;
    }
    public List<Employee> GetEmployees() { // deference of a possibly null reference
        return this.employees;
    }
    public string GetStatus() {
        return this.status;
    }

    public void SetType(string type) {
        this.type = type;
    }
    public void SetDevice(string device) {
        this.device = device;
    }
    public void SetEmployees(List<Employee> employees) {
        this.employees = employees;
    }
    public void SetStatus(string status) {
        this.status = status;
    }
    public void AddEmployee(Employee employee) {
        this.employees.Add(employee);
    }
    public void RemoveEmployee(Employee employee) {
        this.employees.Remove(employee);
    }

}