namespace Program;

using Spectre.Console;
using System.Globalization;

class UI
{
    private OrderService orderService;
    private EmployeeService employeeService;
    private Employee employee;

    public UI(OrderService orderService, EmployeeService employeeService) {
        this.orderService = orderService;
        this.employeeService = employeeService;
    }
    
    public void Start() {
        string mode = this.SelectArr("Select Mode.", new string[] {"customer", "employee", "manager"});
        if      (mode == "customer")    {this.SelectCustomer();}
        else if (mode == "employee")    {this.SelectEmployee();}
        else if (mode == "manager")     {this.SelectManager();}
    }

    void SelectCustomer() {
        string task = this.SelectArr("What would you like to do?", new string[] {"New Order", "View Order"});
        if      (task == "New Order")   {this.NewOrder();}
        else if (task == "View Order")  {this.ViewOrder();}
    }

    void NewOrder() {
        string type = this.SelectArr("Select your Repair Type.",
        new string[] {
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

        if (order == null) {
            Console.WriteLine("No order matches this number." + 
            "Either the number is incorrect, the order was rejected, or it was already picked up.");
        } else {
            Console.WriteLine("Here is your Order:");
            Console.WriteLine("order type: " + order.GetTypeString());
            Console.WriteLine("order device: " + order.GetDevice());
            Console.WriteLine("order name: " + order.GetName());
            Console.WriteLine("order status: " + order.GetStatus());
            Console.WriteLine("order date: " + order.GetDate());

            if (order.GetStatus() == "finished") {
                Console.WriteLine("Your order is ready to be picked up.");
            }
        }
    }

    void SelectEmployee() {
        List<string> names = employeeService.GetNames();
        if (names.Count == 0) {
            Console.WriteLine("No employees found. Ask a manager for help.");
        } else {
            string name = this.SelectList("Select an employee", names);
            this.employee = this.employeeService.GetOne(name);
            List<string> orders = this.orderService.GetOrderStrings();
            if (orders.Count == 0) {
                Console.WriteLine("No orders found. Have a customer place an order to start working on it.");
            } else {
                this.EditOrders();
            }
        }
    }

    void EditOrders() {
        
        string sortBy = "oldest";
        List<string> orders = this.orderService.GetOrderStrings(sortBy);

        while (true) {

            orders = this.orderService.GetOrderStrings(sortBy);

            if (orders.Find(x => x == "Sort Options") == null) {
                orders.Add("Sort Options");
            }
            if (orders.Find(x => x == "end") == null) {
                orders.Add("end");
            }
            string selectedOrder = this.SelectList("Select an order", orders);

            if (selectedOrder == "end") {
                break;
            } else if (selectedOrder == "Sort Options") {
                string[] sortOptions = {"Order Number Ascending", "Order Number Descending",
                "Date Ascending (Default)", "Date Descending","Name Ascending", "Name Descending"};
                sortBy = SelectArr("How would you like to sort?", sortOptions);
                if (sortBy == "Order Number Ascending") {
                    orders = this.orderService.GetOrderStrings("idAsc");
                } else if (sortBy == "Order Number Descending") {
                    orders = this.orderService.GetOrderStrings("idDesc");
                } else if (sortBy == "Date Ascending") {
                    orders = this.orderService.GetOrderStrings("oldest");
                } else if (sortBy == "Date Descending") {
                    orders = this.orderService.GetOrderStrings("newest");
                } else if (sortBy == "Name Ascending") {
                    orders = this.orderService.GetOrderStrings("nameAsc");
                } else if (sortBy == "Name Descending") {
                    orders = this.orderService.GetOrderStrings("nameDesc");
                }
            } else {

                string id = selectedOrder.Split("\t")[0];
                Order order = orderService.GetOneOrder(Int32.Parse(id));
                this.DisplayOrder(order);
                while (true) {
                    List<string> options = ["back"];
                    string status = order.GetStatus();
                    bool hasEmployee = orderService.HasEmployee(Int32.Parse(id), this.employee);

                    if (status == "not started") {
                        options.Insert(0, "start order");
                        }
                    else if (status == "started" && !hasEmployee) {
                        options.Insert(0, "join order");
                        }
                    else if (status == "started" && hasEmployee) {
                        options.Insert(0, "leave order");
                        options.Insert(0, "finish order");
                        }
                    string input = SelectList("Select a task.", options);

                    if (input == "back") {
                        break;
                    } else {
                        string actionWord = input.Split(" ")[0];
                        string choiceText = "Are you sure you want to " + actionWord + " this order?";
                        string choice = SelectArr(choiceText, new string[] {"Yes", "No"});

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
    }

    void SelectManager() {
        string prompt = "What would you like to do?";
        string[] choices = {"manage requests", "manage orders", "manage employees"};
        string choice = SelectArr(prompt, choices);
        if      (choice == "manage requests")   {this.ManageRequests();}
        else if (choice == "manage orders")     {this.ManageOrders();}
        else if (choice == "manage employees")  {this.ManageEmployees();}
    }

    void ManageRequests() {
        while (true) {
            List<string> requestStrings = this.orderService.GetRequestStrings();
            if (requestStrings.Find(x => x == "end") == null) {
                requestStrings.Add("end");
            }
            string request = this.SelectList("Select request", requestStrings);
            if (request == "end") {
                break;
            }
            string id = request.Split("\t")[0];
            Order order = this.orderService.GetOneOrder(Int32.Parse(id));
            this.DisplayOrder(order);

            while (true) {

                string[] options = {"approve", "reject", "back"};
                string option = SelectArr("Select a task.", options);

                if (option == "approve") {
                    DateTime suggestedDate = this.orderService.SuggestOrderDate();
                    string suggestedDateString = suggestedDate.ToString("d");
                    string actionPrompt = "Select a date for the order.";
                    string[] actions = {"Suggested Date: " + suggestedDateString, "Custom Date", "Back"};
                    string action = SelectArr(actionPrompt, actions);

                    if (action == "Custom Date") {
                        string customDate = AnsiConsole.Ask<string>("Enter a date for this order (mm/dd/yyyy).");
                        string confirm = SelectArr("Confirm date?", new string[] {"Yes", "No"});
                        if (confirm == "Yes") {
                            try {
                                DateTime newDate = DateTime.ParseExact(customDate, "d", CultureInfo.InvariantCulture);
                                this.orderService.AdjustDate(Int32.Parse(id), newDate);
                                this.orderService.ApproveRequest(Int32.Parse(id));
                            } catch (System.FormatException) {
                                Console.WriteLine("Unable to read date. Make sure your date is in the correct format.");
                            }
                            break;
                        }
                    } else if (action == "Suggested Date: " + suggestedDateString) {
                        string confirm = SelectArr("Confirm date?", new string[] {"Yes", "No"});
                        if (confirm == "Yes") {
                            this.orderService.AdjustDate(Int32.Parse(id), suggestedDate);
                            this.orderService.ApproveRequest(Int32.Parse(id));
                        }
                        break;
                    }
                } else if (option == "reject") {
                    string confirmPrompt = "Are you sure you want to reject this request?";
                    string confirm = SelectArr(confirmPrompt, new string[] {"Yes", "No"});

                    if (confirm == "Yes") {
                        this.orderService.RejectRequest(order);
                        Console.WriteLine("Request rejected.");
                        break;
                    }
                } else {
                    break;
                }
            }
        }
        
    }

    void ManageOrders() {

        string sortBy = "oldest";
        List<string> orders = this.orderService.GetOrderStrings(sortBy);
        
        while (true) {
            List<string> orderStrings = this.orderService.GetOrderStrings();
            if (orderStrings.Find(x => x == "end") == null) { // add the end option if it's not there
                orderStrings.Add("end");
            }
            string selectedOrder = this.SelectList("Select an order", orderStrings); // decide on order, or end

            if (selectedOrder == "end") {
                break;
            } else if (selectedOrder == "**Sort Options**") {
                string[] sortOptions = {"Order Number Ascending", "Order Number Descending",
                "Date Ascending (Default)", "Date Descending","Name Ascending", "Name Descending"};
                sortBy = SelectArr("How would you like to sort?", sortOptions);
                if (sortBy == "Order Number Ascending") {
                    orders = this.orderService.GetOrderStrings("idAsc");
                } else if (sortBy == "Order Number Descending") {
                    orders = this.orderService.GetOrderStrings("idDesc");
                } else if (sortBy == "Date Ascending") {
                    orders = this.orderService.GetOrderStrings("oldest");
                } else if (sortBy == "Date Descending") {
                    orders = this.orderService.GetOrderStrings("newest");
                } else if (sortBy == "Name Ascending") {
                    orders = this.orderService.GetOrderStrings("nameAsc");
                } else if (sortBy == "Name Descending") {
                    orders = this.orderService.GetOrderStrings("nameDesc");
                }
            } else {
                string id = selectedOrder.Split("\t")[0];
                Order order = orderService.GetOneOrder(Int32.Parse(id));
                this.DisplayOrder(order);
                while (true) {
                    List<string> options = ["remove order", "back"];
                    if (order.GetStatus() == "waiting for approval") {
                        options.Insert(0, "finish order");
                    } else if (order.GetStatus() == "finished") {
                        options.Insert(0, "deliver order");
                    }
                    string action = SelectList("Select a task.", options);
                    if (action == "back") {
                        break;
                    } else {
                        string actionWord = action.Split(" ")[0];
                        string confirmText = "Are you sure you want to " + actionWord + " this order?";
                        string confirmChoice = SelectArr(confirmText, new string[] {"Yes", "No"});
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
    }

    void ManageEmployees() {
        while(true) {
            List<string> names = this.employeeService.GetNames();
            if (names.Find(x => x == "end") == null) {
                names.Add("end");
            }
            if (names.Find(x => x == "New Employee") == null) { 
                names.Insert(0, "New Employee");
            }
            string choice = SelectList("Select an employee to remove, or select \"New Employee\" to add one.", names);
            if (choice == "end") {
                break;
            } else if (choice == "New Employee") {
                while(true) {
                    string newName = AnsiConsole.Ask<string>("Enter the name of the new employee.");
                    List<string> notAllowed = ["New Employee", "end"];
                    if (notAllowed.Find(x => x == newName) != null) {
                        Console.WriteLine("This name is reserved by the program. Please enter a different name.");
                    } else {
                        this.employeeService.Add(newName);
                        break;
                    }
                }
            } else {
                string confirm = SelectArr("Delete " + choice + "?", new string[] {"Yes", "No"});
                if (confirm == "Yes") {
                    this.employeeService.Remove(choice);
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

    string SelectList(string title, List<string> choices) {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .AddChoices(choices)
            );
    }

    string SelectArr(string title, string[] choices) {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .AddChoices(choices)
            );
    }
    

}