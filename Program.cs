using System;

class  Program
{
    static  void  Main()
    {
        ContactDatabase.Initialize();

        while  (true)
        {
            Console.WriteLine("\nSQLite Contact Book");
            Console.WriteLine("1. Add Contact");
            Console.WriteLine("2. List Contacts");
            Console.WriteLine("3. Delete Contact");
            Console.WriteLine("4. Edit Contact");
            Console.WriteLine("5. Search Contacts");
            Console.WriteLine("6. Export Contacts to JSON");
            Console.WriteLine("7. Exit");

            Console.Write("Choose an option: ");
            var choice = Console.ReadLine();

            switch(choice)
            {
                case "1":
                    Console.Write("Name: ");
                    string? name = Console.ReadLine();

                    Console.Write("Phone: ");
                    string? phone = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone))
                    {
                        Console.WriteLine("Name and phone cannot be empty!");
                    }
                    else if (!ContactDatabase.IsValidPhoneNumber(phone))
                    {
                        Console.WriteLine("Invalid phone number format!");
                    }
                    else if (ContactDatabase.ContactExists(name, phone))
                    {
                        Console.WriteLine("Contact already exists!");
                    }
                    else
                    {
                        ContactDatabase.AddContact(name, phone);
                    }
                    break;

                case "2":
                    ContactDatabase.ListContacts();
                    break;

                case "3":
                    Console.Write("Enter Contact ID to delete: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    {
                        ContactDatabase.DeleteContact(id);
                    }
                    else
                    {
                        Console.WriteLine("Invalid ID.");
                    }
                    break;

                case "4":
                    Console.Write("Enter Contact ID to edit: ");
                    if (int.TryParse(Console.ReadLine(), out int editId))
                    {
                        Console.Write("Enter new name (or press Enter to keep current): ");
                        string? newName = Console.ReadLine();

                        Console.Write("Enter new phone (or press Enter to keep current): ");
                        string? newPhone = Console.ReadLine();

                        ContactDatabase.EditContact(editId, newName, newPhone);               
                    }
                    else
                    {
                        Console.WriteLine("Invalid ID.");
                    }
                    break;

                case "5":
                    Console.Write("Enter search term: ");
                    string? term = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(term))
                    {
                        ContactDatabase.SearchContacts(term);
                    }
                    else
                    {
                        Console.WriteLine("Search term cannot be empty.");
                    }
                    break;

                case "6":
                    ContactDatabase.ExportToJson();
                    break;

                case "7":
                    return;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}
