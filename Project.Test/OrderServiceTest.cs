namespace Project.Test;

using Program;

public class OrderServiceTest
{
    OrderData orderData;
    Random rand;
    OrderService orderService;
    EmployeeData employeeData;
    EmployeeService employeeService;

    public OrderServiceTest() {
        this.orderData = new OrderData();
        this.rand = new Random();
        this.orderService = new OrderService(this.orderData, rand);
        this.employeeData = new EmployeeData();
        this.employeeService = new EmployeeService(this.employeeData);

        List<Order> orders = orderService.GetAll();
        for (int i=0; i<orders.Count; i++) {
            this.orderService.RemoveOrderManager(orders[i]);
        }
        List<Employee> employees = employeeService.GetAll();
        for (int i=0; i<employees.Count; i++) {
            this.employeeService.Remove(employees[i].GetName());
        }

    }

    [Fact]
    public void GetAllTest()
    {
        Console.Write("\nnumber of orders: " + this.orderService.GetAll() + "\n");
        int id1 = this.orderService.NewOrder("Stability", "Macbook", "John Doe");
        int id2 = this.orderService.NewOrder("Screen", "PC Laptop", "Jane Doe");
        List<Order> all = this.orderService.GetAll();
        Order order1 = this.orderService.GetOneOrder(id1);
        Order order2 = this.orderService.GetOneOrder(id2);
        Assert.Contains(order1, all);
        Assert.Contains(order2, all);
        Assert.Equal(2, all.Count);
        this.orderService.RemoveOrderManager(this.orderService.GetOneOrder(id1));
        this.orderService.RemoveOrderManager(this.orderService.GetOneOrder(id2));
    }

    [Fact]
    public void GetRequestsOrdersTest() // Tests for requests and orders
    {
        int id1 = this.orderService.NewOrder("Stability", "Macbook", "John Doe");
        int id2 = this.orderService.NewOrder("Screen", "PC Laptop", "Jane Doe");
        int id3 = this.orderService.NewOrder("Performance", "PC Desktop", "Another Person");
        this.orderService.ApproveRequest(id2);
        this.orderService.ApproveRequest(id3);
        List<Order> orders = this.orderService.GetOrders();
        List<Order> requests = this.orderService.GetRequests();
        Order order1 = this.orderService.GetOneOrder(id1);
        Order order2 = this.orderService.GetOneOrder(id2);
        Order order3 = this.orderService.GetOneOrder(id3);
        Assert.Contains(order1, requests);
        Assert.Contains(order2, orders);
        Assert.Contains(order3, orders);
        Assert.Single(requests);
        Assert.Equal(2, orders.Count);
        this.orderService.RemoveOrderManager(order1);
        this.orderService.RemoveOrderManager(order2);
        this.orderService.RemoveOrderManager(order3);
    }

    // GetOneOrder()
    // GetOrderStrings()
    // GetRequestStrings()
    // static CompareDates, CompareIDs, CompareNames
    // SuggestOrderDate()

    [Fact]
    public void NewOrderTest()
    {
        int id = this.orderService.NewOrder("Stability", "Macbook", "John Doe");
        Order order = this.orderService.GetOneOrder(id);
        Assert.Equal(id, order.GetID());
        Assert.Equal("Stability", order.GetTypeString());
        Assert.Equal("Macbook", order.GetDevice());
        Assert.Equal("John Doe", order.GetName());
        Assert.Equal("request", order.GetStatus());
        Assert.Empty(order.GetEmployees());
        this.orderService.RemoveOrderManager(order);
    }

    [Fact]
    public void AdjustDateTest() {
        int id = this.orderService.NewOrder("Stability", "Macbook", "John Doe");
        Order order = this.orderService.GetOneOrder(id);
        this.orderService.ApproveRequest(id);
        DateTime newDate = new DateTime(2026, 8, 1);
        this.orderService.AdjustDate(id, newDate);
        Assert.Equal(newDate, this.orderService.GetOneOrder(id).GetDate());
        this.orderService.RemoveOrderManager(order);
    }

    [Fact]
    public void NewIDTest() {
        List<int> orderIDs = [];
        for (int i=0; i<50; i++) {
            int orderID = this.orderService.NewOrder("Screen", "Macbook", "John Doe");
            if (!orderIDs.Exists(x => x == orderID)) {
                orderIDs.Add(orderID);
            }
        }
        Assert.Equal(50, orderIDs.Count);
        for (int i=0; i<orderIDs.Count; i++) {
            Order order = this.orderService.GetOneOrder(orderIDs[i]);
            this.orderService.RemoveOrderManager(order);
        }
    }

    [Fact]
    public void StartOrderTest() {
        Employee employee = new Employee("Jane Doe");
        int id = this.orderService.NewOrder("Performance", "PC Laptop", "New Customer");
        this.orderService.ApproveRequest(id);
        this.orderService.StartOrder(id, employee);
        Order order = this.orderService.GetOneOrder(id);
        Assert.Equal("started", order.GetStatus());
        this.orderService.RemoveOrderManager(order);
    }

