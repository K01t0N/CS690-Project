namespace Program;

class Program
{
    static void Main(string[] args)
    {
        OrderData orderData = new OrderData();
        EmployeeData employeeData = new EmployeeData();

        Random rand = new Random();
        OrderService orderService = new OrderService(orderData, rand);
        EmployeeService employeeService = new EmployeeService(employeeData);

        UI ui = new UI(orderService, employeeService);
        ui.Start();
    }
}
