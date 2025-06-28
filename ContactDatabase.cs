using System;
using System.Data.SQLite;
using System.IO;

public class ContactDatabase
{
    private static string contactsdb = "contacts.db";

    public static void Initialize()
    {
        if (!File.Exists(contactsdb))
        {
            SQLiteConnection.CreateFile(contactsdb);
        }

        using var connection = new SQLiteConnection($"Data Source={contactsdb};Version=3;");
        connection.Open();

        string tableCommand = @"CREATE TABLE IF NOT EXISTS Contacts (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Name TEXT NOT NULL,
                                    Phone TEXT NOT NULL
                                );";

        using var command = new SQLiteCommand(tableCommand, connection);
        command.ExecuteNonQuery();
    }

    public static void AddContact(string name, string phone)
    {
        using var connection = new SQLiteConnection($"Data Source={contactsdb};Version=3;");
        connection.Open();

        string insertQuery = "INSERT INTO Contacts (Name, Phone) VALUES (@name, @phone);";
        using var command = new SQLiteCommand(insertQuery, connection);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@phone", phone);

        command.ExecuteNonQuery();
        Console.WriteLine("Contact added.");
    }

    public static void ListContacts()
    {
        using var connection = new SQLiteConnection($"Data Source={contactsdb};Version=3;");
        connection.Open();

        string selectQuery = "SELECT * FROM Contacts;";
        using var command = new SQLiteCommand(selectQuery, connection);
        using var reader = command.ExecuteReader();

        Console.WriteLine("\nContacts:");
        while (reader.Read())
        {
            Console.WriteLine($"{reader["Id"]}: {reader["Name"]} - {reader["Phone"]}");
        }
    }

    public static void DeleteContact(int id)
    {
        using var connection = new SQLiteConnection($"Data Source={contactsdb};Version=3;");
        connection.Open();

        string deleteQuery = "DELETE FROM Contacts WHERE Id = @id;";
        using var command = new SQLiteCommand(deleteQuery, connection);
        command.Parameters.AddWithValue("@id", id);

        int deleted = command.ExecuteNonQuery();
        if (deleted > 0)
            Console.WriteLine("Contact deleted.");
        else
            Console.WriteLine("Contact not found.");
    }
}