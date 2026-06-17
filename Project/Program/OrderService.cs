namespace Program;

class OrderService
{

    OrderData orderData;
    Random rand;

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
            if (orders[i].GetStatus() != "order") {
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
    
    public Order? GetOneOrder(int id) {
        return orderData.GetOne(id);
    }

    public List<string> GetOrderStrings() {
        // returns order fields separated by tabs
        List<string> orderStrings = [];
        for (int i=0; i<this.orderData.GetOrders().Count; i++) {
            Order order = this.orderData.GetOrders()[i];
            if (order.GetStatus() != "request") {
                orderStrings.Add(order.GetID() + "\t\t" + order.GetTypeString() + "\t\t" + order.GetDevice() + "\t\t" + order.GetName());
            }
            
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
        this.orderData.Add(order); // fix
        this.orderData.SaveOrders();
        return order.GetID();
    }

    int NewId() { // generates a random id that is unique from other ids in the orders list
        // https://learn.microsoft.com/en-us/dotnet/api/system.random?view=net-10.0
        int number = new int();
        // List<Order> orders = this.orders; // what is this for?
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

    public void StartOrder(Order order, Employee employee) {
        this.orderData.GetOne(order.GetID()).SetStatus("started");
    }
    public void FinishOrder(Order order, Employee employee) {
        if (this.HasEmployee(order, employee)) {
           this.GetOrder(order).SetStatus("finished"); 
        }
    }
    public void JoinOrder(Order order, Employee employee) {
        if (!this.HasEmployee(order, employee)) {
            this.GetOrder(order).GetEmployees().Add(employee);
        }
    }
    public void LeaveOrder(Order order, Employee employee) {
        if (this.HasEmployee(order, employee)) {
            this.GetOrder(order).GetEmployees().Remove(employee);
        }
    }
    public void ApproveRequest(Order order) {
        this.GetOrder(order).SetStatus("not started");
    }
    public void RejectRequest(Order order) {
        this.orderData.GetOrders().Remove(order);
    }
    public void FinishOrderManager(Order order) {
        return;
    }
    public void DeliverOrderManager(Order order) {
        return;
    }

    bool HasEmployee(Order order, Employee employee) {
        return order.GetEmployees().Find(x => x.GetName() == employee.GetName()) != null;
    }
    Order GetOrder(Order order) {
        return this.orderData.GetOne(order.GetID());
    }


}