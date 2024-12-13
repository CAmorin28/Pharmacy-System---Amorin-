using System;
using System.Collections.Generic;
using System.IO;
using Pharmacy_System;
using static Pharmacy_System.Class1;

public abstract class Entity
{
    public abstract void DisplayDetails();
}

public class User : Entity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    private string Password { get; set; }

    private static readonly string usersFilePath = @"C:\Users\Christian Paul\Documents\Project File Handling data\users.txt";

    public void SetPassword(string password)
    {
        Password = password;
    }

    public bool Authenticate(string email, string password)
    {
        return Email == email && Password == password;
    }

    public override void DisplayDetails()
    {
        Console.WriteLine($"Name: {FirstName} {LastName}, Email: {Email}");
    }

    public void SaveToFile()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(usersFilePath));
            using (StreamWriter writer = new StreamWriter(usersFilePath, true))
            {
                writer.WriteLine($"{FirstName},{LastName},{Email},{Password}");
            }
            Console.WriteLine("User saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while saving the user: " + ex.Message);
        }
    }

    public static List<User> LoadFromFile()
    {
        List<User> users = new List<User>();
        if (File.Exists(usersFilePath))
        {
            foreach (var line in File.ReadAllLines(usersFilePath))
            {
                var parts = line.Split(',');
                if (parts.Length == 4)
                {
                    User user = new User
                    {
                        FirstName = parts[0],
                        LastName = parts[1],
                        Email = parts[2]
                    };
                    user.SetPassword(parts[3]);
                    users.Add(user);
                }
            }
        }
        return users;
    }
}

public class Reservation : Entity
{
    private static Random random = new Random();
    public int ReservationID { get; private set; }
    public DateTime Date { get; set; }
    public DateTime TimeStamp { get; private set; }
    public string UserEmail { get; set; }

    private static readonly string reservationsFilePath = @"C:\Users\Christian Paul\Documents\Project File Handling data\reservations.txt";
    private static readonly string logFilePath = @"C:\Users\Christian Paul\Documents\Project File Handling data\log.txt";

    public Reservation(string userEmail)
    {
        ReservationID = random.Next(100000, 999999);
        TimeStamp = DateTime.Now;
        UserEmail = userEmail;
    }

    public override void DisplayDetails()
    {
        Console.WriteLine($"Reservation ID: {ReservationID}, Date: {Date.ToShortDateString()}, Time: {TimeStamp.ToShortTimeString()}, User: {UserEmail}");
    }

    public void SaveToFile()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(reservationsFilePath));
            using (StreamWriter writer = new StreamWriter(reservationsFilePath, true))
            {
                writer.WriteLine($"{ReservationID},{Date:yyyy-MM-dd},{TimeStamp:HH:mm},{UserEmail}");
            }

            LogAction();
            Console.WriteLine("Reservation saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while saving the reservation: " + ex.Message);
        }
    }

    private void LogAction()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now:yyyy-MM-dd},{DateTime.Now:HH:mm},{UserEmail}");
            }
            Console.WriteLine("Log entry created successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while creating log entry: " + ex.Message);
        }
    }
}

public class Medicine : Entity
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public bool RequiresPrescription { get; set; }

    private static readonly string reservedMedicinesFilePath = @"C:\Users\Christian Paul\Documents\Project File Handling data\reserved_medicines.txt";

    public override void DisplayDetails()
    {
        Console.WriteLine($"Name: {Name}, Price: {Price:C}, Prescription Required: {RequiresPrescription}");
    }

    public void SaveToReservedFile()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(reservedMedicinesFilePath));
            using (StreamWriter writer = new StreamWriter(reservedMedicinesFilePath, true))
            {
                writer.WriteLine($"{Name},{Price}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while saving the reserved medicine data: " + ex.Message);
        }
    }

    public static void UpdateStock(string medicineName, int reservedQuantity, string inventoryFilePath)
    {
        try
        {
            var lines = File.ReadAllLines(inventoryFilePath);
            for (int i = 0; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                if (parts[0].Equals(medicineName, StringComparison.OrdinalIgnoreCase))
                {
                    int currentStock = int.Parse(parts[3]);
                    if (currentStock >= reservedQuantity)
                    {
                        parts[3] = (currentStock - reservedQuantity).ToString();
                        lines[i] = string.Join(",", parts);
                    }
                    else
                    {
                        Console.WriteLine($"Not enough stock for {medicineName}. Current stock: {currentStock}");
                        return;
                    }
                }
            }

            File.WriteAllLines(inventoryFilePath, lines);
            Console.WriteLine("Inventory updated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while updating inventory: " + ex.Message);
        }
    }
}
public class SalesReport : Entity
{
    public int ReservationID { get; set; }
    public string MedicineName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public DateTime Date { get; set; }
    public decimal Total => Price * Quantity;

