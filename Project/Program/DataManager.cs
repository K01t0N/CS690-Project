namespace Program;

using System.IO;
using System.Text.Json;

class DataManager
{
    EmployeeContainer employeeContainer;

    public DataManager() {
        this.employeeContainer = this.LoadEmployees();
    }

    public Employee GetOneEmployee(string employeeName) {
        return employeeContainer.employees.Find(x => x.name == employeeName);
    }

    public List<string> GetEmployees() {
        List<string> employeeNames = [];
        for (int i=0; i<this.employeeContainer.employees.Count; i++) {
            employeeNames.Add(this.employeeContainer.employees[i].name);
        }
        return employeeNames;
    }
    EmployeeContainer LoadEmployees() {
        string employeeData = File.ReadAllText("employees.json");
        try {
            return JsonSerializer.Deserialize<EmployeeContainer>(employeeData)!;
        } catch (JsonException e) {
            return JsonSerializer.Deserialize<EmployeeContainer>("{\"employees\":[]}")!;
        }
    }
    void SaveEmployees() {
        string jsonString = JsonSerializer.Serialize(this.employeeContainer);
        File.WriteAllText("employees.json", jsonString);
    }

}