namespace Program;

using Spectre.Console;

class UI
{
    // declare dataManager
    DataManager dataManager;
    OrderService orderService;
    Employee employee;

    // constructor instantiates dataManager
    public UI(OrderService orderService, DataManager dataManager) {
        this.orderService = orderService;
        this.dataManager = dataManager;
    }
    
    public void Start() {
        List<string> tempOptions = ["customer", "employee", "manager"];
        string mode = this.Select("You are a...", tempOptions);
        if      (mode == "customer")    {this.SelectCustomer();}
        else if (mode == "employee")    {this.SelectEmployee();}
        else if (mode == "manager")     {this.SelectManager();}
    }

    public void SelectCustomer() {
        string task = this.Select("What would you like to do?", new List<string> {"New Order", "View Order"});
        if      (task == "New Order")   {this.NewOrder();}
        else if (task == "View Order")  {this.ViewOrder();}
    }

    public void NewOrder() {
        string type = this.Select("Select your Repair Type.",
        new List<string> {
            "Won't Turn On", "Frequent Crashes", "Broken Screen",
            "Other Damage", "Lost Data", "Slower than Usual"
            });

        type = type.Replace("Won't Turn On", "Performace")
            .Replace("Frequent Crashes", "Stability")
            .Replace("Broken Screen", "Screen")
            .Replace("Other Damage", "Hardware Other")
            .Replace("Lost Data", "Data Recovery")
            .Replace("Slower than Usual", "Performance");
        string device = AnsiConsole.Ask<string>("Enter your Device.");
        string name = AnsiConsole.Ask<string>("Enter your Name.");
        int orderNumber = orderService.NewOrder(type, device, name);
        Console.WriteLine("Your order number is " + orderNumber);
    }

    public void ViewOrder() {
        string orderNumber = AnsiConsole.Ask<string>("Enter your Order Number.");
        Order? order = orderService.GetOneOrder(Int32.Parse(orderNumber));

        Console.WriteLine("Here is your Order:");
        Console.WriteLine("{type:" + order.type + ", device:" + order.device + ", name:" + order.name + "}");
    }

    void SelectEmployee() {
        List<string> employees = dataManager.GetEmployees();
        if (employees.Count == 0) {
            Console.WriteLine("No employees found. Ask a manager for help.");
        } else {
            string employeeName = this.Select("Select an employee", employees);
            this.employee = dataManager.GetOneEmployee(employeeName);
            List<string> orders = this.orderService.GetOrderStrings();
            if (orders.Count == 0) {
                Console.WriteLine("No orders found. Have a customer place an order to start working on it.");
            } else {
                this.EditOrders(orders);
            }
        }
    }

    void EditOrders(List<string> orderStrings) {

        while (true) {

            // decide on order, or exit
            if (orderStrings.Find(x => x == "end") == null) {
                orderStrings.Add("end");
            }
            string selectedOrder = this.Select("Select an order", orderStrings);
            if (selectedOrder == "end") {break;}

            // display order information
            string id = selectedOrder.Split("\t")[0];
            Order order = orderService.GetOneOrder(Int32.Parse(id));
            Console.WriteLine("order id: " + id);
            Console.WriteLine("order type: " + order.type);
            Console.WriteLine("order device: " + order.device);
            Console.WriteLine("order name: " + order.name);

            if (order.employees.Count == 0) {
                Console.WriteLine("No one has started this order yet.");
            } else {
                List<string> employeeNames = [];
                for (int i=0; i<order.employees.Count; i++) {
                    employeeNames.Add(order.employees[i].name);
                }
                string employeeString = String.Join(", ", employeeNames);
                Console.WriteLine("Employees: " + employeeString);
            }
            Console.WriteLine("ORDER STATUS: " + order.status);

            // internal loop for actions
            while (true) {
                List<string> options = ["back"];
                string actionWord;

                if (order.status == "not started") {
                    options.Add("start order");
                    actionWord = "start";
                    }
                else if (order.status == "started") {
                    if (order.employees.Find(x => x.name == this.employee.name) != null) {
                        options.Add("finish order");
                        options.Add("leave order");
                        actionWord = "finish";
                    } else {
                    options.Add("join order");
                    actionWord = "join";
                }

                string decision = Select("Select a task.", options);
                if (decision == "back") {break;}
                else if (decision == "leave order") {actionWord = "leave";}

                string choiceText = "Are you sure you want to " + actionWord + " this order?";
                string choice = Select(choiceText, new List<string> {"Yes", "No"});

                if (choice == "Yes") {
                    if (decision == "start order") {
                        orderService.StartOrder();
                        break;
                    } else if (decision == "finish order") {
                        orderService.FinishOrder();
                        break;
                    } else if (decision == "join order") {
                        orderService.JoinOrder();
                        break;
                    } else if (decision == "leave order") {
                        orderService.LeaveOrder();
                    }
                }
            }
        }
    }

    } // extra bracket needed

    public void SelectManager() {
        return;
    }

    string Select(string title, List<string> choices) {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .AddChoices(choices)
            );
    }

}