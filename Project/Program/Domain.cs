namespace Program;

using System.Text.Json;
using System.Text.Json.Serialization;

public class Order
{
    [JsonInclude] public int id {get;}
    [JsonInclude] public string type {get; set;}
    [JsonInclude] public string device {get; set;}
    [JsonInclude] public string name {get; set;}
    [JsonInclude] public List<Employee> employees {get; set;}
    [JsonInclude] public string status {get; set;}

    public Order(int id, string type, string device, string name, string status) {
        this.id = id;
        this.type = type;
        this.device = device;
        this.name = name;
        this.employees = [];
        this.status = status;
    }

}
public class EmployeeContainer
{
    [JsonInclude] public List<Employee> employees {get; set;}

    public void Add(Employee employee) {
        this.employees.Add(employee);
    }
    public Employee GetOne(string name) {
        return this.employees.Find(x => x.name == name);
    }
    public void Remove(string name) {
        this.employees.Remove(this.GetOne(name));
    }
}
public class Employee
{
    [JsonInclude] public string name {get;}

    public Employee(string name) {
        this.name = name;
    }
}