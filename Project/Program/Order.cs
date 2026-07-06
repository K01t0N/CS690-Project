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
    [JsonInclude] private DateTime date;
    [JsonInclude] private List<Task> tasks;

    public Order(int id, string type, string device, string name, string status) {
        this.id = id;
        this.type = type;
        this.device = device;
        this.name = name;
        this.employees = [];
        this.status = status;
        this.date = default;
        this.tasks = [];
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
    public List<Employee> GetEmployees() {
        return this.employees;
    }
    public string GetStatus() {
        return this.status;
    }
    public DateTime GetDate() {
        return this.date;
    }
    public List<Task> GetTasks() {
        return this.tasks;
    }
    public Task GetOneTask(int index) {
        return this.tasks.Find(x => x.GetIndex() == index);
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
    public void SetDate(DateTime date) {
        this.date = date;
    }
    public void AddEmployee(Employee employee) {
        this.employees.Add(employee);
    }
    public void RemoveEmployee(Employee employee) {
        this.employees.Remove(employee);
    }
    public void SetTasks(List<Task> tasks) {
        this.tasks = tasks;
    }
    public void SetOneTask(int index, string status) {
        this.tasks.Find(x => x.GetIndex() == index).SetStatus(status);
    }
    public void AddTask(Task task) {
        this.tasks.Add(task);
    }
    public void RemoveTask(Task task) {
        this.tasks.Remove(task);
    }
    public void AddEmployeeToTask(int index, Employee employee) {
        this.tasks.Find(x => x.GetIndex() == index).AddEmployee(employee);
    }
    public void RemoveEmployeeFromTask(int index, Employee employee) {
        this.tasks.Find(x => x.GetIndex() == index).RemoveEmployee(employee);
    }

}