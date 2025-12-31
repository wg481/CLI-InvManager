using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace CLI_TTRPGInventoryManager
{
    public class Player
    {
        public string Name { get; set; }
        public List<Item> Inventory {get; set; }

        public Player()
        {
            Inventory = new List<Item>();
        }

        public Player(string name)
        {
            Name = name;
            Inventory = new List<Item>();
        }

        public void AddItem(Item item)
        {
            var existing = Inventory.FirstOrDefault(i => i.Name == item.Name);

            if (existing != null)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                Inventory.Add(item);
            }
        }
    }
}