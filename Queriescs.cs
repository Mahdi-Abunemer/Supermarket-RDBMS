using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Project2024
{
    internal class Queriescs
    {
       public void AddEmployee(MySqlConnection connection, string[] command)
        {
            // Expected command syntax: add_employee <name> <position> <salary> <phone> <supermarket_id>
            if (command.Length != 6)
            {
                Console.WriteLine("Incorrect syntax! Expected: add_employee <name> <position> <salary> <phone> <supermarket_id>");
                return;
            }

            try
            {
                var name = command[1];
                var position = command[2];
                var salary = decimal.Parse(command[3]);

                var phone = command[4];
                var supermarketId = int.Parse(command[5]);
                var hireDate = DateTime.Now.ToString("yyyy-MM-dd");

                var cmd = new MySqlCommand(
                    "INSERT INTO employee (name, Position, salary, phone, SupermarketId, HireDate) " +
                    "VALUES (@name, @position, @salary, @phone, @supermarket_id, @hire_date)", connection);

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@position", position);
                cmd.Parameters.AddWithValue("@salary", salary);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@supermarket_id", supermarketId);
                cmd.Parameters.AddWithValue("@hire_date", hireDate);

                cmd.Prepare();
                cmd.ExecuteNonQuery();
                Console.WriteLine("Employee created!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

       public void end_all_offers(MySqlConnection connection, string[] command)
        {

            if (command.Length != 1)
            {
                Console.WriteLine("Incorrect syntax! Expected: end_all_offers");
                return;
            }

            var cmd = new MySqlCommand(
                     "UPDATE offers " +
                "SET EndDate = CURDATE() " +
                "WHERE EndDate > CURDATE() OR EndDate IS NULL " +
                "OR EndDate < CURDATE();", connection);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            Console.WriteLine("End all offers Today!");

        }

       public void get_top_suppliers(MySqlConnection connection)
        {
            var cmd = new MySqlCommand(
                     "SELECT supplier.id,  SUM(items.price) AS total_price " +
                     "FROM items " +
                     "INNER JOIN supplier ON items.SupplierId = supplier.id " +
                     "GROUP BY supplier.id " +
                     "ORDER BY total_price DESC " +
                     "LIMIT 10;"
                     , connection);

            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
            // Execute the query and get the result set
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                Console.WriteLine("Top 10 Suppliers (by total supplied price):");
                while (reader.Read())
                {

                    int supplierId = reader.GetInt32("id");

                    decimal totalPrice = reader.GetDecimal("total_price");

                    Console.WriteLine($"Supplier ID: {supplierId}, " +
                        $"Total Price: {totalPrice:C}");
                }
            }
            else
            {
                Console.WriteLine("No suppliers found.");
            }

            reader.Close();
            connection.Close();
        }

       public void lost_customers(MySqlConnection connection)
        {
            DateTime oneMonthAgo = DateTime.Now.AddMonths(-1);

            // better to use "WHERE orders.Date < DATE_ADD(CURDATE(), INTERVAL -1 MONTH) " +
            // and not  to use DateTime 
            var cmd = new MySqlCommand(
                     "SELECT customer.id, " +
                     "count(orders.CustemerId) AS total_order ,orders.Date " +
                     "FROM orders " +
                     "INNER JOIN customer  ON customer.id = orders.CustemerId " +
                     "WHERE orders.Date < DATE_ADD(CURDATE(), INTERVAL -1 MONTH) " +
                     "GROUP BY customer.id " +
                     "having total_order = 1;"
                     , connection);

            cmd.Parameters.AddWithValue("@OneMonthAgo", oneMonthAgo);

            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            // Execute the query and get the result set
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                Console.WriteLine("Who only made one order and it happen more than a month ago):");
                while (reader.Read())
                {

                    int coustomerId = reader.GetInt32("id");

                    DateTime date = reader.GetDateTime("date");

                    Console.WriteLine($"Customer ID: {coustomerId} , Date: {date}");

                }
            }
            else
            {
                Console.WriteLine("No customers found.");
            }
            reader.Close();
            connection.Close();
        }

       public void remove_cat_from_offer(MySqlConnection connection, string[] command)
        {
            if (command.Length != 3)
            {
                Console.WriteLine("Incorrect syntax! Expected: add_employee <CategoryName> <OfferId> ");
                return;
            }

            try
            {
                var CategoryName = command[1];
                var OfferId = command[2];

                var cmd = new MySqlCommand(
                    "DELETE io " +
                    "FROM itemsoffers io " +
                    "JOIN items i ON io.ItemsId = i.id " +
                    "JOIN category c ON i.CategoryId = c.id " +
                    "WHERE c.name = @CategoryName " +
                    "AND io.OffersId = @OfferId;"
                    , connection
                    );

                cmd.Parameters.AddWithValue("@CategoryName", CategoryName);
                cmd.Parameters.AddWithValue("@OfferId", OfferId);


                cmd.Prepare();
                cmd.ExecuteNonQuery();
                Console.WriteLine("remove_cat_from_offer Done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

       public void inflation(MySqlConnection connection, string[] command)
        {
            if (command.Length != 2)
            {
                Console.WriteLine("Incorrect syntax! Expected: add_employee <ItemId> ");
                return;
            }

            try
            {
                var ItemId = command[1];

                var cmd = new MySqlCommand(
                    "SELECT DISTINCT s.price, o.`Date` " +
                    "FROM sales s " +
                    "JOIN items i ON s.itemId = i.id " +
                    "JOIN orders o ON s.OrderId = o.id " +
                    "WHERE i.id = @ItemId " +
                    "ORDER BY o.`Date` ASC;"
                    , connection
                    );

                cmd.Parameters.AddWithValue("@ItemId", ItemId);

                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }
                // Execute the query and get the result set
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {

                    while (reader.Read())
                    {

                        decimal Price = reader.GetInt32("price");
                        DateTime date = reader.GetDateTime("date");


                        Console.WriteLine($"Price: {Price} , Date from old to new {date}");
                    }
                }
                else
                {
                    Console.WriteLine("No prices found.");
                }

                reader.Close();
                connection.Close();

                cmd.Prepare();
                cmd.ExecuteNonQuery();
                Console.WriteLine("Inflation has been  Done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

       public void empty_categories(MySqlConnection connection, string[] command)
        {
            if (command.Length != 1)
            {
                Console.WriteLine("Incorrect syntax! Expected: empty_categories ");
                return;
            }

            try
            {
                var cmd = new MySqlCommand(
                    "select id , name " +
                    "FROM supermarket.category c " +
                    "WHERE c.id NOT IN " +
                    "(SELECT ParentId FROM supermarket.category) " +
                    "And c.id NOT IN (SELECT CategoryId FROM supermarket.items);"
                    , connection
                    );
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }

                // Execute the query and get the result set
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                        decimal id = reader.GetInt32("id");
                        string name = reader.GetString("name");


                        Console.WriteLine($"id: {id} , name {name}");
                    }
                }
                else
                {
                    Console.WriteLine("No prices found.");
                }

                reader.Close();
                connection.Close();

                cmd.Prepare();
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

       public void category_loops(MySqlConnection connection, string[] command)
        {
            if (command.Length != 1)
            {
                Console.WriteLine("Incorrect syntax! Expected: category_loops");
            }
            try
            {
                var cmd = new MySqlCommand(
                    "SELECT c1.id, c1.name " +
                    "FROM supermarket.category c1 " +
                    "JOIN supermarket.category c2 ON c1.id = c2.ParentId " +
                    "WHERE c2.id = c1.ParentId;"
                    , connection
                    );
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }

                // Execute the query and get the result set
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        decimal id = reader.GetInt32("id");
                        string name = reader.GetString("name");
                        Console.WriteLine($"id: {id} , name {name}");
                    }
                }
                else
                {
                    Console.WriteLine("No prices found.");
                }

                reader.Close();
                connection.Close();

                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

       public void self_buyers(MySqlConnection connection, string[] command)
        {
            if (command.Length != 1)
            {
                Console.WriteLine("Incorrect syntax! Expected: self_buyers");
            }
            try
            {
                var cmd = new MySqlCommand(
                    "SELECT o.id AS OrderId, " +
                    "c.id AS CustomerId, " +
                    "c.phone AS phone, " +
                    "s.phone AS phone " +
                    "FROM sales sa " +
                    "JOIN orders o ON sa.OrderId = o.id " +
                    "JOIN items i ON sa.itemId = i.id " +
                    "JOIN supplier s ON i.SupplierId = s.id " +
                    "JOIN customer c ON o.CustemerId = c.id " +
                    "WHERE s.phone = c.phone;"
                    , connection
                    );



                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }

                // Execute the query and get the result set
                MySqlDataReader reader = cmd.ExecuteReader();


                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string customer_phone = reader.GetString("phone");
                        string supplier_phone = reader.GetString("phone");
                        Console.WriteLine
                            ($"customer_phone: {customer_phone} , supplier_phone {supplier_phone}");
                    }
                }
                else
                {
                    Console.WriteLine("No self_buyers found.");
                }

                reader.Close();
                connection.Close();

                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

       public void bestsellers(MySqlConnection connection, string[] command)
        {
            if (command.Length != 3)
            {
                Console.WriteLine("Invalid number of parameters. Expected: bestsellers [report_type] [limit]");
                return;
            }

            try
            {
                string reportType = command[1].ToLower();
                int limit;
                if (!int.TryParse(command[2], out limit))
                {
                    Console.WriteLine("Invalid limit. Please provide a numeric value.");
                    return;
                }

                if (reportType != "quantity" && reportType != "money")
                {
                    Console.WriteLine("Invalid report type. Expected 'quantity' or 'money'.");
                    return;
                }

                string query = @"
            SELECT 
                s.itemId, 
                i.name AS ItemName, 
                CASE 
                    WHEN @report_type = 'quantity' THEN SUM(s.quantity)
                    WHEN @report_type = 'money' THEN SUM(s.`Sub Total`)
                END AS ReportValue
            FROM supermarket.sales s
            JOIN supermarket.items i ON s.itemId = i.id
            GROUP BY s.itemId
            ORDER BY ReportValue DESC
            LIMIT @limit;
        ";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@report_type", reportType);
                cmd.Parameters.AddWithValue("@limit", limit);

                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }

                // Execute the query and read results
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Console.WriteLine("Top Items:");
                        while (reader.Read())
                        {
                            string itemID = reader["itemId"].ToString();
                            string itemName = reader["ItemName"].ToString();
                            string reportValue = reader["ReportValue"].ToString();

                            Console.WriteLine($"ItemID: {itemID}, ItemName: {itemName}, ReportValue: {reportValue}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No results found.");
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

       public void IncorrectCommand(MySqlConnection connection, string[] command)
        {
            Console.WriteLine("No such command!");
        }
    }
}
