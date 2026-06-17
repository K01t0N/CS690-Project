namespace Program;

class EmployeeService
{
    private EmployeeData employeeData;

    /*
    methods:
    Get All
    Get Names (service layer only)
    Get One
    Add
    Remove
    Load (data layer only)
    Save (data layer only)
    */

    public EmployeeService(EmployeeData employeeData) {
        this.employeeData = employeeData;
    }

    public List<Employee> GetAll() {
        return this.employeeData.GetAll();
    }

    public List<string> GetNames() {
        List<string> names = [];
        List<Employee> employees = this.employeeData.GetAll();
        for (int i = 0; i < employees.Count; i++) {
            names.Add(employees[i].GetName());
        }
        return names;
    }

    public Employee GetOne(string name) {
        return this.employeeData.GetOne(name);
    }

    public void Add(string name) {
        Employee e = new Employee(name);
        this.employeeData.Add(e);
    }

    public void Remove(string name) {
        this.employeeData.Remove(name);
    }

    

}