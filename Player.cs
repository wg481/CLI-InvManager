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
        public List<Item> Inventory { get; set; }
        public int Gold { get; set; }
        public int Experience { get; set; }
        public double Weight { get; set; }
        public int Level { get; set; }


        public Player()
        {
            Gold = 0;
            Inventory = new List<Item>();
            Experience = 0;
            Weight = 0.00;
            Level = 1;
        }

        public Player(string name)
        {
            Name = name;
            Gold = 0;
            Inventory = new List<Item>();
            Experience = 0;
            Weight = 0.00;
            Level = 1;
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

        public void EditGold(int gold)
        {
            Gold += gold;
        }
    }
}