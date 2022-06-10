using System;
using System.Collections.Generic;
using System.Linq;
using Roommates.Models;
using Roommates.Repositories;

namespace Roommates
{
    class Program
    {
        //  This is the address of the database.
        //  We define it here as a constant since it will never change.
        private const string CONNECTION_STRING = @"server=localhost\SQLExpress;database=Roommates;integrated security=true;TrustServerCertificate=true;";

        static void Main(string[] args)
        {
            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);
            ChoreRepository choreRepo = new ChoreRepository(CONNECTION_STRING);
            RoommateRepository roommateRepo = new RoommateRepository(CONNECTION_STRING);

            bool runProgram = true;
            while (runProgram)
            {
                string selection = GetMenuSelection();

                switch (selection)
                {
                    case ("Show all rooms"):
                        List<Room> rooms = roomRepo.GetAll();
                        foreach (Room r in rooms)
                        {
                            Console.WriteLine($"{r.Name} has an Id of {r.Id} and a max occupancy of {r.MaxOccupancy}");
                        }
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Show all chores"):
                        List<Chore> chores = choreRepo.GetAll();
                        foreach (var chore in chores)
                        {
                            Console.WriteLine($"{chore.Name} has an Id of {chore.Id}");
                        }

                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Get chore counts"):
                        Dictionary<string, int> counts = choreRepo.GetChoreCounts();
                        foreach (var kvp in counts)
                        {
                            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                        }

                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Show all unassigned chores"):
                        chores = choreRepo.GetUnassignedChores();
                        foreach (var chore in chores)
                        {
                            Console.WriteLine($"{chore.Id} - {chore.Name} is unassigned");
                        }

                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Assign chore"):
                        chores = choreRepo.GetAll();
                        foreach (var chore in chores)
                        {
                            Console.WriteLine($"{chore.Id} - {chore.Name}");
                        }
                        Console.WriteLine("Which chore would you like to assign?");
                        Console.Write(">");
                        int choreId = int.Parse(Console.ReadLine());

                        List<Roommate> roommates = roommateRepo.GetAll();
                        foreach (var roommate in roommates)
                        {
                            Console.WriteLine($"{roommate.Id} - {roommate.FirstName} {roommate.LastName}");
                        }
                        Console.WriteLine("Which roommate would you like to the chore?");
                        Console.Write(">");
                        int roommateId = int.Parse(Console.ReadLine());

                        choreRepo.AssignChore(choreId, roommateId);

                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Search for room"):
                        Console.Write("Room Id: ");
                        try
                        {
                            int id = int.Parse(Console.ReadLine());

                            Room room = roomRepo.GetById(id);
                            Console.WriteLine($"{room.Id} - {room.Name} Max Occupancy({room.MaxOccupancy})");
                            Console.Write("Press any key to continue");
                            Console.ReadKey();
                            break;
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    case ("Search for chore"):
                        Console.Write("Chore Id: ");
                        try
                        {
                            int id = int.Parse(Console.ReadLine());

                            Chore chore = choreRepo.GetById(id);
                            Console.WriteLine($"{chore.Id} - {chore.Name}");
                            Console.Write("Press any key to continue");
                            Console.ReadKey();
                            break;
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    case ("Search for roommate"):
                        Console.Write("Roommate Id: ");
                        try
                        {
                            int id = int.Parse(Console.ReadLine());

                            Roommate roomie = roommateRepo.GetById(id);
                            Console.WriteLine($"{roomie.Id} - {roomie.FirstName} lives in the {roomie.Room.Name} and pays {roomie.RentPortion}% of the rent.");
                            Console.Write("Press any key to continue");
                            Console.ReadKey();
                            break;
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    case ("Add a room"):
                        Console.Write("Room name: ");
                        string name = Console.ReadLine();

                        Console.Write("Max occupancy: ");
                        int max = int.Parse(Console.ReadLine());

                        Room roomToAdd = new Room()
                        {
                            Name = name,
                            MaxOccupancy = max
                        };

                        roomRepo.Insert(roomToAdd);

                        Console.WriteLine($"{roomToAdd.Name} has been added and assigned an Id of {roomToAdd.Id}");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Add a chore"):
                        Console.Write("Chore name: ");
                        name = Console.ReadLine();

                        Chore choreToAdd = new Chore()
                        {
                            Name = name,
                        };

                        choreRepo.Insert(choreToAdd);

                        Console.WriteLine($"{choreToAdd.Name} has been added and assigned an Id of {choreToAdd.Id}");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Update a room"):
                        List<Room> roomOptions = roomRepo.GetAll();
                        foreach (Room r in roomOptions)
                        {
                            Console.WriteLine($"{r.Id} - {r.Name} Max Occupancy({r.MaxOccupancy})");
                        }

                        Console.Write("Which room would you like to update? ");
                        int selectedRoomId = int.Parse(Console.ReadLine());
                        Room selectedRoom = roomOptions.FirstOrDefault(r => r.Id == selectedRoomId);

                        Console.Write("New Name: ");
                        selectedRoom.Name = Console.ReadLine();

                        Console.Write("New Max Occupancy: ");
                        selectedRoom.MaxOccupancy = int.Parse(Console.ReadLine());

                        roomRepo.Update(selectedRoom);

                        Console.WriteLine("Room has been successfully updated");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Update a chore"):
                        List<Chore> choreOptions = choreRepo.GetAll();
                        foreach (Chore c in choreOptions)
                        {
                            Console.WriteLine($"{c.Id} - {c.Name}");
                        }

                        Console.Write("Which chore would you like to update? ");
                        int selectedChoreId = int.Parse(Console.ReadLine());
                        Chore selectedChore = choreOptions.FirstOrDefault(c => c.Id == selectedChoreId);

                        Console.Write("New Name: ");
                        selectedChore.Name = Console.ReadLine();

                        choreRepo.Update(selectedChore);

                        Console.WriteLine("Chore has been successfully updated");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Delete a room"):
                        roomOptions = roomRepo.GetAll();
                        foreach (Room r in roomOptions)
                        {
                            Console.WriteLine($"{r.Id} - {r.Name} Max Occupancy({r.MaxOccupancy})");
                        }
                        Console.Write("Which room would you like to delete? ");
                        selectedRoomId = int.Parse(Console.ReadLine());

                        roomRepo.Delete(selectedRoomId);

                        Console.WriteLine("Room has been successfully deleted");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Delete a chore"):
                        choreOptions = choreRepo.GetAll();
                        foreach (Chore c in choreOptions)
                        {
                            Console.WriteLine($"{c.Id} - {c.Name}");
                        }
                        Console.Write("Which chore would you like to delete? ");
                        selectedChoreId = int.Parse(Console.ReadLine());

                        try
                        {
                            choreRepo.Delete(selectedChoreId);
                            Console.WriteLine("Chore has been successfully deleted");
                        }
                        catch (Microsoft.Data.SqlClient.SqlException)
                        {
                            Console.WriteLine("The chore you are deleting is assigned to a roommate.  Please unassign it before deleting.");
                        }

                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Reassign a chore"):
                        choreOptions = choreRepo.GetAssignedChores();
                        foreach (Chore c in choreOptions)
                        {
                            Console.WriteLine($"{c.Id} - {c.Name}");
                        }
                        Console.Write("Which chore would you like to reassign? ");
                        selectedChoreId = int.Parse(Console.ReadLine());

                        var assignedRoomie = choreRepo.GetAssignmentsByRoommateChoreId(selectedChoreId);
                        Console.WriteLine($"This chore is currently assigned to {assignedRoomie.FirstName}.");
                        Console.WriteLine("Who would you like to assign it to?");
                        List<Roommate> allRoommates = roommateRepo.GetAll();
                        // Print list of roommates (except the assigned roommate
                        foreach (Roommate r in allRoommates)
                        {
                            if (r.FirstName != assignedRoomie.FirstName)
                            {
                                Console.WriteLine($"{r.Id}: {r.FirstName}");
                            }
                        }
                        Console.Write(">");
                        int selectedRoommateId = int.Parse(Console.ReadLine());
                        //collect input
                        // call the reassign method on choreRepo
                        choreRepo.ReassignChore(selectedChoreId, selectedRoommateId);

                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Exit"):
                        runProgram = false;
                        break;
                }
            }

        }

        static string GetMenuSelection()
        {
            Console.Clear();

            List<string> options = new List<string>()
            {
                "Show all rooms",
                "Show all chores",
                "Get chore counts",
                "Show all unassigned chores",
                "Assign chore",
                "Search for room",
                "Search for chore",
                "Search for roommate",
                "Add a room",
                "Add a chore",
                "Update a room",
                "Update a chore",
                "Delete a room",
                "Delete a chore",
                "Reassign a chore",
                "Exit"
            };

            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i]}");
            }

            while (true)
            {
                try
                {
                    Console.WriteLine();
                    Console.Write("Select an option > ");

                    string input = Console.ReadLine();
                    int index = int.Parse(input) - 1;
                    return options[index];
                }
                catch (Exception)
                {

                    continue;
                }
            }
        }
    }
}