namespace Program;

using Spectre.Console;
using System.Globalization;

class UI
{
    private OrderService os;
    private EmployeeService es;
    private Employee employee;

    public UI(OrderService os, EmployeeService es) {
        this.os = os;
        this.es = es;
    }
    
    public void Start() {
        AnsiConsole.Clear();
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

        type = type.Replace("Won't Turn On", "Power")
            .Replace("Frequent Crashes", "Stability")
            .Replace("Broken Screen", "Screen")
            .Replace("Other Damage", "Hardware Other")
            .Replace("Lost Data", "Data Recovery")
            .Replace("Slower than Usual", "Performance");
        string device = AnsiConsole.Ask<string>("Enter your Device.");
        string name = AnsiConsole.Ask<string>("Enter your Name.");
        int orderNumber = os.NewOrder(type, device, name);
        Console.WriteLine("Your order number is " + orderNumber);
    }

    void ViewOrder() {
        string orderNumber = AnsiConsole.Ask<string>("Enter your Order Number.");
        Order order = os.GetOneOrder(Int32.Parse(orderNumber));

        if (order == null) {
            Console.WriteLine("No order matches this number." + 
            "Either the number is incorrect, the order was rejected, or it was already picked up.");
        } else {
            Console.Write("id:", order.GetID(), ", type:", order.GetTypeString(), ", device:", order.GetDevice(),
            ", name:", order.GetName(), "status:", order.GetStatus());

            for (int i=0; i<order.GetTasks().Count; i++) {
                Task task = order.GetTasks()[i];
                Console.WriteLine(task.GetIndex().ToString(), task.GetText(), task.GetEmployees(), task.GetStatus());
            }

            if (order.GetStatus() == "finished") {
                Console.WriteLine("Your order is ready to be picked up.");
            }
        }
    }

    void SelectEmployee() {
        List<string> names = es.GetNames();
        if (names.Count == 0) {
            Console.WriteLine("No employees found. Ask a manager for help.");
        } else {
            string name = this.SelectList("Select an employee", names);
            this.employee = this.es.GetOne(name);
            List<Order> orders = this.os.GetOrders();
            if (orders.Count == 0) {
                Console.WriteLine("No orders found. Have a customer place an order to start working on it.");
            } else {
                this.EditOrders();
            }
        }
    }

    void EditOrders() {

        string sortBy = "oldest";
        List<Order> orders;

        while (true) {

            orders = this.os.GetOrders(sortBy);

            Table table = new Table();
            List<string> columns = ["id", "type", "device", "name", "date", "status"]; // employees
            for (int i=0; i<columns.Count; i++) {
                table.AddColumn(columns[i]);
            }

            List<string> options = [];
            for (int i=0; i<orders.Count; i++) {
                Order order = orders[i];
                string id = order.GetID().ToString();
                string date = order.GetDate().ToString("d");
                table.AddRow(id, order.GetTypeString(), order.GetDevice(), order.GetName(), date, order.GetStatus());
                options.Add(info);
            }

            AnsiConsole.Write(table);
            options.Add("Sorting Options");
            options.Add("End");
            string option = SelectList("Select an order.", options);

            if (option == "End") {
                AnsiConsole.Clear();
                break;
            } else if (option == "Sorting Options") {
                string[] sortOptions = {"Order Number Ascending", "Order Number Descending",
                "Date Ascending (Default)", "Date Descending","Name Ascending", "Name Descending"};
                sortBy = SelectArr("How would you like to sort?", sortOptions);
                AnsiConsole.Clear();
            } else {
                while (true) {
                    int id = Int32.Parse(option);
                    Order order = this.os.GetOneOrder(id);
                    this.DisplayOrder(order);
                    List<string> taskOptions = this.os.GetTaskOptions(id, this.employee);
                    string input = SelectList("Select an option.", taskOptions);

                    if (input == "Back") {
                        AnsiConsole.Clear();
                        break;
                    } else {
                        string confirm = SelectArr("Are you sure?", new string[] {"Yes", "No"});
                        if (confirm == "Yes") {
                            this.os.EditOrder(input, id, this.employee);
                        }
                    }
                    AnsiConsole.Clear();
                }
            }
        }
    }