    [Fact]
    public void FinishOrderTest() {
        Employee employee = new Employee("Jane Doe");
        int id = this.orderService.NewOrder("Performance", "PC Laptop", "New Customer");
        this.orderService.ApproveRequest(id);
        this.orderService.StartOrder(id, employee);
        this.orderService.FinishOrder(id, employee);
        Order order = this.orderService.GetOneOrder(id);
        Assert.Equal("waiting for approval", order.GetStatus());
        this.orderService.RemoveOrderManager(order);
    }
    
    [Fact]
    public void JoinOrderTest() {
        Employee employee1 = new Employee("Jane Doe");
        Employee employee2 = new Employee("John Doe");
        int id = this.orderService.NewOrder("Performance", "PC Laptop", "New Customer");
        this.orderService.ApproveRequest(id);
        this.orderService.StartOrder(id, employee1);
        this.orderService.JoinOrder(id, employee2);
        Order order = this.orderService.GetOneOrder(id);
        List<Employee> employees = order.GetEmployees();
        Assert.Contains(employee1, employees);
        Assert.Contains(employee2, employees);
        this.orderService.RemoveOrderManager(order);
    }

    [Fact]
    public void LeaveOrderTwoEmployeesTest()
    {
        Employee employee1 = new Employee("Jane Doe");
        Employee employee2 = new Employee("John Doe");
        int id = this.orderService.NewOrder("Performance", "PC Laptop", "New Customer");
        this.orderService.ApproveRequest(id);
        this.orderService.StartOrder(id, employee1);
        this.orderService.JoinOrder(id, employee2);
        this.orderService.LeaveOrder(id, employee1);
        Order order = this.orderService.GetOneOrder(id);
        List<Employee> employees = order.GetEmployees();
        Assert.Single(employees);
        Assert.Contains(employee2, employees);
    }

    [Fact]
    public void LeaveOrderOneEmployeeTest() {
        Employee employee = new Employee("Jane Doe");
        int id = this.orderService.NewOrder("Performance", "PC Laptop", "New Customer");
        this.orderService.ApproveRequest(id);
        this.orderService.StartOrder(id, employee);
        this.orderService.LeaveOrder(id, employee);
        Order order = this.orderService.GetOneOrder(id);
        List<Employee> employees = order.GetEmployees();
        Assert.Empty(employees);
        this.orderService.RemoveOrderManager(order);
    }

    [Fact]
    public void ApproveRequestTest()
    {
        int id = this.orderService.NewOrder("Stability", "Macbook", "John Doe");
        Order order = this.orderService.GetOneOrder(id);
        Assert.Equal("request", order.GetStatus());
        this.orderService.ApproveRequest(id);
        Assert.Equal("not started", order.GetStatus());
        this.orderService.RemoveOrderManager(order);
    }

    [Fact]
    public void RejectRequestTest()
    {
        int id = this.orderService.NewOrder("Stability", "Macbook", "John Doe");
        Order order = this.orderService.GetOneOrder(id);
        this.orderService.RejectRequest(order);
        List<Order> orders = this.orderService.GetAll();
        Assert.Empty(orders);
    }

    [Fact]
    public void FinishOrderManagerTest() {
        Employee employee = new Employee("Jane Doe");
        int id = this.orderService.NewOrder("Performance", "PC Laptop", "New Customer");
        this.orderService.ApproveRequest(id);
        this.orderService.StartOrder(id, employee);
        this.orderService.FinishOrder(id, employee);
        this.orderService.FinishOrderManager(id);
        Order order = this.orderService.GetOneOrder(id);
        Assert.Equal("finished", order.GetStatus());
        this.orderService.RemoveOrderManager(order);
    }

    [Fact]
    public void DeliverOrderManagerTest() {
        Employee employee = new Employee("Jane Doe");
        int id = this.orderService.NewOrder("Performance", "PC Laptop", "New Customer");
        this.orderService.ApproveRequest(id);
        this.orderService.StartOrder(id, employee);
        this.orderService.FinishOrder(id, employee);
        this.orderService.FinishOrderManager(id);
        Order order = this.orderService.GetOneOrder(id);
        this.orderService.DeliverOrderManager(order);
        List<Order> orders = this.orderService.GetAll();
        Assert.Empty(orders);
    }

    [Fact]
    public void RemoveOrderManagerTest() {
        Employee employee = new Employee("Jane Doe");
        int id = this.orderService.NewOrder("Performance", "PC Laptop", "New Customer");
        this.orderService.ApproveRequest(id);
        this.orderService.StartOrder(id, employee);
        Order order = this.orderService.GetOneOrder(id);
        this.orderService.RemoveOrderManager(order);
        List<Order> orders = this.orderService.GetAll();
        Assert.Empty(orders);
    }

    [Fact]
    public void HasEmployeeTest() {
        Employee employee = new Employee("Jane Doe");
        int id = this.orderService.NewOrder("Performance", "PC Laptop", "New Customer");
        this.orderService.ApproveRequest(id);
        this.orderService.StartOrder(id, employee);
        Order order = this.orderService.GetOneOrder(id);
        bool hasEmployee = this.orderService.HasEmployee(id, employee);
        Assert.True(hasEmployee);
        this.orderService.RemoveOrderManager(order);
    }


}
