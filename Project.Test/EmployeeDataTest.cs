namespace Project.Test;

using Program;

public class EmployeeDataTest
{
    EmployeeData employeeData;

    public EmployeeDataTest() {
        this.employeeData = new EmployeeData();

        List<Employee> employees = employeeData.GetAll();
        for (int i=0; i<employees.Count; i++) {
            this.employeeData.Remove(employees[i].GetName());
        }
    }

    [Fact]
    public void GetAllTest() {
        string name1 = "John Doe";
        string name2 = "Jane Doe";
        Employee employee1 = new Employee(name1);
        Employee employee2 = new Employee(name2);
        this.employeeData.Add(employee1);
        this.employeeData.Add(employee2);
        List<Employee> employees = this.employeeData.GetAll();
        Assert.Equal(name1, employees[0].GetName());
        Assert.Equal(name2, employees[1].GetName());
        Assert.Equal(2, employees.Count);
        this.employeeData.Remove(name1);
        this.employeeData.Remove(name2);
        Assert.Empty(employeeData.GetAll());
    }

    [Fact]
    public void GetOne() {
        string name = "John Doe";
        Employee before = new Employee(name);
        this.employeeData.Add(before);
        Employee after = this.employeeData.GetOne(name);
        Assert.Equal(name, after.GetName());
        this.employeeData.Remove(name);
    }

    [Fact]
    public void Remove() {
        string name = "John Doe";
        Employee employee = new Employee(name);
        this.employeeData.Add(employee);
        this.employeeData.Remove(name);
        Assert.Empty(this.employeeData.GetAll());
    }

    [Fact]
    public void SaveLoadTest() {
        string beforeName = "John Doe";
        Employee employee = new Employee(beforeName);
        this.employeeData.Add(employee);
        this.employeeData.SaveEmployees();
        this.employeeData.LoadEmployees();
        Employee loadedEmployee = this.employeeData.GetAll()[0];
        string afterName = loadedEmployee.GetName();
        Assert.Equal(beforeName, afterName);
    }

}