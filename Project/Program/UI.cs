namespace Program;

using Spectre.Console;

class UI
{
    OrderService orderService;
    EmployeeService employeeService;
    Employee employee;

    public UI(OrderService orderService, EmployeeService employeeService) {
        this.orderService = orderService;
        this.employeeService = employeeService;
    }
    
    public void Start() {
        List<string> tempOptions = ["customer", "employee", "manager"];
        string mode = this.Select("You are a...", tempOptions);
        if      (mode == "customer")    {this.SelectCustomer();}
        else if (mode == "employee")    {this.SelectEmployee();}
        else if (mode == "manager")     {this.SelectManager();}
    }

    void SelectCustomer() {
        string task = this.Select("What would you like to do?", new List<string> {"New Order", "View Order"});
        if      (task == "New Order")   {this.NewOrder();}
        else if (task == "View Order")  {this.ViewOrder();}
    }

    void NewOrder() {
        string type = this.Select("Select your Repair Type.",
        new List<string> {
            "Won't Turn On", "Frequent Crashes", "Broken Screen",
            "Other Damage", "Lost Data", "Slower than Usual"
            });

        type = type.Replace("Won't Turn On", "Performance")
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

    void ViewOrder() {
        string orderNumber = AnsiConsole.Ask<string>("Enter your Order Number.");
        Order order = orderService.GetOneOrder(Int32.Parse(orderNumber));

        Console.WriteLine("Here is your Order:");
        Console.WriteLine("order type: " + order.GetTypeString());
        Console.WriteLine("order device: " + order.GetDevice());
        Console.WriteLine("order name: " + order.GetName());
        Console.WriteLine("order status: " + order.GetStatus());
    }

    void SelectEmployee() {
        List<string> names = employeeService.GetNames();
        if (names.Count == 0) {
            Console.WriteLine("No employees found. Ask a manager for help.");
        } else {
            string name = this.Select("Select an employee", names);
            this.employee = this.employeeService.GetOne(name);
            List<string> orders = this.orderService.GetOrderStrings();
            // 0
            if (orders.Count == 0) {
                Console.WriteLine("No orders found. Have a customer place an order to start working on it.");
            } else {
                this.EditOrders(orders);
            }
        }
    }

    void EditOrders(List<string> orderStrings) {
        while (true) {
            if (orderStrings.Find(x => x == "end") == null) { // add the end option if it's not there
                orderStrings.Add("end");
            }
            string selectedOrder = this.Select("Select an order", orderStrings); // decide on order, or end
            if (selectedOrder == "end") {break;}
            string id = selectedOrder.Split("\t")[0];
            Order order = orderService.GetOneOrder(Int32.Parse(id));
            this.DisplayOrder(order); // display order information
            while (true) { // internal loop for actions
                // this.EditOrder(order);
                List<string> options = ["back"];
                string status = order.GetStatus();
                bool hasEmployee = order.GetEmployees().Find(x => x.GetName() == employee.GetName()) == null;

                if (status == "not started") {
                    options.Insert(0, "start order");
                    }
                else if (status == "started" && hasEmployee) {
                    options.Insert(0, "join order");
                    }
                else if (status == "started" && hasEmployee) {
                    options.Insert(0, "leave order");
                    options.Insert(0, "finish order");
                    
                    }
                string input = Select("Select a task.", options);

                if (input == "back") {
                    break; // was break
                    } else {
                        string actionWord = input.Split(" ")[0];
                        string choiceText = "Are you sure you want to " + actionWord + " this order?";
                        string choice = Select(choiceText, new List<string> {"Yes", "No"});

                        if (choice == "Yes") {
                            if      (input == "start order")     {orderService.StartOrder(Int32.Parse(id), this.employee);}
                            else if (input == "finish order")    {orderService.FinishOrder(Int32.Parse(id), this.employee);}
                            else if (input == "join order")      {orderService.JoinOrder(Int32.Parse(id), this.employee);}
                            else if (input == "leave order")     {orderService.LeaveOrder(Int32.Parse(id), this.employee);}
                        }
                    }
            }
        }
    }

    void SelectManager() {
        string prompt = "What would you like to do?";
        List<string> choices = ["manage requests", "manage orders"];
        string choice = Select(prompt, choices);
        if      (choice == "manage requests")   {this.ManageRequests();}
        else if (choice == "manage orders")     {this.ManageOrders();}
        // manage employees
    }

    void ManageRequests() {
        while (true) {
            List<string> requestStrings = this.orderService.GetRequestStrings();
            if (requestStrings.Find(x => x == "end") == null) { // add the end option if it's not there
                requestStrings.Add("end");
            }
            string request = this.Select("Select request", requestStrings); // decide on order, or end
            if (request == "end") {break;}
            string id = request.Split("\t")[0];
            Order order = this.orderService.GetOneOrder(Int32.Parse(id));
            this.DisplayOrder(order); // display request information
            while (true) { // internal loop for actions
                List<string> options = ["approve", "reject", "back"];
                string action = Select("Select a task.", options);
                if (action == "back") {break;}
                string confirmPrompt = "Are you sure you want to " + action + " this request?";
                string confirm = Select(confirmPrompt, new List<string> {"Yes", "No"});
                if (action == "approve" && confirm == "Yes") {
                    this.orderService.ApproveRequest(Int32.Parse(id));
                    Console.WriteLine("Request approved.");
                    break;
                    }
                else if (action == "reject" && confirm == "Yes") {
                    this.orderService.RejectRequest(order);
                    Console.WriteLine("Request rejected.");
                    break;
                }
            }
        }
        
    }

    public void ManageOrders() {
        
        while (true) {
            List<string> orderStrings = this.orderService.GetOrderStrings();
            if (orderStrings.Find(x => x == "end") == null) { // add the end option if it's not there
                orderStrings.Add("end");
            }
            string selectedOrder = this.Select("Select order", orderStrings); // decide on order, or end
            if (selectedOrder == "end") {break;}
            string id = selectedOrder.Split("\t")[0];
            Order order = orderService.GetOneOrder(Int32.Parse(id));
            this.DisplayOrder(order); // display order information
            while (true) { // internal loop for actions
                List<string> options = ["remove order", "back"];
                // finish, deliver, remove
                if (order.GetStatus() == "waiting approval") {
                    options.Insert(0, "finish order");
                } else if (order.GetStatus() == "finished") {
                    options.Insert(0, "deliver order");
                }
                string action = Select("Select a task.", options);
                if (action == "back") {
                    break;
                } else {
                    string actionWord = action.Split(" ")[0];
                    string confirmText = "Are you sure you want to " + actionWord + " this order?";
                    string confirmChoice = Select(confirmText, new List<string> {"Yes", "No"});
                    if (actionWord == "finish" && confirmChoice == "Yes") {
                        this.orderService.FinishOrderManager(order.GetID());
                        Console.WriteLine("Order Finished.");
                        }
                    else if (actionWord == "deliver" && confirmChoice == "Yes") {
                        this.orderService.DeliverOrderManager(order);
                        Console.WriteLine("Order Delivered.");
                        break;
                    }
                    else if (actionWord == "remove" && confirmChoice == "Yes") {
                        this.orderService.RemoveOrderManager(order);
                        Console.WriteLine("Order Removed.");
                        break;
                    }
                }
            }
        }
    }

    void DisplayOrder(Order order) {
        Console.WriteLine("order id: " + order.GetID());
        Console.WriteLine("order type: " + order.GetTypeString());
        Console.WriteLine("order device: " + order.GetDevice());
        Console.WriteLine("order name: " + order.GetName());
        if (order.GetStatus() != "request") {
            if (order.GetEmployees().Count == 0) {
                Console.WriteLine("No one has started this order yet.");
            } else {
                List<string> employeeNames = [];
                List<Employee> employees = order.GetEmployees();
                for (int i=0; i<employees.Count; i++) {
                    employeeNames.Add(employees[i].GetName());
                }
                string employeeString = String.Join(", ", employeeNames);
                Console.WriteLine("Employees: " + employeeString);
            }
            Console.WriteLine("ORDER STATUS: " + order.GetStatus());
            Console.WriteLine("-----------------------");
        }
    }

    string Select(string title, List<string> choices) {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .AddChoices(choices)
            );
    }
    

}