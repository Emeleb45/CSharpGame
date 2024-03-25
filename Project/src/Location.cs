using System.Collections.Generic;

class Location
{

	private string description;
	internal Dictionary<string, Enemy> enemies;
	private Inventory chest;
	public Inventory Chest
	{
		get { return chest; }
	}
	public Location()
	{
		chest = new Inventory(999999);

	}
	internal Dictionary<string, Location> exits;
	internal Dictionary<string, (Location, Item)> lockedExits;

	public Location(string desc)
	{
		chest = new Inventory(999999);
		description = desc;
		exits = new Dictionary<string, Location>();
		enemies = new Dictionary<string, Enemy>();
		lockedExits = new Dictionary<string, (Location, Item)>();
	}
	public void AddEnemy(string enemyname, Enemy enemy)
	{
		enemy.Name = enemyname;
		enemies.Add(enemyname, enemy);
	}

	public void AddExit(string direction, Location neighbor)
	{
		exits.Add(direction, neighbor);
	}

	public void AddLockedExit(string direction, Location neighbor, Item requiredKey)
	{
		lockedExits.Add(direction, (neighbor, requiredKey));
	}
	public string GetShortDescription()
	{
		return description;
	}

	public string GetLongDescription()
	{
		string str = "You are ";
		str += description;
		str += ".\n";

		str += GetExitString();
		str += "\n";
		str += GetLockedExitString();
		return str;
	}


	public Location GetExit(string direction)
	{
		if (exits.ContainsKey(direction))
		{
			return exits[direction];
		}
		return null;
	}
	public (Location, Item) GetLockedExit(string direction)
	{
		if (lockedExits.ContainsKey(direction))
		{
			return lockedExits[direction];
		}
		return (null, null);
	}

	private string GetExitString()
	{
		string str = "Exits:";


		int countCommas = 0;
		foreach (string key in exits.Keys)
		{
			if (countCommas != 0)
			{
				str += ",";
			}
			str += " " + key;
			countCommas++;
		}

		return str;
	}
	private string GetLockedExitString()
	{
		string str = "Locked Exits:";

		foreach (string key in lockedExits.Keys)
		{
			string requiredItemCode = lockedExits[key].Item2.Func; // Assuming Item has an ItemCode property
			str += $" {key} (Requires {requiredItemCode})";
		}

		return str + "\n";
	}
}
