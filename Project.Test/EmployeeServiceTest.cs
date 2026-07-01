namespace Project.Test;

using Program;

public class EmployeeServiceTest
{
    EmployeeData employeeData;
    EmployeeService employeeService;

    public EmployeeServiceTest() {
        this.employeeData = new EmployeeData();
        this.employeeService = new EmployeeService(this.employeeData);

        List<Employee> employees = employeeService.GetAll();
        for (int i=0; i<employees.Count; i++) {
            this.employeeService.Remove(employees[i].GetName());
        }
    }

    [Fact]
    public void GetAllTest() {
        string name1 = "John Doe";
        string name2 = "Jane Doe";
        Employee employee1 = new Employee(name1);
        Employee employee2 = new Employee(name2);
        this.employeeService.Add(name1);
        this.employeeService.Add(name2);
        List<Employee> employees = this.employeeService.GetAll();
        Assert.Equal(name1, employees[0].GetName());
        Assert.Equal(name2, employees[1].GetName());
        Assert.Equal(2, employees.Count);
        this.employeeService.Remove(name1);
        this.employeeService.Remove(name2);
    }

    [Fact]
    public void GetNamesTest() {
        string name1 = "John Doe";
        string name2 = "Jane Doe";
        this.employeeService.Add(name1);
        this.employeeService.Add(name2);
        List<string> employees = this.employeeService.GetNames();
        Assert.Contains(name1, employees);
        Assert.Contains(name2, employees);
        Assert.Equal(2, employees.Count);
        this.employeeService.Remove(name1);
        this.employeeService.Remove(name2);
    }

    [Fact]
    public void GetOne() {
        string name1 = "John Doe";
        this.employeeService.Add(name1);
        Employee employee = this.employeeService.GetOne(name1);
        Assert.Equal(name1, employee.GetName());
        this.employeeService.Remove(name1);
    }

    [Fact]
    public void Remove() {
        string name1 = "John Doe";
        this.employeeService.Add(name1);
        this.employeeService.Remove(name1);
        Assert.Empty(this.employeeService.GetAll());
    }

}