namespace Program;

class OrderService
{

    private OrderData orderData;
    private Random rand;

    public OrderService(OrderData orderData, Random rand) {
        this.orderData = orderData;
        this.rand = rand;
    }

    public List<Order> GetAll() {
        return this.orderData.GetOrders();
    }

    public List<Order> GetOrders() {
        List<Order> orders = this.orderData.GetOrders();
        List<Order> filtered = new List<Order>();
        for (int i = 0; i < orders.Count; i++) {
            if (orders[i].GetStatus() == "order") {
                filtered.Add(orders[i]);
            }
        }
        return filtered;
    }

    public List<Order> GetRequests() {
        List<Order> orders = this.orderData.GetOrders();
        List<Order> requests = new List<Order>();
        for (int i = 0; i < orders.Count; i++) {
            if (orders[i].GetStatus() == "request") {
                requests.Add(orders[i]);
            }
        }
        return requests;
    }
    
    public Order GetOneOrder(int id) {
        return orderData.GetOne(id);
    }

    public List<string> GetOrderStrings() {
        // returns order fields separated by tabs
        List<string> orderStrings = [];
        // 1
        for (int i=0; i<this.orderData.GetOrders().Count; i++) {
            Order order = this.orderData.GetOrders()[i];
            if (order.GetStatus() != "request") {
                orderStrings.Add(order.GetID() + "\t\t" + order.GetTypeString() + "\t\t" + order.GetDevice() + "\t\t" + order.GetName());
            }
            // 0
        }
        return orderStrings;
    }

    public List<string> GetRequestStrings() {
        List<string> orderStrings = [];
        for (int i=0; i<this.orderData.GetOrders().Count; i++) {
            Order order = this.orderData.GetOrders()[i];
            if (order.GetStatus() == "request") {
                orderStrings.Add(order.GetID() + "\t\t" + order.GetTypeString() + "\t\t" + order.GetDevice() + "\t\t" + order.GetName());
            }
            
        }
        return orderStrings;
    }

    public int NewOrder(string type, string device, string name) {
        int id = this.NewId();
        string status = "request";
        Order order = new Order(id, type, device, name, status);
        this.orderData.Add(order);
        return order.GetID();
    }

    int NewId() {
        // generates a random id that is unique from other ids in the orders list
        // https://learn.microsoft.com/en-us/dotnet/api/system.randomview=net-10.0
        int number = new int();
        bool match = true;
        while(match == true) {
            match = false;
            number = this.rand.Next(10000, 99999);
            if (orderData.GetOne(number) != null) {
                match = true;
                }
        }
        return number;
    }

    public void StartOrder(int id, Employee employee) {
        this.orderData.UpdateOrderStatus(id, "started");
        this.orderData.AddEmployee(id, employee);
    }
    public void FinishOrder(int id, Employee employee) {
        if (this.HasEmployee(id, employee)) {
           this.orderData.UpdateOrderStatus(id, "waiting for approval");
        }
    }
    public void JoinOrder(int id, Employee employee) {
        if (!this.HasEmployee(id, employee)) {
            this.orderData.AddEmployee(id, employee);
        }
    }
    public void LeaveOrder(int id, Employee employee) {
        if (this.HasEmployee(id, employee)) {
            this.orderData.RemoveEmployee(id, employee);
        }
    }
    public void ApproveRequest(int id) {
        this.orderData.UpdateOrderStatus(id, "not started");
    }
    public void RejectRequest(Order order) {
        this.orderData.Remove(order);
    }
    public void FinishOrderManager(int id) {
        this.orderData.UpdateOrderStatus(id, "finished");
    }
    public void DeliverOrderManager(Order order) {
        this.orderData.Remove(order);
    }

    public bool HasEmployee(int id, Employee employee) {
        return this.orderData.GetOne(id)
        .GetEmployees()
        .Find(x => x.GetName() == employee.GetName()) != null;
    }
    Order GetOrder(Order order) {
        return this.orderData.GetOne(order.GetID());
    }

    public void RemoveOrderManager(Order order) {
        this.orderData.Remove(order);
    }


}