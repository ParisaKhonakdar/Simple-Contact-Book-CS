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
            Console.WriteLine("4. Exit");

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
                    return;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}