    void SelectManager() {
        string prompt = "What would you like to do?";
        string[] choices = {"manage requests", "manage orders", "manage employees"};
        string choice = SelectArr(prompt, choices);
        if      (choice == "manage requests")   {this.ManageRequestsNew();}
        else if (choice == "manage orders")     {this.ManageOrders();}
        else if (choice == "manage employees")  {this.ManageEmployees();}
    }

    void ManageRequestsNew() {

        List<Order> orders;

        while (true) {

            orders = this.os.GetRequests();

            Table table = new Table();
            List<string> columns = ["id", "type", "device", "name", "status"];
            for (int i=0; i<columns.Count; i++) {
                table.AddColumn(columns[i]);
            }

            List<string> options = [];
            for (int i=0; i<orders.Count; i++) {
                Order order = orders[i];
                string id = order.GetID().ToString();
                string date = order.GetDate().ToString("d");
                table.AddRow(id, order.GetTypeString(), order.GetDevice(), order.GetName(), order.GetStatus());
                options.Add(order.GetID().ToString());
            }

            AnsiConsole.Write(table);
            options.Add("End");
            string option = SelectList("Select an order.", options);

            if (option == "End") {
                AnsiConsole.Clear();
                break;
            } else {
                while (true) {
                    int id = Int32.Parse(option);
                    Order order = this.os.GetOneOrder(id);
                    this.DisplayOrder(order);
                    string input = SelectArr("Select an option.", new string[] {"approve", "reject", "back"});
                    if (input == "Back") {
                        break;
                    } else {
                        string confirm = SelectArr("Are you sure?", new string[] {"Yes", "No"});
                        if (confirm == "Yes" && input == "approve") {
                            DateTime suggestedDate = this.os.SuggestOrderDate();
                            string suggestedDateString = suggestedDate.ToString("d");
                            string[] actions = {"Suggested Date: " + suggestedDateString, "Custom Date", "Back"};
                            string action = SelectArr("Select a date for the order.", actions);

                            if (action == "Custom Date") {
                                string customDate = AnsiConsole.Ask<string>("Enter a date for this order (mm/dd/yyyy).");
                                string customDateConfirm = SelectArr("Confirm date?", new string[] {"Yes", "No"});
                                if (customDateConfirm == "Yes") {
                                    try {
                                        DateTime newDate = DateTime.ParseExact(customDate, "d", CultureInfo.InvariantCulture);
                                        this.os.AdjustDate(id, newDate);
                                        this.os.ApproveRequest(id);
                                    } catch (System.FormatException) {
                                        Console.WriteLine("Unable to read date. Make sure your date is in the correct format.");
                                    }
                                    break;
                                }
                            } else if (action == "Suggested Date: " + suggestedDateString) {
                                string suggesteDateConfirm = SelectArr("Confirm date?", new string[] {"Yes", "No"});
                                if (suggesteDateConfirm == "Yes") {
                                    this.os.AdjustDate(id, suggestedDate);
                                    this.os.ApproveRequest(id);
                                    AnsiConsole.Clear();
                                }
                                break;
                            }
                        } else if (confirm == "Yes" && input == "reject") {
                            this.os.RemoveOrderManager(order);
                            break;
                        }
                    }
                    AnsiConsole.Clear();
                }
            }
        }
    }

