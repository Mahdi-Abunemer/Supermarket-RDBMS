using MySqlConnector;
using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using DB_Project2024; 

var connectionString = "Server=your_server;Port=your_port_number;Database=your_RDBM;Uid=;Pwd=;";

while (true)
{
    Console.WriteLine("Write down your command:");
    var command = Console.ReadLine().Split(" ", StringSplitOptions.RemoveEmptyEntries);
    if (command.Length == 0)
        break; // Giving an empty command exits the application

    var connection = new MySqlConnection(connectionString);
    connection.Open();
    Queriescs query = new Queriescs();
    switch (command[0])
    {
        case "add_employee":
            query.AddEmployee(connection, command);
            break;

        case "end_all_offers":
            query.end_all_offers(connection,command);
            break;
                
        case "get_top_suppliers":
            query.get_top_suppliers(connection); 
            break;

        case "lost_customers":
            query.lost_customers(connection); 
            break;
        case "remove_cat_from_offer":
            query.remove_cat_from_offer(connection ,command); 
            break;
        case "inflation":
            query.inflation(connection ,command);
            break;
        case "empty_categories":
            query.empty_categories(connection ,command);
            break;
        case "category_loops":
            query.category_loops(connection ,command);
            break;
        case "self_buyers":
            query.self_buyers(connection ,command);
            break;
        case "bestsellers":
            query.bestsellers(connection ,command);
            break;
        default:
            query.IncorrectCommand(connection, command);
            break;
    }

    Console.WriteLine(String.Empty);
    connection.Close();
}

