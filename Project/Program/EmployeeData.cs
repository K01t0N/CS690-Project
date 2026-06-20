namespace Program;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

class EmployeeData
{
    [JsonInclude] private List<Employee> employees;

    public EmployeeData() {
        this.employees = this.LoadEmployees();
    }
    public List<Employee> GetAll() {
        return this.employees;
    }
    public Employee GetOne(string name) {
        return this.employees.Find(x => x.GetName() == name);
    }
    public void Add(Employee e) {
        this.employees.Add(e);
        this.employees.SaveEmployees();
    }
    public void Remove(string name) {
        this.employees.Remove(this.GetOne(name));
        this.employees.SaveEmployees();
    }
    List<Employee> LoadEmployees() {
        if (!File.Exists("employees.json")) {
            File.WriteAllText("employees.json", "{\"employees\":[]}");
        }
        string orderImport = File.ReadAllText("employees.json");
        try {
            JsonNode dom = JsonNode.Parse(orderImport)!;
            JsonArray arr = dom!["employees"]!.AsArray()!;
            return JsonSerializer.Deserialize<List<Employee>>(arr)!;
        } catch (JsonException) {
            return JsonSerializer.Deserialize<List<Employee>>("[]")!;
        }
    }
    void SaveEmployees() {
        string jsonString = JsonSerializer.Serialize(this.employees);
        File.WriteAllText("employees.json", "{\"employees\":" + jsonString + "}");
    }
}