namespace Program;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

class EmployeeData
{
    [JsonInclude] private List<Employee> employees;

    public EmployeeData() {
        this.employees = this.Load();
    }

    public List<Employee> GetAll() {
        return this.employees;
    }

    public Employee GetOne(string name) {
        return this.employees.Find(x => x.GetName() == name);
    }

    public void Add(Employee e) {
        this.employees.Add(e);
    }

    public void Remove(string name) {
        this.employees.Remove(this.GetOne(name));
    }
    
    List<Employee> Load() {
        string orderImport = File.ReadAllText("employees.json");
        try {
            JsonNode dom = JsonNode.Parse(orderImport)!;
            JsonArray arr = dom!["employees"]!.AsArray()!;
            return JsonSerializer.Deserialize<List<Employee>>(arr)!;
        } catch (JsonException) {
            return JsonSerializer.Deserialize<List<Employee>>("[]")!;
        }
    }

    public void SaveOrders() {
        string jsonString = JsonSerializer.Serialize(this.employees);
        File.WriteAllText("employees.json", "{\"employees\":" + jsonString + "}");
    }

}