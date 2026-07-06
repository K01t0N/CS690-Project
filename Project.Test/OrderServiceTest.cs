namespace Project.Test;

using Program;

public class OrderServiceTest
{
    OrderData orderData;
    Random rand;
    OrderService orderService;
    
    public OrderServiceTest() {
        this.orderData = new OrderData();
        this.rand = new Random();
        this.orderService = new OrderService(this.orderData, rand);

        List<Order> orders = orderService.GetAll();
        for (int i=0; i<orders.Count; i++) {
            this.orderService.RemoveOrderManager(orders[i]);
        }

    }

    [Fact]
    public void GetAllTest() {
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
    public void GetRequestsOrdersTest() {
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

    [Fact]
    public void NewOrderTest() {
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
    public void SuggestOrderDateTest() {
        int id = this.orderService.NewOrder("Stability", "Macbook", "John Doe");
        DateTime newDate = new DateTime(2026, 8, 1);
        this.orderService.ApproveRequest(id);
        this.orderService.AdjustDate(id, newDate);
        DateTime suggestedDate = this.orderService.SuggestOrderDate();
        newDate = newDate.AddDays(this.orderData.GetDefaultDays());
        Assert.Equal(suggestedDate, newDate); // expected 8/4, got 8/1
        this.orderService.RemoveOrderManager(this.orderService.GetOneOrder(id));
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
        this.orderService.EditOrder("Start Order", id, employee);
        Order order = this.orderService.GetOneOrder(id);
        Assert.Equal("started", order.GetStatus());
        this.orderService.RemoveOrderManager(order);
    }

    [Fact]
    public void FinishOrderTest() {
        Employee employee = new Employee("Jane Doe");
        int id = this.orderService.NewOrder("Performance", "PC Laptop", "New Customer");
        this.orderService.ApproveRequest(id);
        this.orderService.EditOrder("Start Order", id, employee);
        Order order = this.orderService.GetOneOrder(id);
        for (int i = 1; i < order.GetTasks().Count + 1; i++) {
            this.orderService.EditOrder("Start Task " + i, id, employee);
            this.orderService.EditOrder("Finish Task " + i, id, employee);
            this.orderService.FinishTaskManager(id, i);
        }
        this.orderService.EditOrder("Finish Order", id, employee);
        order = this.orderService.GetOneOrder(id);
        Assert.Equal("waiting for approval", order.GetStatus());
        this.orderService.RemoveOrderManager(order);
    }
    
    [Fact]
    public void JoinOrderTest() {
        Employee employee1 = new Employee("Jane Doe");
        Employee employee2 = new Employee("John Doe");
        int id = this.orderService.NewOrder("Performance", "PC Laptop", "New Customer");
        this.orderService.ApproveRequest(id);
        this.orderService.EditOrder("Start Order", id, employee1);
        this.orderService.EditOrder("Join Order", id, employee2);
        Order order = this.orderService.GetOneOrder(id);
        List<Employee> employees = order.GetEmployees();
        Assert.Contains(employee1, employees);
        Assert.Contains(employee2, employees);
        this.orderService.RemoveOrderManager(order);
    }

    [Fact]
    public void LeaveOrderTwoEmployeesTest() {
        Employee employee1 = new Employee("Jane Doe");
        Employee employee2 = new Employee("John Doe");
        int id = this.orderService.NewOrder("Performance", "PC Laptop", "New Customer");
        this.orderService.ApproveRequest(id);
        this.orderService.EditOrder("Start Order", id, employee1);
        this.orderService.EditOrder("Join Order", id, employee2);
        this.orderService.EditOrder("Leave Order", id, employee1);
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
        this.orderService.EditOrder("Start Order", id, employee);
        this.orderService.EditOrder("Leave Order", id, employee);
        Order order = this.orderService.GetOneOrder(id);
        List<Employee> employees = order.GetEmployees();
        Assert.Empty(employees);
        this.orderService.RemoveOrderManager(order);
    }

    [Fact]
    public void ApproveRequestTest() {
        int id = this.orderService.NewOrder("Stability", "Macbook", "John Doe");
        Order order = this.orderService.GetOneOrder(id);
        Assert.Equal("request", order.GetStatus());
        this.orderService.ApproveRequest(id);
        Assert.Equal("not started", order.GetStatus());
        this.orderService.RemoveOrderManager(order);
    }

    [Fact]
    public void RejectRequestTest() {
        int id = this.orderService.NewOrder("Stability", "Macbook", "John Doe");
        Order order = this.orderService.GetOneOrder(id);
        this.orderService.RemoveOrderManager(order);
        List<Order> orders = this.orderService.GetAll();
        Assert.Empty(orders);
    }

    [Fact]
    public void FinishOrderManagerTest() {
        Employee employee = new Employee("Jane Doe");
        int id = this.orderService.NewOrder("Performance", "PC Laptop", "New Customer");
        this.orderService.ApproveRequest(id);
        this.orderService.EditOrder("Start Order", id, employee);
        Order order = this.orderService.GetOneOrder(id);
        for (int i = 1; i < order.GetTasks().Count + 1; i++) {
            this.orderService.EditOrder("Start Task " + i, id, employee);
            this.orderService.EditOrder("Finish Task " + i, id, employee);
            this.orderService.FinishTaskManager(id, i);
        }
        this.orderService.EditOrder("waiting for approval", id, employee);
        this.orderService.FinishOrderManager(id);
        order = this.orderService.GetOneOrder(id);
        Assert.Equal("finished", order.GetStatus());
        this.orderService.RemoveOrderManager(order);
    }

    [Fact]
    public void DeliverOrderManagerTest() {
        Employee employee = new Employee("Jane Doe");
        int id = this.orderService.NewOrder("Performance", "PC Laptop", "New Customer");
        this.orderService.ApproveRequest(id);
        this.orderService.EditOrder("Start Order", id, employee);
        Order order = this.orderService.GetOneOrder(id);
        for (int i = 1; i < order.GetTasks().Count + 1; i++) {
            this.orderService.EditOrder("Start Task " + i, id, employee);
            this.orderService.EditOrder("Finish Task " + i, id, employee);
            this.orderService.FinishTaskManager(id, i);
        }
        this.orderService.EditOrder("waiting for approval", id, employee);
        this.orderService.FinishOrderManager(id);
        order = this.orderService.GetOneOrder(id);
        this.orderService.RemoveOrderManager(order);
        List<Order> orders = this.orderService.GetAll();
        Assert.Empty(orders);
    }

    [Fact]
    public void RemoveOrderManagerTest() {
        Employee employee = new Employee("Jane Doe");
        int id = this.orderService.NewOrder("Performance", "PC Laptop", "New Customer");
        this.orderService.ApproveRequest(id);
        this.orderData.UpdateOrderStatus(id, "started");
        this.orderData.AddEmployee(id, employee);
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
        this.orderData.UpdateOrderStatus(id, "started");
        this.orderData.AddEmployee(id, employee);
        Order order = this.orderService.GetOneOrder(id);
        bool hasEmployee = this.orderService.HasEmployee(id, employee);
        Assert.True(hasEmployee);
        this.orderService.RemoveOrderManager(order);
    }


}
