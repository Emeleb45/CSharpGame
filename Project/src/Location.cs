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


	public Location(string desc)
	{
		chest = new Inventory(999999);
		description = desc;
		exits = new Dictionary<string, Location>();
		enemies = new Dictionary<string, Enemy>();

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
}
