namespace Program;

public class OrderService
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

    public List<Order> GetOrders(string sortBy="Date Ascending (Default)") {
        List<Order> orders = this.orderData.GetOrders();

        if (sortBy == "Order Number Ascending") {
            orders.Sort(CompareIDs);
        } else if (sortBy == "Order Number Descending") {
            orders.Sort(CompareIDs);
            orders.Reverse();
        } else if (sortBy == "Date Ascending (Default)") {
            orders.Sort(CompareDates);
        } else if (sortBy == "Date Descending") {
            orders.Sort(CompareDates);
            orders.Reverse();
        } else if (sortBy == "Name Ascending") {
            orders.Sort(CompareNames);
        } else if (sortBy == "Name Descending") {
            orders.Sort(CompareNames);
            orders.Reverse();
        }
        
        List<Order> filtered = new List<Order>();
        for (int i = 0; i < orders.Count; i++) {
            if (orders[i].GetStatus() != "request") {
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

    private static int CompareDates(Order x, Order y) {
        return x.GetDate().CompareTo(y.GetDate());
    }

    private static int CompareIDs(Order x, Order y) {
        return x.GetID().CompareTo(y.GetID());
    }

    private static int CompareNames(Order x, Order y) {
        return x.GetName().CompareTo(y.GetName());
    }

    public int NewOrder(string type, string device, string name) {
        int id = this.NewId();
        string status = "request";
        List<Order> orders = orderData.GetOrders();
        Order order = new Order(id, type, device, name, status);
        this.orderData.Add(order);
        return order.GetID();
    }

    public DateTime SuggestOrderDate() {
        List<Order> orders = this.GetOrders();
        if (orders.Count == 0) {
            return DateTime.Today.AddDays(this.orderData.GetDefaultDays());
        } else {
            orders.Sort(CompareDates);
            orders.Reverse();
            DateTime lastDate = orders[0].GetDate();
            return lastDate.AddDays(this.orderData.GetDefaultDays());
        }
    }

    public void AdjustDate(int id, DateTime date) {
        this.orderData.AdjustDate(id, date);
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

    public void ApproveRequest(int id) {
        this.orderData.UpdateOrderStatus(id, "not started");
        Order order = this.orderData.GetOne(id);
        string type = order.GetTypeString();
        if (type == "Power") {
            this.AddPowerTasks(id);
        } else if (type == "Stability") {
            this.AddStabilityTasks(id);
        } else if (type == "Screen") {
            this.AddScreenTasks(id);
        } else if (type == "Hardware Other") {
            this.AddHardwareTasks(id);
        } else if (type == "Performance") {
            this.AddPerformanceTasks(id);
        }

    }

    void AddPowerTasks(int id) {
        this.orderData.AddTask(id, 1, "text");
        this.orderData.AddTask(id, 2, "text");
        this.orderData.AddTask(id, 3, "text");
    }

    void AddStabilityTasks(int id) {
        this.orderData.AddTask(id, 1, "text");
        this.orderData.AddTask(id, 2, "text");
        this.orderData.AddTask(id, 3, "text");
    }

    void AddScreenTasks(int id) {
        this.orderData.AddTask(id, 1, "text");
        this.orderData.AddTask(id, 2, "text");
        this.orderData.AddTask(id, 3, "text");
    }

    void AddHardwareTasks(int id) {
        this.orderData.AddTask(id, 1, "text");
        this.orderData.AddTask(id, 2, "text");
        this.orderData.AddTask(id, 3, "text");
    }

    void AddPerformanceTasks(int id) {
        this.orderData.AddTask(id, 1, "text");
        this.orderData.AddTask(id, 2, "text");
        this.orderData.AddTask(id, 3, "text");
    }

    public void FinishTaskManager(int id, int index) {
        string status = this.orderData.GetOne(id).GetOneTask(index).GetStatus();
        int numEmployees = this.orderData.GetOne(id).GetOneTask(index).GetEmployees().Count;
        if (status == "waiting for approval" && numEmployees > 0) {
            this.orderData.UpdateTaskStatus(id, index, "finished");
        }
    }

    public void FinishOrderManager(int id) {
        if (this.orderData.GetOne(id).GetEmployees().Count > 0) {
            this.orderData.UpdateOrderStatus(id, "finished");
        }
    }

    public void RemoveOrderManager(Order order) {
        this.orderData.Remove(order);
    }

    public bool HasEmployee(int id, Employee employee) {
        return this.orderData.GetOne(id)
        .GetEmployees()
        .Find(x => x.GetName() == employee.GetName()) != null;
    }

    public List<string> GetTaskOptions(int id, Employee employee) {
        Order order = this.orderData.GetOne(id);
        List<string> taskOptions = new List<string>();
        if (order.GetStatus() == "not started") {
            taskOptions.Add("Start Order");
        } else if (order.GetStatus() == "started" && !this.HasEmployee(id, employee)) {
            taskOptions.Add("Join Order");
        } else if (order.GetStatus() == "started" && this.HasEmployee(id, employee)) {
            taskOptions.Add("Leave Order");
            if (this.AllTasksCompleted(id)) {
                taskOptions.Add("Finish Order");
            } else {
                for (int i=1; i<order.GetTasks().Count+1; i++) {
                    Task task = order.GetOneTask(i);
                    string index = i.ToString();
                    if (task.GetStatus() == "not started") {
                        bool unfinished = false;
                        for (int j = 1; j < i; j++) {
                            string status = order.GetOneTask(j).GetStatus();
                            if (status == "not started" || status == "started" || status == "waiting for approval") {
                                unfinished = true;
                            }
                        }
                        if (unfinished == false) {
                            taskOptions.Add("Start Task " + index);
                        }
                        break;
                    } else if (task.GetStatus() == "started") {
                        if (!task.HasEmployee(employee)) {
                            taskOptions.Add("Join Task " + index);
                        } else {
                            taskOptions.Add("Finish Task " + index);
                            taskOptions.Add("Leave Task " + index);
                        }
                        break;
                    }
                }
            }
        }
        taskOptions.Add("Back");
        return taskOptions;
    }

    public bool AllTasksCompleted(int id) {
        List<Task> tasks = this.orderData.GetOne(id).GetTasks();
        for (int i=0; i<tasks.Count; i++) {
            if (tasks[i].GetStatus() != "finished") {
                return false;
            }
        }
        return true;
    }

    public void EditOrder(string choice, int id, Employee employee) {
        
        if (choice == "Start Order") {
            this.orderData.UpdateOrderStatus(id, "started");
            this.orderData.AddEmployee(id, employee);
        } else if (choice == "Join Order") {
            this.orderData.AddEmployee(id, employee);
        } else if (choice == "Leave Order") {
            this.orderData.RemoveEmployee(id, employee);
        } else if (choice == "Finish Order") {
            this.orderData.UpdateOrderStatus(id, "waiting for approval");
        } else if (choice.StartsWith("Start Task")) {
            int index = Int32.Parse(choice.Split(" ")[^1]);
            this.orderData.UpdateTaskStatus(id, index, "started");
            this.orderData.GetOne(id).AddEmployeeToTask(index, employee);
        } else if (choice.StartsWith("Join Task")) {
            int index = Int32.Parse(choice.Split(" ")[^1]);
            this.orderData.GetOne(id).AddEmployeeToTask(index, employee);
        } else if (choice.StartsWith("Leave Task")) {
            int index = Int32.Parse(choice.Split(" ")[^1]);
            this.orderData.GetOne(id).RemoveEmployeeFromTask(index, employee);
        } else if (choice.StartsWith("Finish Task")) {
            int index = Int32.Parse(choice.Split(" ")[^1]);
            this.orderData.UpdateTaskStatus(id, index, "waiting for approval");
        }
    }

    public List<string> ApproveOrderOptions(int id) {
        Order order = this.orderData.GetOne(id);
        List<string> options = [];
        for (int i=1; i<(order.GetTasks().Count + 1); i++) {
            if (order.GetOneTask(i).GetStatus() == "waiting for approval") {
                options.Add("Approve Task " + i);
            }
        }
        if (order.GetStatus() == "waiting for approval") {
            options.Add("finish order");
        } else if (order.GetStatus() == "finished") {
            options.Add("deliver order");
        }
        options.Add("remove order");
        options.Add("Back");
        return options;
    }


}