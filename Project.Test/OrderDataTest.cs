namespace Project.Test;

using Program;

public class OrderDataTest
{
    OrderData orderData;

    public OrderDataTest() {
        this.orderData = new OrderData();

        List<Order> orders = orderData.GetOrders();
        for (int i=0; i<orders.Count; i++) {
            orderData.Remove(orders[i]);
        }

    }

    [Fact]
    public void GetAddOrdersTest() {
        Order order1 = new Order(10001, "Screen", "Laptop", "Jane Doe", "request");
        Order order2 = new Order(10002, "Performance", "Macbook", "John Doe", "not started");
        this.orderData.Add(order1);
        this.orderData.Add(order2);
        List<Order> orders = orderData.GetOrders();
        Assert.Equal(2, orders.Count);
        Assert.Contains(order1, orders);
        Assert.Contains(order2, orders);
        this.orderData.Remove(order1);
        this.orderData.Remove(order2);
    }

    [Fact]
    public void GetOneTest() {
        int id = 10001;
        Order order = new Order(id, "Screen", "Laptop", "Jane Doe", "request");
        this.orderData.Add(order);
        Order returnedOrder = orderData.GetOne(id);
        Assert.Equal(order, returnedOrder);
    }

    [Fact]
    public void RemoveOrderTest() {
        Order order = new Order(10001, "Screen", "Laptop", "Jane Doe", "request");
        this.orderData.Add(order);
        this.orderData.Remove(order);
        List<Order> orders = orderData.GetOrders();
        Assert.Empty(orders);
    }

    [Fact]
    public void UpdateOrderStatusTest() {
        int id = 10001;
        Order order = new Order(id, "Screen", "Laptop", "Jane Doe", "request");
        this.orderData.Add(order);
        this.orderData.UpdateOrderStatus(id, "not started");
        Order returedOrder = this.orderData.GetOne(id);
        Assert.Equal("not started", returedOrder.GetStatus());
        this.orderData.Remove(returedOrder);
    }

    [Fact]
    public void AdjustDateTest() {
        int id = 10001;
        Order order = new Order(id, "Screen", "Laptop", "Jane Doe", "request");
        this.orderData.Add(order);
        DateTime newDate = new DateTime(2026, 7, 1);
        this.orderData.AdjustDate(id, newDate);
        Order returedOrder = this.orderData.GetOne(id);
        Assert.Equal(newDate, returedOrder.GetDate());
        this.orderData.Remove(returedOrder);
    }

    [Fact]
    public void AddEmployeeTest() {
        int id = 10001;
        Order order = new Order(id, "Screen", "Laptop", "Jane Doe", "request");
        this.orderData.Add(order);
        Employee employee = new Employee("John Doe");
        this.orderData.AddEmployee(id, employee);
        Order returedOrder = this.orderData.GetOne(id);
        Assert.Contains(employee, returedOrder.GetEmployees());
        this.orderData.Remove(returedOrder);
    }

    [Fact]
    public void RemoveEmployeeTest() {
        int id = 10001;
        Order order = new Order(id, "Screen", "Laptop", "Jane Doe", "request");
        this.orderData.Add(order);
        Employee employee = new Employee("John Doe");
        this.orderData.AddEmployee(id, employee);
        this.orderData.RemoveEmployee(id, employee);
        Order returedOrder = this.orderData.GetOne(id);
        Assert.Empty(returedOrder.GetEmployees());
        this.orderData.Remove(returedOrder);
    }

    [Fact]
    public void SaveLoadTest() {
        int id = 10001;
        Order before = new Order(id, "Screen", "PC Laptop", "Jane Doe", "request");
        this.orderData.Add(before);
        this.orderData.SaveOrderData();
        this.orderData.LoadOrderData();
        Order after = this.orderData.GetOne(id);
        Assert.Equal(before.GetID(), after.GetID());
        Assert.Equal(before.GetTypeString(), after.GetTypeString());
        Assert.Equal(before.GetDevice(), after.GetDevice());
        Assert.Equal(before.GetName(), after.GetName());
    }

}