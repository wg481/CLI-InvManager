using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace CLI_TTRPGInventoryManager
{
    public class Party
    {
        public string Name { get; set; }
        public List<Player> Players { get; set; }

        public Party(string name)
        {
            Name = name;
            Players = new List<Player>();
        }

        // Remove a player by name
        public bool RemovePlayer(string playerName)
        {
            var player = Players.FirstOrDefault(p => p.Name == playerName);
            if (player == null)
                return false;

            Players.Remove(player);
            return true;
        }
    }
}
