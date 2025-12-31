using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Text.Json;

namespace CLI_TTRPGInventoryManager
{
    public class Item
    {
	    public string Name { get; set; }
	    public int Quantity { get; set; }
	    public string Description { get; set; }

	    public Item(string name, int quantity, string description)
	    {
		    Name = name;
		    Quantity = quantity;
		    Description = description;
	    }
        
        public Item() { }
    }

}