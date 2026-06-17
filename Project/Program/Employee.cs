namespace Program;

using System.Text.Json;
using System.Text.Json.Serialization;

public class Employee
{
    [JsonInclude] private string name;

    public Employee(string name) {
        this.name = name;
    }

    // getters and setters

    public string GetName() {
        return this.name;
    }
    public void SetName(string name) {
        this.name = name;
    }

}