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
    public static void EditContact(int id, string? newName, string? newPhone)
    {
        using var connection = new SQLiteConnection($"Data Source={contactsdb};Version=3;");
        connection.Open();

        string getQuery = "SELECT name, phone FROM contacts WHERE id = @id";
        using var getCommand = new SQLiteCommand(getQuery, connection);
        getCommand.Parameters.AddWithValue("@id", id);
        using var reader = getCommand.ExecuteReader();

        if (!reader.Read())
        {
            Console.WriteLine("Contact not found.");
            return;
        }

        string currentName = reader.GetString(0);
        string currentPhone = reader.GetString(1);

        string finalName = string.IsNullOrWhiteSpace(newName) ? currentName : newName;
        string finalPhone = string.IsNullOrWhiteSpace(newPhone) ? currentPhone : newPhone;

        string updateQuery = "UPDATE contacts SET name = @name, phone = @phone WHERE id = @id";
        using var updateCommand = new SQLiteCommand(updateQuery, connection);
        updateCommand.Parameters.AddWithValue("@name", finalName);
        updateCommand.Parameters.AddWithValue("@phone", finalPhone);
        updateCommand.Parameters.AddWithValue("@id", id);

        int rowsAffected = updateCommand.ExecuteNonQuery();
        Console.WriteLine(rowsAffected > 0 ? "Contact updated!" : "Update failed.");
    }
    public static bool IsValidPhoneNumber(string phone)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\+?[0-9\s\-()]{5,20}$");
    }

    public static bool ContactExists(string name, string phone)
    {
        using var connection = new SQLiteConnection($"Data Source={contactsdb};Version=3;");
        connection.Open();

        string query = "SELECT COUNT(*) FROM Contacts WHERE Name = @name AND Phone = @phone";
        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@phone", phone);

        long count = (long)command.ExecuteScalar();
        return count > 0;
    }

}