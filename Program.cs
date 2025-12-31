using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CLI_TTRPGInventoryManager
{
	class Program
	{
		static void Main(string[] args)
		{
			Party party = ShowMainMenu();

			RunPartyLoop(party);
			SaveParty(party);
		}

		static void ClearAndPrintHeader(string title)
		{
			Console.Clear();
			Console.WriteLine($"=== {title} ===\n");
		}

		static void WaitForKey(string message = "Press any key to continue...")
		{
			Console.WriteLine($"\n{message}");
			Console.ReadKey();
		}

		static Party ShowMainMenu()
		{
			ClearAndPrintHeader("TTRPG Inventory Manager");

			Console.WriteLine("1. New Party");
			Console.WriteLine("2. Load a Party");
			Console.Write("\nChoice: ");
			string choice = Console.ReadLine();

			Party party = new Party("null");

			switch (choice)
			{
				case "1":
					Console.Write("Name your Party: ");
					string name = Console.ReadLine();
					party = new Party(name);
					break;

				case "2":
					Console.Write("Party name: ");
					string partyname = Console.ReadLine();
					party = LoadParty(partyname);
					break;

				default:
					Console.WriteLine("Invalid option.");
					WaitForKey();
					break;
			}

			return party;
		}

		static void RunPartyLoop(Party party)
		{
			bool running = true;

			while (running)
			{
				ClearAndPrintHeader($"Party: {party.Name}");

				Console.WriteLine("1. Select Player");
				Console.WriteLine("2. Add Player");
				Console.WriteLine("3. Remove Player");
				Console.WriteLine("4. Save & Exit");
				Console.Write("\nChoice: ");

				switch (Console.ReadLine())
				{
					case "1":
						Player selected = SelectPlayer(party);
						if (selected != null)
							RunPlayerLoop(selected);
						break;

					case "2":
						Console.Write("Player name: ");
						party.Players.Add(new Player(Console.ReadLine()));
						break;

					case "3":
						Console.Write("Player name to remove: ");
						string nameToRemove = Console.ReadLine();
						if (party.RemovePlayer(nameToRemove))
							Console.WriteLine("Player removed.");
						else
							Console.WriteLine("Player not found.");
						WaitForKey();
						break;

					case "4":
						running = false;
						break;
				}
			}
		}

		static Player SelectPlayer(Party party)
		{
			if (party.Players.Count == 0)
			{
				Console.WriteLine("The party is empty!");
				WaitForKey();
				return null;
			}

			ClearAndPrintHeader($"Select Player in {party.Name}");
			for (int i = 0; i < party.Players.Count; i++)
				Console.WriteLine($"{i}. {party.Players[i].Name}");

			Console.Write("\nSelect player: ");
			if (!int.TryParse(Console.ReadLine(), out int index))
				return null;

			if (index < 0 || index >= party.Players.Count)
				return null;

			return party.Players[index];
		}

		static void RunPlayerLoop(Player player)
		{
			bool running = true;

			while (running)
			{
				ClearAndPrintHeader($"Player: {player.Name}");
				Console.WriteLine("1) View Inventory");
				Console.WriteLine("2) Add Item");
				Console.WriteLine("3) Edit Item");
				Console.WriteLine("4) Remove Item");
				Console.WriteLine("5) Back");
				Console.Write("\nChoice: ");

				switch (Console.ReadLine())
				{
					case "1":
						ShowInventory(player);
						break;

					case "2":
						AddItemCLI(player);
						break;

					case "3":
						EditItemCLI(player);
						break;

					case "4":
						RemoveItemCLI(player);
						break;

					case "5":
						running = false;
						break;
				}
			}
		}

		static void ShowInventory(Player player)
		{
			ClearAndPrintHeader($"Inventory for {player.Name}");

			if (player.Inventory.Count == 0)
			{
				Console.WriteLine("Inventory is empty.");
			}
			else
			{
				for (int i = 0; i < player.Inventory.Count; i++)
				{
					Item item = player.Inventory[i];
					Console.WriteLine($"{i}) {item.Name} x{item.Quantity}");
					Console.WriteLine($"   {item.Description}");
				}
			}

			WaitForKey();
		}

		static void AddItemCLI(Player player)
		{
			ClearAndPrintHeader($"Adding item to {player.Name}'s inventory");

			Console.Write("Item name: ");
			string name = Console.ReadLine();

			Console.Write("Quantity: ");
			int quantity = int.Parse(Console.ReadLine());

			Console.Write("Description: ");
			string description = Console.ReadLine();

			player.AddItem(new Item(name, quantity, description));
		}

		static void EditItemCLI(Player player)
		{
			ClearAndPrintHeader($"Editing items for {player.Name}");
			ShowInventory(player);

			Console.Write("Select item index: ");
			if (!int.TryParse(Console.ReadLine(), out int index) || index < 0 || index >= player.Inventory.Count)
				return;

			Item item = player.Inventory[index];

			Console.WriteLine("1) Change quantity");
			Console.WriteLine("2) Change description");
			Console.Write("\nChoice: ");

			switch (Console.ReadLine())
			{
				case "1":
					Console.Write("New quantity: ");
					item.Quantity = int.Parse(Console.ReadLine());
					break;

				case "2":
					Console.Write("New description: ");
					item.Description = Console.ReadLine();
					break;
			}
		}

		static void RemoveItemCLI(Player player)
		{
			ClearAndPrintHeader($"Removing items from {player.Name}'s inventory");
			ShowInventory(player);

			Console.Write("Select item index to remove: ");
			if (!int.TryParse(Console.ReadLine(), out int index) || index < 0 || index >= player.Inventory.Count)
				return;

			Item item = player.Inventory[index];

			Console.Write($"Enter quantity to remove (max {item.Quantity}, leave blank to remove all): ");
			string input = Console.ReadLine();

			if (string.IsNullOrWhiteSpace(input))
			{
				player.Inventory.RemoveAt(index);
				Console.WriteLine($"{item.Name} removed completely from inventory.");
			}
			else
			{
				if (int.TryParse(input, out int qty))
				{
					if (qty >= item.Quantity)
					{
						player.Inventory.RemoveAt(index);
						Console.WriteLine($"{item.Name} removed completely from inventory.");
					}
					else if (qty > 0)
					{
						item.Quantity -= qty;
						Console.WriteLine($"{qty} x {item.Name} removed. Remaining: {item.Quantity}");
					}
					else
					{
						Console.WriteLine("Quantity must be positive.");
					}
				}
				else
				{
					Console.WriteLine("Invalid quantity.");
				}
			}

			WaitForKey();
		}

		static void SaveParty(Party party)
		{
			string json = JsonSerializer.Serialize(
				party,
				new JsonSerializerOptions { WriteIndented = true }
			);

			File.WriteAllText($"{party.Name}.json", json);
		}

		static Party LoadParty(string filename)
		{
			string json = File.ReadAllText($"{filename}.json");
			return JsonSerializer.Deserialize<Party>(json);
		}
	}
}
 