    private static readonly string salesReportFilePath = @"C:\Users\Christian Paul\Documents\Project File Handling data\sales_report.txt";

    public override void DisplayDetails()
    {
        // This method is intentionally empty to prevent public access.
        // Sales reports should only be accessible programmatically.
    }

    public void SaveToDatabase()
    {
        try
        {
            // Load existing sales data
            List<SalesReport> existingReports = LoadFromDatabase();

            // Check if the current entry already exists
            bool exists = existingReports.Any(sr =>
                sr.ReservationID == this.ReservationID &&
                sr.MedicineName == this.MedicineName &&
                sr.Quantity == this.Quantity &&
                sr.Date == this.Date &&
                sr.Price == this.Price);

            if (exists)
            {
                Console.WriteLine($"Duplicate entry for {MedicineName} in Reservation ID {ReservationID}. Skipping...");
                return; // Skip saving
            }

            // Append to the file if it's not a duplicate
            Directory.CreateDirectory(Path.GetDirectoryName(salesReportFilePath));
            using (StreamWriter writer = new StreamWriter(salesReportFilePath, true))
            {
                writer.WriteLine($"{ReservationID},{MedicineName},{Price:F2},{Quantity},{Date:yyyy-MM-dd},{Total:F2}");
            }

            Console.WriteLine("Sales report saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while saving the sales report: " + ex.Message);
        }
    }

    public static List<SalesReport> LoadFromDatabase()
    {
        List<SalesReport> salesReports = new List<SalesReport>();
        if (File.Exists(salesReportFilePath))
        {
            foreach (var line in File.ReadAllLines(salesReportFilePath))
            {
                var parts = line.Split(',');
                if (parts.Length == 6)
                {
                    salesReports.Add(new SalesReport
                    {
                        ReservationID = int.Parse(parts[0]),
                        MedicineName = parts[1],
                        Price = decimal.Parse(parts[2]),
                        Quantity = int.Parse(parts[3]),
                        Date = DateTime.Parse(parts[4])
                    });
                }
            }
        }
        return salesReports;
    }

    public static decimal CalculateWeeklyTotal()
    {
        var salesReports = LoadFromDatabase();
        DateTime now = DateTime.Now;
        DateTime weekStart = now.AddDays(-(int)now.DayOfWeek); // Start of the current week

        return salesReports
            .Where(sr => sr.Date >= weekStart && sr.Date <= now)
            .Sum(sr => sr.Total);
    }

    public static decimal CalculateMonthlyTotal()
    {
        var salesReports = LoadFromDatabase();
        DateTime now = DateTime.Now;
        DateTime monthStart = new DateTime(now.Year, now.Month, 1);

        return salesReports
            .Where(sr => sr.Date >= monthStart && sr.Date <= now)
            .Sum(sr => sr.Total);
    }
}


class Program
{
    static List<User> users = new List<User>();
    static List<Medicine> medicines = new List<Medicine>();

    public static bool NO { get; private set; }
    public static bool YES { get; private set; }


    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Welcome to Pharmacy Reservation System!");

        string inventoryFilePath = @"C:\Users\Christian Paul\Documents\Project File Handling data\inventory.txt";

        // Update inventory to replace true/false and add expiration date
        //InventoryUpdater.UpdateInventoryFile();
        string pharmacyFilePath = @"C:\Users\Christian Paul\Documents\Project File Handling data\pharmacyDetails.txt";

        users = User.LoadFromFile();


