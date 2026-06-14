namespace Program;

class Program
{
    static void Main(string[] args)
    {
        DataManager dataManager = new DataManager();
        OrderData orderData = new OrderData();
        Random rand = new Random();
        OrderService orderService = new OrderService(dataManager, orderData, rand);
        UI ui = new UI(orderService, dataManager);
        ui.Start();
    }
}
