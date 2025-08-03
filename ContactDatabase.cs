using System;
using System.Data.SQLite;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class ContactDatabase
{
        public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Phone { get; set; } = "";
    }
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
        try
        {
            using var connection = new SQLiteConnection($"Data Source={contactsdb};Version=3;");
            connection.Open();

            using var command = new SQLiteCommand("INSERT INTO Contacts (Name, Phone) VALUES (@name, @phone)", connection);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@phone", phone);
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] Failed to add contact: {ex.Message}");
        }
    }

    public static void ListContacts()
    {
        try
        {
            using var connection = new SQLiteConnection($"Data Source={contactsdb};Version=3;");
            connection.Open();

            using var command = new SQLiteCommand("SELECT Id, Name, Phone FROM Contacts", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"{reader["Id"]}: {reader["Name"]} - {reader["Phone"]}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] Failed to list contacts: {ex.Message}");
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
    public static void SearchContacts(string term)
    {
        using var connection = new SQLiteConnection($"Data Source={contactsdb};Version=3;");
        connection.Open();

        string query = "SELECT Id, Name, Phone FROM Contacts WHERE Name LIKE @term OR Phone LIKE @term";
        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@term", $"%{term}%");

        using var reader = command.ExecuteReader();
        Console.WriteLine("\nSearch Results:");
        while (reader.Read())
        {
            Console.WriteLine($"ID: {reader["Id"]} | Name: {reader["Name"]} | Phone: {reader["Phone"]}");
        }
    }
    public static void ExportToJson()
    {
        var contacts = new List<Contact>();

        using var connection = new SQLiteConnection($"Data Source={contactsdb};Version=3;");
        connection.Open();

        string query = "SELECT Id, Name, Phone FROM Contacts";
        using var command = new SQLiteCommand(query, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            contacts.Add(new Contact
            {
                Id = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"].ToString() ?? "",
                Phone = reader["Phone"].ToString() ?? ""
            });
        }

        try
        {
            var json = JsonSerializer.Serialize(contacts, new JsonSerializerOptions { WriteIndented = true });
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "contacts_export.json");
            File.WriteAllText(filePath, json);
            Console.WriteLine($"Contacts exported to: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] Failed to export contacts: {ex.Message}");
        }
    }

}