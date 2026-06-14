namespace Program;

class OrderService
{

    DataManager dataManager;
    OrderData orderData;
    Random rand;

    public OrderService(DataManager datamanager, OrderData orderData, Random rand) {
        this.dataManager = dataManager;
        this.orderData = orderData;
        this.rand = rand;
    }

    public List<Order> GetOrders() {
        return this.orderData.GetOrders();
    }

    public Order? GetOneOrder(int id) {
        return orderData.GetOne(id);
    }

    public List<string> GetOrderStrings() {
        // returns order fields separated by tabs
        List<string> orderStrings = [];
        for (int i=0; i<this.orderData.GetOrders().Count; i++) {
            Order order = this.orderData.GetOrders()[i];
            orderStrings.Add(order.id + "\t\t" + order.type + "\t\t" + order.device + "\t\t" + order.name);
        }
        return orderStrings;
    }

    public int NewOrder(string type, string device, string name) {
        int id = this.NewId();
        string status = "request";
        Order order = new Order(id, type, device, name, status);
        this.orderData.Add(order); // fix
        this.orderData.SaveOrders();
        return order.id;
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

    public void StartOrder() {
        Console.WriteLine("StartOrder works!");
    }
    public void FinishOrder() {
        Console.WriteLine("FinishOrder works!");
    }
    public void JoinOrder() {
        Console.WriteLine("JoinOrder works!");
    }
    public void LeaveOrder() {
        Console.WriteLine("LeaveOrder works!");
    }

}