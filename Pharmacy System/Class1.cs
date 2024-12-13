using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy_System
{
    internal class Class1
    {
        public class Program
        {
            
        }
        public class SalesReport
        {
            public string TransactionId { get; set; }
            public string ProductName { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }
            public DateTime Date { get; set; }
            public double Total { get; set; }
        }
        public class Admin
        {
            private static readonly string inventoryFilePath = @"C:\Users\Christian Paul\Documents\Project File Handling data\inventory.txt";
            private static readonly string receiptsFilePath = @"C:\Users\Christian Paul\Documents\Project File Handling data\receipts.txt";
            private static readonly string salesReportFilePath = @"C:\Users\Christian Paul\Documents\Project File Handling data\sales_report.txt";
            private static readonly string usersFilePath = @"C:\Users\Christian Paul\Documents\Project File Handling data\users.txt";

            public static void AdminMenu()
            {
                string choice;

                do
                {
                    Console.WriteLine("\n--- Admin Management Menu ---");
                    Console.WriteLine("1. Modify User Details");
                    Console.WriteLine("2. View Inventory");
                    Console.WriteLine("3. Modify Inventory");
                    Console.WriteLine("4. Delete Product");
                    Console.WriteLine("5. Add Product");
                    Console.WriteLine("6. View Receipts");
                    Console.WriteLine("7. View Monthly Sales Report");
                    Console.WriteLine("8. Exit");
                    Console.Write("Enter your choice: ");
                    choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            ModifyUserDetails();
                            break;
                        case "2":
                            ViewInventory();
                            break;
                        case "3":
                            ModifyInventory();
                            break;
                        case "4":
                            DeleteProduct();
                            break;
                        case "5":
                            AddProduct();
                            break;
                        case "6":
                            ViewReceipts();
                            break;
                        case "7":
                            ViewMonthlySalesReport();
                            break;
                        case "8":
                            Console.WriteLine("Exiting Admin Menu...");
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                } while (choice != "8");
            }

            public static void ModifyUserDetails()
            {
                Console.Write("Enter the email of the user to modify: ");
                string email = Console.ReadLine();

                if (File.Exists(usersFilePath))
                {
                    var lines = File.ReadAllLines(usersFilePath).ToList();
                    bool userFound = false;

                    for (int i = 0; i < lines.Count; i++)
                    {
                        var parts = lines[i].Split(',');

                        if (parts.Length >= 4 && parts[2].Equals(email, StringComparison.OrdinalIgnoreCase))
                        {
                            userFound = true;

                            Console.Write("Enter new first name: ");
                            parts[0] = Console.ReadLine();
                            Console.Write("Enter new last name: ");
                            parts[1] = Console.ReadLine();
                            Console.Write("Enter new password: ");
                            parts[3] = Console.ReadLine();

                            lines[i] = string.Join(",", parts);
                            break;
                        }
                    }

                    if (userFound)
                    {
                        File.WriteAllLines(usersFilePath, lines);
                        Console.WriteLine("User details updated successfully.");
                    }
                    else
                    {
                        Console.WriteLine("User not found.");
                    }
                }
                else
                {
                    Console.WriteLine("Users file does not exist.");
                }
            }

            public static void ViewInventory()
            {
                if (File.Exists(inventoryFilePath))
                {
                    Console.WriteLine("\n--- Inventory ---");
                    var lines = File.ReadAllLines(inventoryFilePath); // Always fetch fresh data from the file

                    if (lines.Length == 0)
                    {
                        Console.WriteLine("Inventory is empty.");
                        return;
                    }

                    foreach (var line in lines)
                    {
                        var parts = line.Split(',');
                        if (parts.Length >= 5) // Adjusted for the expiration date field
                        {
                            string productName = parts[0];
                            string price = parts[1];
                            string prescription = parts[2].ToLower() == "yes" ? "Required" : "Not Required";
                            string stock = parts[3];
                            string expirationDate = parts[4]; // Added expiration date

                            Console.WriteLine($"Name: {productName}, Price: {price}, Prescription: {prescription}, Stock: {stock}, Expiration Date: {expirationDate}");
                        }
                        else
                        {
                            Console.WriteLine("Invalid inventory record found. Please check the file format.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Inventory file does not exist.");
                }
            }


            public static void ModifyInventory()
            {
                Console.Write("Enter the product name to modify: ");
                string productName = Console.ReadLine();

                if (File.Exists(inventoryFilePath))
                {
                    var lines = File.ReadAllLines(inventoryFilePath).ToList();
                    bool productFound = false;

                    for (int i = 0; i < lines.Count; i++)
                    {
                        var parts = lines[i].Split(',');

                        if (parts[0].Equals(productName, StringComparison.OrdinalIgnoreCase))
                        {
                            productFound = true;

                            Console.Write("Enter new price: ");
                            parts[1] = Console.ReadLine();

                            Console.Write("Requires prescription (yes/no): ");
                            string prescriptionInput = Console.ReadLine().ToLower();
                            parts[2] = prescriptionInput == "yes" ? "yes" : "no";

                            Console.Write("Enter new stock quantity: ");
                            parts[3] = Console.ReadLine();

                            Console.Write("Enter new expiration date (yyyy-MM-dd): ");
                            parts[4] = Console.ReadLine();

                            lines[i] = string.Join(",", parts);
                            Console.WriteLine($"Product found: {productName} - Updating details...");
                            break;
                        }
                    }

                    if (productFound)
                    {
                        Console.WriteLine("Saving changes to the inventory...");
                        File.WriteAllLines(inventoryFilePath, lines); // Ensure updated inventory is saved
                        Console.WriteLine("Changes saved successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Product not found.");
                    }
                }
                else
                {
                    Console.WriteLine("Inventory file does not exist.");
                }
            }

            public static void DeleteProduct()
            {
                Console.Write("Enter the product name to delete: ");
                string productName = Console.ReadLine();

                if (File.Exists(inventoryFilePath))
                {
                    var lines = File.ReadAllLines(inventoryFilePath).ToList();
                    bool productFound = false;

                    for (int i = 0; i < lines.Count; i++)
                    {
                        var parts = lines[i].Split(',');

                        if (parts[0].Equals(productName, StringComparison.OrdinalIgnoreCase))
                        {
                            lines.RemoveAt(i);
                            productFound = true;
                            break;
                        }
                    }

                    if (productFound)
                    {
                        File.WriteAllLines(inventoryFilePath, lines);
                        Console.WriteLine("Product deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Product not found.");
                    }
                }
                else
                {
                    Console.WriteLine("Inventory file does not exist.");
                }
            }

            public static void AddProduct()
            {
                Console.Write("Enter Product Name: ");
                string name = Console.ReadLine();

                Console.Write("Enter Price: ");
                string price = Console.ReadLine();

                Console.Write("Requires Prescription (yes/no): ");
                string prescriptionInput = Console.ReadLine().ToLower();
                string requiresPrescription = prescriptionInput == "yes" ? "yes" : "no";

                Console.Write("Enter Stock Quantity: ");
                string stock = Console.ReadLine();

                Console.Write("Enter Expiration Date (yyyy-MM-dd): ");
                string expirationDate = Console.ReadLine();

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(inventoryFilePath));
                    using (StreamWriter writer = new StreamWriter(inventoryFilePath, true))
                    {
                        writer.WriteLine($"{name},{price},{requiresPrescription},{stock},{expirationDate}");
                    }
                    Console.WriteLine("Product added successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while adding the product: " + ex.Message);
                }
            }

            public static void ViewReceipts()
            {
                if (File.Exists(receiptsFilePath))
                {
                    Console.WriteLine("\n--- Receipts ---");
                    var lines = File.ReadAllLines(receiptsFilePath);

                    foreach (var line in lines)
                    {
                        // Ensure all "?" are replaced with "₱"
                        if (line.Contains("₱"))
                        {
                            Console.WriteLine(line.Replace("?", "₱"));
                        }
                        else
                        {
                            Console.WriteLine(line);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Receipts file does not exist.");
                }
            }

            public static void ViewMonthlySalesReport()
            {
                if (File.Exists(salesReportFilePath))
                {
                    Console.WriteLine("\n--- Monthly Sales Report ---");

                    var lines = File.ReadAllLines(salesReportFilePath);
                    var salesData = new List<SalesReport>();

                    foreach (var line in lines)
                    {
                        var parts = line.Split(',');
                        if (parts.Length >= 6)
                        {
                            salesData.Add(new SalesReport
                            {
                                TransactionId = parts[0],
                                ProductName = parts[1],
                                Price = Convert.ToDouble(parts[2]),
                                Quantity = Convert.ToInt32(parts[3]),
                                Date = DateTime.ParseExact(parts[4], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                Total = Convert.ToDouble(parts[5])
                            });
                        }
                    }

                    // Sort sales data by date before filtering
                    salesData = salesData.OrderBy(s => s.Date).ToList();

                    GenerateMonthlySalesReport(salesData);
                }
                else
                {
                    Console.WriteLine("Sales report file does not exist.");
                }
            }


            public static void GenerateMonthlySalesReport(List<SalesReport> salesData)
            {
                DateTime today = DateTime.Now;
                DateTime oneMonthAgo = today.AddDays(-30);

                // Filter sales data for the last 30 days
                var monthlySales = salesData
                    .Where(s => s.Date.Date >= oneMonthAgo.Date && s.Date.Date <= today.Date)
                    .ToList();

                // Calculate total income for the month
                double monthlyTotalIncome = monthlySales.Sum(s => s.Total);

                // Display the monthly sales report
                if (monthlySales.Any())
                {
                    Console.WriteLine("\n--- Monthly Sales Report ---");
                    foreach (var sale in monthlySales)
                    {
                        Console.WriteLine($"Transaction ID: {sale.TransactionId}, Product: {sale.ProductName}, Total: {sale.Total}, Date: {sale.Date.ToShortDateString()}");
                    }
                    Console.WriteLine($"\nMonthly Total Income: {monthlyTotalIncome:C}");
                }
                else
                {
                    Console.WriteLine("No sales in the past month.");
                }
            }
        }

    }
}