        while (true)
        {
            Console.WriteLine("\nPlease select an option:");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Log In");
            Console.WriteLine("3. Exit");
            Console.Write("Your choice: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                if (RegisterUser())
                {
                    Console.WriteLine("Registration successful! You can now log in.");
                    Console.Clear();
                }
            }
            else if (choice == "2")
            {
                if (LoginUser(out User loggedInUser))
                {
                    if (loggedInUser == null) continue;
                    Console.Write("Enter Pharmacy Name: ");
                    string pharmacyName = Console.ReadLine();
                    Console.Write("Enter Pharmacy Location: ");
                    string pharmacyLocation = Console.ReadLine();

                    SavePharmacyDetails(pharmacyName, pharmacyLocation, pharmacyFilePath);

                    SeedMedicines();
                    DisplayMedicinesByCategory();

                    List<(Medicine medicine, int quantity)> reservedMedicines = new List<(Medicine medicine, int quantity)>();
                    Console.WriteLine("\nSearch and Reserve Medicines (Type 'done' when finished)");

                    while (true)
                    {
                        Console.Write("Enter medicine name: ");
                        string searchTerm = Console.ReadLine();
                        if (searchTerm.Equals("done", StringComparison.OrdinalIgnoreCase)) break;

                        var selectedMedicine = SearchMedicine(searchTerm);

                        if (selectedMedicine != null)
                        {
                            int availableStock = GetStockAvailability(selectedMedicine.Name, inventoryFilePath);

                            if (availableStock > 0)
                            {
                                Console.WriteLine($"Available Stock for {selectedMedicine.Name}: {availableStock}");

                                if (selectedMedicine.RequiresPrescription)
                                {
                                    Console.Write("This medicine requires a prescription. Do you have one? (yes/no): ");
                                    if (Console.ReadLine().ToLower() != "yes") continue;
                                }

                                Console.Write("Enter quantity: ");
                                if (int.TryParse(Console.ReadLine(), out int quantity))
                                {
                                    if (quantity > availableStock)
                                    {
                                        Console.WriteLine($"Not enough stock. Available stock is {availableStock}.");
                                        continue;
                                    }

                                    reservedMedicines.Add((selectedMedicine, quantity));
                                    Console.WriteLine($"{quantity} of {selectedMedicine.Name} added to reservation.");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid quantity. Please enter a numeric value.");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Medicine {selectedMedicine.Name} is out of stock.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Medicine not found.");
                        }
                    }

                    static int GetStockAvailability(string medicineName, string inventoryFilePath)
                    {
                        try
                        {
                            if (File.Exists(inventoryFilePath))
                            {
                                var lines = File.ReadAllLines(inventoryFilePath);
                                foreach (var line in lines)
                                {
                                    var parts = line.Split(',');
                                    if (parts[0].Equals(medicineName, StringComparison.OrdinalIgnoreCase))
                                    {
                                        return int.Parse(parts[3]); // Assuming stock is the 4th column
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Inventory database file not found.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred while checking stock availability: {ex.Message}");
                        }
                        return 0; // Default to 0 if not found or error occurs
                    }
                    // Allow modifications to the reservation
                    while (true)
                    {
                        Console.WriteLine("\n--- Current Reserved Items ---");
                        // After user finalizes reserved medicines

                        // Create the reservation object
                        Reservation reservation = new Reservation(loggedInUser.Email)
                        {
                            Date = DateTime.Now // Set the reservation date
                        };

                        // Iterate over reserved medicines and save to the sales report
                        // Group reserved medicines by medicine name to avoid duplicates
                        reservedMedicines = reservedMedicines
                            .GroupBy(m => m.medicine.Name) // Group by medicine name
                            .Select(g => (g.First().medicine, g.Sum(x => x.quantity))) // Combine quantities for duplicates
                            .ToList();

                        // Process each unique reserved medicine
                        foreach (var (medicine, quantity) in reservedMedicines)
                        {
                            decimal total = medicine.Price * quantity;

                            // Update stock
                            Medicine.UpdateStock(medicine.Name, quantity, inventoryFilePath);

                            // Create and save sales report entry
                            SalesReport salesReport = new SalesReport
                            {
                                ReservationID = reservation.ReservationID, // Shared ReservationID
                                MedicineName = medicine.Name,
                                Price = medicine.Price,
                                Quantity = quantity,
                                Date = reservation.Date
                            };

                            // Save the sales report to the database
                            salesReport.SaveToDatabase();
                        }





                        Console.WriteLine("\nOptions:");
                        Console.WriteLine("1. Modify quantity");
                        Console.WriteLine("2. Remove item");
                        Console.WriteLine("3. Add new item");
                        Console.WriteLine("4. Proceed to payment");
                        Console.Write("Your choice: ");
                        string userChoice = Console.ReadLine();

                        if (userChoice == "1")
                        {
                            Console.Write("Enter the item number to modify: ");
                            if (int.TryParse(Console.ReadLine(), out int itemIndex))
                            {
                                itemIndex -= 1; // Convert to zero-based index
                                if (itemIndex >= 0 && itemIndex < reservedMedicines.Count)
                                {
                                    Console.Write($"Enter new quantity for {reservedMedicines[itemIndex].medicine.Name}: ");
                                    if (int.TryParse(Console.ReadLine(), out int newQuantity))
                                    {
                                        // Validate stock
                                        int availableStock = GetStockAvailability(reservedMedicines[itemIndex].medicine.Name, inventoryFilePath);
                                        if (newQuantity > availableStock)
                                        {
                                            Console.WriteLine($"Not enough stock. Available stock is {availableStock}.");
                                        }
                                        else
                                        {
                                            reservedMedicines[itemIndex] = (reservedMedicines[itemIndex].medicine, newQuantity);
                                            Console.WriteLine("Quantity updated successfully.");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid input. Please enter a valid quantity.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid item number.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please enter a valid item number.");
                            }
                        }
                        else if (userChoice == "2")
                        {
                            Console.Write("Enter the item number to remove: ");
                            if (int.TryParse(Console.ReadLine(), out int itemIndex))
                            {
                                itemIndex -= 1; // Convert to zero-based index
                                if (itemIndex >= 0 && itemIndex < reservedMedicines.Count)
                                {
                                    Console.WriteLine($"{reservedMedicines[itemIndex].medicine.Name} removed from reservation.");
                                    reservedMedicines.RemoveAt(itemIndex);
                                }
                                else
                                {
                                    Console.WriteLine("Invalid item number.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please enter a valid item number.");
                            }
                        }
                        else if (userChoice == "3")
                        {
                            Console.Write("Enter medicine name: ");
                            string searchTerm = Console.ReadLine();

                            var selectedMedicine = SearchMedicine(searchTerm);
                            if (selectedMedicine != null)
                            {
                                int availableStock = GetStockAvailability(selectedMedicine.Name, inventoryFilePath);
                                if (availableStock > 0)
                                {
                                    Console.Write("Enter quantity: ");
                                    if (int.TryParse(Console.ReadLine(), out int quantity))
                                    {
                                        if (quantity > availableStock)
                                        {
                                            Console.WriteLine($"Not enough stock. Available stock is {availableStock}.");
                                        }
                                        else
                                        {
                                            reservedMedicines.Add((selectedMedicine, quantity));
                                            Console.WriteLine($"{quantity} of {selectedMedicine.Name} added to reservation.");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid input. Please enter a valid quantity.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Medicine {selectedMedicine.Name} is out of stock.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Medicine not found.");
                            }
                        }
                        else if (userChoice == "4")
                        {
                            break; // Proceed to payment
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice. Please try again.");
                        }
                    }

                    // Recalculate total cost
                    decimal totalCost = CalculateUpdatedTotalCost(reservedMedicines, inventoryFilePath);
                    Console.WriteLine($"Updated Total Cost of Reserved Medicines: ₱ {totalCost:F2}");
                    if (reservedMedicines.Count > 0)
                    {
                        Reservation reservation = new Reservation(loggedInUser.Email);
                        Console.Write("Enter Reservation Date (yyyy-MM-dd): ");
                        reservation.Date = DateTime.ParseExact(Console.ReadLine(), "yyyy-MM-dd", null);
                        reservation.SaveToFile();

                        // Payment process
                        bool isPaid = false;
                        string paymentMethod = "";

                        while (!isPaid)
                        {
                            Console.WriteLine("\nPayment Options:");
                            Console.WriteLine("1. Cash");
                            Console.WriteLine("2. Gcash");
                            Console.Write("Select a payment option: ");
                            string paymentOption = Console.ReadLine();

                            switch (paymentOption)
                            {
                                case "1": // Cash payment
                                    paymentMethod = "Cash";
                                    Console.Write("Enter cash amount: ");
                                    decimal cashAmount = decimal.Parse(Console.ReadLine());

                                    if (cashAmount < totalCost)
                                    {
                                        Console.WriteLine($"Insufficient amount. You need ₱{(totalCost - cashAmount):F2} more.");
                                    }
                                    else
                                    {
                                        decimal change = cashAmount - totalCost;
                                        Console.WriteLine($"Cash payment successful. Change: ₱{change:F2}");
                                        isPaid = true; // Mark as paid
                                    }
                                    break;

                                case "2": // Gcash payment
                                    paymentMethod = "Gcash";
                                    Console.Write("Enter Gcash Reference Number: ");
                                    string referenceNumber = Console.ReadLine();

                                    // Validate reference number (e.g., not empty)
                                    while (string.IsNullOrWhiteSpace(referenceNumber))
                                    {
                                        Console.WriteLine("Invalid Reference Number. Please enter again.");
                                        Console.Write("Enter Gcash Reference Number: ");
                                        referenceNumber = Console.ReadLine();
                                    }

                                    Console.WriteLine("Gcash payment successful.");
                                    isPaid = true; // Mark as paid
                                    break;

                                default:
                                    Console.WriteLine("Invalid option. Please select again.");
                                    break;
                            }
                        }

                        // Record receipt after successful payment
                        if (isPaid)
                        {
                            ReceiptManager.SaveReceipt(reservation, reservedMedicines, totalCost, paymentMethod);
                            Console.WriteLine("Receipt has been recorded in the database.");
                        }




                        Console.WriteLine("\n--- Receipt ---");
                        reservation.DisplayDetails();

                        foreach (var (medicine, quantity) in reservedMedicines)
                        {
                            Console.WriteLine($"Medicine: {medicine.Name}, Quantity: {quantity}, Price per unit: {medicine.Price:C}, Total: {(medicine.Price * quantity):C}");
                            Medicine.UpdateStock(medicine.Name, quantity, inventoryFilePath);
                        }
                    }
                }
            }
            else if (choice == "3")
            {
                Console.WriteLine("Thank you for using the Pharmacy Reservation System. Goodbye!");
                break;
            }
            else
            {
                Console.WriteLine("Invalid choice. Please try again.");
            }
        }
    }
    public static void UpdateSalesReportFromInventory()
    {
        string inventoryFilePath = @"C:\Users\Christian Paul\Documents\Project File Handling data\inventory.txt";
        string salesReportFilePath = @"C:\Users\Christian Paul\Documents\Project File Handling data\sales_report.txt";

        try
        {
            // Load inventory data into a dictionary (MedicineName -> Price)
            var inventory = new Dictionary<string, decimal>();
            if (File.Exists(inventoryFilePath))
            {
                foreach (var line in File.ReadAllLines(inventoryFilePath))
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 2) // Ensure at least name and price are present
                    {
                        string medicineName = parts[0].Trim();
                        decimal price = decimal.Parse(parts[1]);
                        inventory[medicineName] = price;
                    }
                }
            }

            // Load and update sales report data
            var updatedSalesReports = new List<string>();
            if (File.Exists(salesReportFilePath))
            {
                foreach (var line in File.ReadAllLines(salesReportFilePath))
                {
                    var parts = line.Split(',');
                    if (parts.Length == 6) // Ensure all fields are present
                    {
                        string reservationID = parts[0];
                        string medicineName = parts[1];
                        decimal oldPrice = decimal.Parse(parts[2]);
                        int quantity = int.Parse(parts[3]);
                        string oldDate = parts[4];
                        decimal oldTotal = decimal.Parse(parts[5]);

                        // Update price and total from inventory if it exists
                        if (inventory.TryGetValue(medicineName, out decimal updatedPrice))
                        {
                            if (updatedPrice != oldPrice) // Only update if the price has changed
                            {
                                oldPrice = updatedPrice;
                                oldTotal = updatedPrice * quantity; // Recalculate total
                                oldDate = DateTime.Now.ToString("yyyy-MM-dd"); // Update to the current date
                            }
                        }

                        // Rebuild the sales report line
                        updatedSalesReports.Add($"{reservationID},{medicineName},{oldPrice:F2},{quantity},{oldDate},{oldTotal:F2}");
                    }
                }

                // Overwrite the sales report file with updated data
                File.WriteAllLines(salesReportFilePath, updatedSalesReports);
                Console.WriteLine("Sales report prices and dates updated successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while updating the sales report: {ex.Message}");
        }
    }


    public static class ReceiptManager
    {
        private static readonly string receiptsFilePath = @"C:\Users\Christian Paul\Documents\Project File Handling data\receipts.txt";

        public static void SaveReceipt(Reservation reservation, List<(Medicine medicine, int quantity)> reservedMedicines, decimal totalCost, string paymentMethod)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(receiptsFilePath));
                using (StreamWriter writer = new StreamWriter(receiptsFilePath, true))
                {
                    writer.WriteLine("--------------------------------------------------");
                    writer.WriteLine($"Reservation ID: {reservation.ReservationID}");
                    writer.WriteLine($"Reservation Date: {reservation.Date:yyyy-MM-dd}");
                    writer.WriteLine($"Customer Email: {reservation.UserEmail}");
                    writer.WriteLine($"Payment Method: {paymentMethod}");
                    writer.WriteLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

                    foreach (var (medicine, quantity) in reservedMedicines)
                    {
                        decimal updatedPrice = GetUpdatedPriceFromInventory(medicine.Name, @"C:\Users\Christian Paul\Documents\Project File Handling data\inventory.txt");
                        decimal lineTotal = updatedPrice * quantity;

                        writer.WriteLine($"- Medicine: {medicine.Name}");
                        writer.WriteLine($"  Quantity: {quantity}");
                        writer.WriteLine($"  Price per unit: ₱{updatedPrice:F2}");
                        writer.WriteLine($"  Total: ₱{lineTotal:F2}");

                        // Update SalesReport
                        SalesReport salesReport = new SalesReport
                        {
                            ReservationID = reservation.ReservationID,
                            MedicineName = medicine.Name,
                            Price = updatedPrice,
                            Quantity = quantity,
                            Date = reservation.Date
                        };
                        salesReport.SaveToDatabase(); // Save updated entry
                    }

                    writer.WriteLine($"Total Cost: ₱{totalCost:F2}");
                    writer.WriteLine("--------------------------------------------------");
                }

                Console.WriteLine("Receipt and SalesReport recorded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving the receipt and updating the sales report: {ex.Message}");
            }
        }

        private static decimal GetUpdatedPriceFromInventory(string medicineName, string inventoryFilePath)
        {
            try
            {
                if (File.Exists(inventoryFilePath))
                {
                    var lines = File.ReadAllLines(inventoryFilePath);
                    foreach (var line in lines)
                    {
                        var parts = line.Split(',');
                        if (parts[0].Equals(medicineName, StringComparison.OrdinalIgnoreCase))
                        {
                            return decimal.Parse(parts[1]); // Assuming price is the second column
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching the price for {medicineName}: {ex.Message}");
            }

            return 0; // Default to 0 if not found or error occurs
        }
    }
    class InventoryUpdater
    {
        public static void UpdateInventoryFile()
        {
            string inventoryFilePath = @"C:\Users\Christian Paul\Documents\Project File Handling data\inventory.txt";

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(inventoryFilePath));

                using (StreamWriter writer = new StreamWriter(inventoryFilePath))
                {
                    writer.WriteLine("Name,Price,RequiresPrescription,Stock,ExpirationDate");

                    // Inventory details matching the output display
                    writer.WriteLine($"Biodesic,50.00,No,100,{GenerateRandomExpirationDate()}");
                    writer.WriteLine($"Alaxan,30.00,No,100,{GenerateRandomExpirationDate()}");
                    writer.WriteLine($"Dolfenal,40.00,No,100,{GenerateRandomExpirationDate()}");
                    writer.WriteLine($"Residol,35.00,No,100,{GenerateRandomExpirationDate()}");
                    writer.WriteLine($"Tuseran Forte,45.00,No,100,{GenerateRandomExpirationDate()}");
                    writer.WriteLine($"Myracof Tablet,60.00,No,100,{GenerateRandomExpirationDate()}");
                    writer.WriteLine($"Solmux Advance,40.00,Yes,100,{GenerateRandomExpirationDate()}");
                    writer.WriteLine($"Diatabs,25.00,No,100,{GenerateRandomExpirationDate()}");
                    writer.WriteLine($"Imodium,20.00,No,100,{GenerateRandomExpirationDate()}");
                    writer.WriteLine($"Flotera Power,50.00,No,100,{GenerateRandomExpirationDate()}");
                    writer.WriteLine($"Kremil S,15.00,No,100,{GenerateRandomExpirationDate()}");
                    writer.WriteLine($"Dolfenal Advance,55.00,No,100,{GenerateRandomExpirationDate()}");
                    writer.WriteLine($"Medicol,25.00,No,100,{GenerateRandomExpirationDate()}");
                    writer.WriteLine($"Skelan,35.00,No,100,{GenerateRandomExpirationDate()}");
                }

                Console.WriteLine("Inventory file updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while updating the inventory file: " + ex.Message);
            }
        }
        private static string GenerateRandomExpirationDate()
        {
            Random random = new Random();
            DateTime expirationDate = DateTime.Now.AddDays(random.Next(30, 365)); // Random expiration within the next year
            return expirationDate.ToString("yyyy-MM-dd");
        }
    }




    static void SeedMedicines()
    {
        medicines.AddRange(new List<Medicine>
    {

        new Medicine { Name = "Biodesic", Price = 50, RequiresPrescription = NO },
        new Medicine { Name = "Alaxan", Price = 30, RequiresPrescription = NO },
        new Medicine { Name = "Dolfenal", Price = 40, RequiresPrescription = NO },
        new Medicine { Name = "Residol", Price = 35, RequiresPrescription = NO },
        new Medicine { Name = "Tuseran Forte", Price = 45, RequiresPrescription = NO },


        new Medicine { Name = "Myracof Tablet", Price = 60, RequiresPrescription = NO },
        new Medicine { Name = "Solmux Advance", Price = 40, RequiresPrescription = YES },
        new Medicine { Name = "Tuseran Forte", Price = 45, RequiresPrescription = NO }, // Duplicate for another category

        
        new Medicine { Name = "Diatabs", Price = 25, RequiresPrescription = NO },
        new Medicine { Name = "Imodium", Price = 20, RequiresPrescription = NO },
        new Medicine { Name = "Flotera Power", Price = 50, RequiresPrescription = NO },
        new Medicine { Name = "Kremil S", Price = 15, RequiresPrescription = NO },


        new Medicine { Name = "Alaxan", Price = 30, RequiresPrescription = NO }, // Duplicate for another category
        new Medicine { Name = "Dolfenal Advance", Price = 55, RequiresPrescription = NO },
        new Medicine { Name = "Medicol", Price = 25, RequiresPrescription = NO },
        new Medicine { Name = "Skelan", Price = 35, RequiresPrescription = NO }
    });
    }

    static void DisplayMedicinesByCategory()
    {
        Console.WriteLine("\n--- Medicines By Category ---");

        // Load inventory data directly from the inventory database
        string inventoryFilePath = @"C:\Users\Christian Paul\Documents\Project File Handling data\inventory.txt";

        if (File.Exists(inventoryFilePath))
        {
            var lines = File.ReadAllLines(inventoryFilePath);

            foreach (var line in lines)
            {
                var parts = line.Split(',');

                if (parts.Length >= 3) // Ensure the line contains at least name, price, and prescription info
                {
                    string name = parts[0];
                    string price = parts[1];
                    string prescriptionRequired = parts[2].Trim().ToLower() == "yes" ? "Yes" : "No";

                    Console.WriteLine($"- {name}, Price: ₱ {price}, Prescription Required: {prescriptionRequired}");
                }
            }
        }
        else
        {
            Console.WriteLine("Inventory database file not found.");
        }
    }

    static Medicine SearchMedicine(string searchTerm)
    {
        string inventoryFilePath = @"C:\Users\Christian Paul\Documents\Project File Handling data\inventory.txt";

        try
        {
            if (File.Exists(inventoryFilePath))
            {
                var lines = File.ReadAllLines(inventoryFilePath);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 4) // Ensure the line contains name, price, prescription, and stock
                    {
                        string name = parts[0];
                        decimal price = decimal.Parse(parts[1]);
                        bool requiresPrescription = parts[2].Trim().ToLower() == "yes";
                        int stock = int.Parse(parts[3]); // Stock is the fourth column

                        // Check if the search term matches the medicine name
                        if (name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        {
                            if (stock > 0)
                            {
                                // Return the medicine details if it has stock
                                return new Medicine
                                {
                                    Name = name,
                                    Price = price,
                                    RequiresPrescription = requiresPrescription
                                };
                            }
                            else
                            {
                                Console.WriteLine($"Medicine {name} is out of stock.");
                                return null; // Indicate that the medicine exists but is out of stock
                            }
                        }
                    }
                }

                Console.WriteLine("Medicine not found in inventory.");
            }
            else
            {
                Console.WriteLine("Inventory database file not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while searching for the medicine: {ex.Message}");
        }

        return null; // Return null if no matching medicine is found
    }


    static string ReadPassword()
    {
        string password = string.Empty;
        ConsoleKey key;
        do
        {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && password.Length > 0)
            {
                Console.Write("\b \b");
                password = password.Substring(0, password.Length - 1);
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                Console.Write("*");
                password += keyInfo.KeyChar;
            }
        } while (key != ConsoleKey.Enter);

        Console.WriteLine();
        return password;
    }

    static bool RegisterUser()
    {
        User user = new User();
        Console.Write("Enter First Name: ");
        user.FirstName = Console.ReadLine();
        Console.Write("Enter Last Name: ");
        user.LastName = Console.ReadLine();
        Console.Write("Enter Email: ");
        user.Email = Console.ReadLine();
        Console.Write("Create Password: ");
        user.SetPassword(ReadPassword()); // Use the ReadPassword method to read the password securely
        users.Add(user);
        user.SaveToFile();
        return true;
    }

    static bool LoginUser(out User loggedInUser)
    {
        Console.Write("Enter Email: ");
        string email = Console.ReadLine();
        Console.Write("Enter Password: ");
        string password = ReadPassword(); // Use the ReadPassword method to read the password securely

        // Check if the credentials match the admin credentials
        if (email == "admin" && password == "233477395")
        {
            loggedInUser = null; // No need to return a User object for admin
            Console.WriteLine("Admin logged in successfully!");
            Admin.AdminMenu(); // Redirect to the Admin Menu
            return true;
        }

        // Check against the regular users
        foreach (var user in users)
        {
            if (user.Authenticate(email, password))
            {
                loggedInUser = user;
                return true;
            }
        }

        loggedInUser = null;
        Console.WriteLine("Invalid email or password.");
        return false;
    }


    static void SavePharmacyDetails(string name, string location, string filePath)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"Pharmacy Name: {name}");
                writer.WriteLine($"Location: {location}");
            }
            Console.WriteLine("Pharmacy details saved successfully.");
            Console.ReadKey();
            Console.Clear();
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while saving pharmacy details: " + ex.Message);
        }
    }
    static decimal CalculateUpdatedTotalCost(List<(Medicine medicine, int quantity)> reservedMedicines, string inventoryFilePath)
    {
        decimal totalCost = 0;

        foreach (var (medicine, quantity) in reservedMedicines)
        {
            decimal updatedPrice = GetUpdatedPriceFromInventory(medicine.Name, inventoryFilePath);
            totalCost += updatedPrice * quantity;

            // Update the medicine's price with the latest value from inventory
            medicine.Price = updatedPrice;
        }

        return totalCost;
    }

    static decimal GetUpdatedPriceFromInventory(string medicineName, string inventoryFilePath)
    {
        try
        {
            if (File.Exists(inventoryFilePath))
            {
                var lines = File.ReadAllLines(inventoryFilePath);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts[0].Equals(medicineName, StringComparison.OrdinalIgnoreCase))
                    {
                        return decimal.Parse(parts[1]); // Assuming price is the second column
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while fetching the price for {medicineName}: {ex.Message}");
        }

        return 0; // Default to 0 if not found or error occurs
    }

}