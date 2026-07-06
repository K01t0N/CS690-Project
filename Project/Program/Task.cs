namespace Program;

using System.Text.Json;
using System.Text.Json.Serialization;

public class Task
{

    [JsonInclude] private int index;
    [JsonInclude] private string text;
    [JsonInclude] private List<Employee> employees;
    [JsonInclude] private string status;

    public Task(int index, string text) {
        this.index = index;
        this.text = text;
        this.employees = [];
        this.status = "not started";
    }

    public int GetIndex() {
        return this.index;
    }
    public string GetText() {
        return this.text;
    }
    public List<Employee> GetEmployees() {
        return this.employees;
    }
    public string GetStatus() {
        return this.status;
    }
    public string GetEmployeeNames() {
        if (this.employees.Count > 0) {
            string names = "";
            for (int i=0; i<this.employees.Count; i++) {
                names += this.employees[i].GetName();
                names += ", ";
            }
            names.TrimEnd(", ");
            return names;
        } else {
            return "none";
        }
        
    }

    public void SetText(string text) {
        this.text = text;
    }
    public void SetEmployees(List<Employee> employees) {
        this.employees = employees;
    }
    public void AddEmployee(Employee employee) {
        this.employees.Add(employee);
    }
    public void RemoveEmployee(Employee employee) {
        this.employees.Remove(employee);
    }
    public void SetStatus(string status) {
        this.status = status;
    }
    public bool HasEmployee(Employee employee) {
        return this.employees.Find(x => x == employee) != null;
    }


}