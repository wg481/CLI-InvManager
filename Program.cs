using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Security.Cryptography.X509Certificates;

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
			Console.WriteLine("3. Credits");
			Console.WriteLine("x. Exit");
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

				case "3":
					Credits();
					break;
				
				case "x":
					Environment.Exit(0);
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
				Console.WriteLine("x. Save & Exit");
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

					case "x":
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
				ClearAndPrintHeader($"Player: {player.Name}   Gold: {player.Gold}");
				Console.WriteLine($"Level: {player.Level}   EXP: {player.Experience}");
				Console.WriteLine("");
				Console.WriteLine("1. Inventory Manager");
				Console.WriteLine("2. Edit Gold");
				Console.WriteLine("3. Experience and Editor");
				Console.WriteLine("x. Exit");
				Console.Write("\nChoice: ");

				switch (Console.ReadLine())
				{
					case "1":
						ShowInventory(player);
						break;
					case "2":
						EditGoldCLI(player);
						break;
					case "3":
						ExperienceEditor(player);
						break;
					case "x":
						running = false;
						break;
				}
			}
		}

// CLI Interfacing - Edits properties of Player

		static void ShowInventory(Player player)
		{
			bool runningInvManager = true;
			while (runningInvManager)
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

				Console.WriteLine("");
				Console.WriteLine("Inventory Options:");
				Console.WriteLine("1. Add Item");
				Console.WriteLine("2. Remove Item");
				Console.WriteLine("3. Edit existing Item");
				Console.WriteLine("x. Exit");
				Console.Write("\n Select an option: ");
				string input = Console.ReadLine();

				switch (input)
				{
					case "1":
						AddItemCLI(player);
						break;
					case "2":
						RemoveItemCLI(player);
						break;
					case "3":
						EditItemCLI(player);
						break;
					case "x":
						runningInvManager = false;
						break;
				}
			}
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
		static void EditGoldCLI(Player player)
		{
			ClearAndPrintHeader($"Editing Gold for {player}");
			Console.WriteLine("1. Add Gold");
			Console.WriteLine("2. Remove Gold");
			Console.WriteLine("3. Exit");
			Console.Write("Choose an option: ");
			string input = Console.ReadLine();

			switch (input)
			{
				case "1":
					Console.WriteLine("Add how much gold: ");
					player.Gold += Convert.ToInt32(Console.ReadLine());
					break;
				case "2":
					Console.WriteLine("Remove how much gold: ");
					player.Gold -= Convert.ToInt32(Console.ReadLine());
					break;
				case "3":
					break;
				case "":
					break;
				case null:
					break;
			}
		}

		static void ExperienceEditor(Player player)
		{
			ClearAndPrintHeader($"Editing Experience for: {player.Name}");
			Console.WriteLine($"Current Level: {player.Level}");
			Console.WriteLine($"Current Experience: {player.Experience}");
			Console.WriteLine("1. Update Experience");
			Console.WriteLine("2. Update Level");
			Console.WriteLine("x. Return to Player Menu");
			Console.Write("\nSelect an option: ");
			string input = Console.ReadLine();

			switch (input)
			{
				case "1":
					ClearAndPrintHeader($"Current Experience: {player.Experience}");
					Console.WriteLine("1. Add Experience");
					Console.WriteLine("2. Remove Experience");
					Console.WriteLine("x. Exit");
					Console.Write("\nSelect an option: ");
					string input2 = Console.ReadLine();

					switch (input2)
					{
						case "1":
							Console.Write("\nAdd how much experience: ");
							player.Experience += Convert.ToInt32(Console.ReadLine());
							break;
						case "2":
							Console.Write("\nRemove how much experience: ");
							player.Experience -= Convert.ToInt32(Console.ReadLine());
							break;
						case "x":
							break;
						default:
							break;
					}

					break;
				case "2":
					ClearAndPrintHeader($"Current Level: {player.Level}");
					Console.WriteLine("1. Level Up");
					Console.WriteLine("2. Level Down");
					Console.WriteLine("3. Manual Level Set");
					Console.WriteLine("x. Exit");
					Console.Write("\nSelect an option: ");
					string input3 = Console.ReadLine();

					switch (input3)
					{
						case "1":
							player.Level++;
							break;
						case "2":
							player.Level--;
							break;
						case "3":
							Console.Write("\nSet level to: ");
							player.Level = Convert.ToInt32(Console.ReadLine());
							break;
						case "x":
							break;
						default:
							break;
					}
					break;
				case "x":
					break;
				default:
					break;
			}

		}


// SAVE AND LOAD - Dumps all to JSON.
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

		static void Credits()
		{
			ClearAndPrintHeader("Credits");
			Console.WriteLine("Written in C# by wg481.");
			Console.WriteLine("");
			Console.WriteLine("If you paid for this software, request a refund.");
			Console.WriteLine("");
			var lines = File.ReadLines("LICENSE");
			foreach (var line in lines)
			{
				Console.WriteLine(line);
			}
			Console.WriteLine();
			Console.WriteLine("Upon pressing a key, the software will close.");
			WaitForKey();
			Environment.Exit(0);
		}

	}
}
 