    void ManageOrders() {

        string sortBy = "oldest";
        List<Order> orders;

        while (true) {

            orders = this.os.GetOrders(sortBy);

            Table table = new Table();
            List<string> columns = ["id", "type", "device", "name", "date", "status"];
            for (int i=0; i<columns.Count; i++) {
                table.AddColumn(columns[i]);
            }

            List<string> options = [];
            for (int i=0; i<orders.Count; i++) {
                Order order = orders[i];
                string id = order.GetID().ToString();
                string date = order.GetDate().ToString("d");
                table.AddRow(id, order.GetTypeString(), order.GetDevice(), order.GetName(), date, order.GetStatus());
                options.Add(order.GetID().ToString());
            }

            AnsiConsole.Write(table);
            options.Add("Sorting Options");
            options.Add("End");
            string option = SelectList("Select an order.", options);

            if (option == "End") {
                AnsiConsole.Clear();
                break;
            } else if (option == "Sorting Options") {
                string[] sortOptions = {"Order Number Ascending", "Order Number Descending",
                "Date Ascending (Default)", "Date Descending","Name Ascending", "Name Descending"};
                sortBy = SelectArr("How would you like to sort?", sortOptions);
                this.os.GetOrders(sortBy);
                AnsiConsole.Clear();
            } else {
                int id = Int32.Parse(option.Split("\t")[0]);
                Order order = this.os.GetOneOrder(id);
                this.DisplayOrder(order);

                while (true) {

                    List<string> orderOptions = this.os.ApproveOrderOptions(id);
                    string action = SelectList("Select a task.", orderOptions);

                    if (action == "Back") {
                        AnsiConsole.Clear();
                        break;
                    } else {
                        string confirmChoice = SelectArr("Are you sure?", new string[] {"Yes", "No"});
                        if (action == "finish order" && confirmChoice == "Yes") {
                            this.os.FinishOrderManager(order.GetID());
                            Console.WriteLine("Order Finished.");
                            }
                        else if (action == "deliver order" && confirmChoice == "Yes") {
                            this.os.RemoveOrderManager(order);
                            AnsiConsole.Clear();
                            Console.WriteLine("Order Delivered.");
                            break;
                        }
                        else if (action == "remove order" && confirmChoice == "Yes") {
                            this.os.RemoveOrderManager(order);
                            AnsiConsole.Clear();
                            Console.WriteLine("Order Removed.");
                            break;
                        }
                        else if (action.StartsWith("Approve Task") && confirmChoice == "Yes") {
                            int index = Int32.Parse(action.Split(" ")[^1]);
                            this.os.FinishTaskManager(id, index);
                            AnsiConsole.Clear();
                            break;
                        }
                    }
                }
            }
        }
    }

    void ManageEmployees() {

        List<string> names;

        while(true) {

            names = this.es.GetNames();
            if (names.Find(x => x == "End") == null) {
                names.Add("End");
            }
            if (names.Find(x => x == "New Employee") == null) { 
                names.Insert(0, "New Employee");
            }
            Table table = new Table();
            table.AddColumn("Employees");
            for (int i = 0; i < names.Count; i++) {
                if (names[i] != "End" && names[i] != "New Employee") {
                    table.AddRow(names[i]);
                }
                
            }
            AnsiConsole.Write(table);
            string choice = SelectList("Select an employee to remove, or select \"New Employee\" to add one.", names);
            if (choice == "End") {
                AnsiConsole.Clear();
                break;
                
            } else if (choice == "New Employee") {

                while(true) {

                    string newName = AnsiConsole.Ask<string>("Enter the name of the new employee.");
                    List<string> notAllowed = ["New Employee", "End"];
                    if (notAllowed.Find(x => x == newName) != null) {
                        Console.WriteLine("This name is reserved by the program. Please enter a different name.");
                    } else {
                        this.es.Add(newName);
                        break;
                    }
                }
            } else {
                string confirm = SelectArr("Delete " + choice + "?", new string[] {"Yes", "No"});
                if (confirm == "Yes") {
                    this.es.Remove(choice);
                }
            }
            AnsiConsole.Clear();
        }
    }

    void DisplayOrder(Order order) {
        Console.Write("id: " + order.GetID() + ", type: " + order.GetTypeString() + ", device: " + order.GetDevice() + 
            ", name: " + order.GetName() + ", status: " + order.GetStatus() + "\n\n");

        if (order.GetStatus() != "request") {
            for (int i=0; i<order.GetTasks().Count; i++) {
                Task task = order.GetTasks()[i];
                Console.WriteLine(task.GetIndex().ToString() + " - " + task.GetText() + ", Employees: " +
                task.GetEmployeeNames() + ", Status: " + task.GetStatus());
            }
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