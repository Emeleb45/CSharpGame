using System;

class Game
{
	private Parser parser;
	private Player player;



	public Game()
	{
		parser = new Parser();
		player = new Player();
		CreateLocations();
	}


	private void CreateLocations()
	{
		// ITEMS
		Item sword = new Item(15, "An old heavy sword rusted and cracked.");


		// Create the rooms
		Location beach = new Location("on the beach you woke up on");
		Location wreck = new Location("in a big shipwreck on the beach, it looks old");
		Location wreckdeck = new Location("on the topdeck of the wreck");

		// Initialise room exits
		beach.AddExit("east", wreck);

		wreck.AddExit("west", beach);
		wreck.AddExit("up", wreckdeck);

		wreckdeck.AddExit("down", wreck);


		// Add your Items here

		wreckdeck.Chest.Put("sword", sword);


		// Start game Location
		player.CurrentLocation = beach;
	}

	public void Play()
	{
		PrintWelcome();


		bool finished = false;
		while (!finished)
		{
			if (player.health > 0)
			{
				Command command = parser.GetCommand();
				finished = ProcessCommand(command);
			}
			else
			{
				finished = true;
			}

		}
		if (player.health > 0)
		{
			Console.WriteLine("Thank you for playing.");
			Console.WriteLine("Press [Enter] to continue.");
			Console.ReadLine();
		}
		else
		{
			Console.WriteLine("You died");
			Console.WriteLine("Press [Enter] to quit.");
			Console.ReadLine();
		}

	}

	private void PrintWelcome()
	{
		Console.WriteLine();
		Console.WriteLine("Wake up");
		Console.WriteLine("Where are you");
		Console.WriteLine("Type 'help' if you need help.");
		Console.WriteLine();
		Console.WriteLine(player.CurrentLocation.GetLongDescription());
	}


	private bool ProcessCommand(Command command)
	{
		bool wantToQuit = false;

		if (command.IsUnknown())
		{
			Console.WriteLine("Unknown Command");
			return wantToQuit;
		}
		Console.Clear();

		switch (command.CommandWord)
		{
			case "help":
				PrintHelp();
				break;
			case "go":
				GoLocation(command);
				break;
			case "look":
				Look();
				break;
			case "status":
				Status();
				break;
			case "take":
				Take(command);
				break;
			case "drop":
				Drop(command);
				break;
			case "quit":
				wantToQuit = true;
				break;
			case "die":
				KillPlayer();
				break;

		}

		return wantToQuit;
	}

	// ######################################
	// implementations of user commands:
	// ######################################
	private void PrintHelp()
	{
		Console.WriteLine("You are lost. You are alone.");
		Console.WriteLine("You wander around on some island.");
		Console.WriteLine();
		// let the parser print the commands
		parser.PrintValidCommands();
	}


	private void GoLocation(Command command)
	{
		if (!command.HasSecondWord())
		{

			Console.WriteLine("Go where?");
			return;
		}

		string direction = command.SecondWord;

		Location nextRoom = player.CurrentLocation.GetExit(direction);
		if (nextRoom == null)
		{
			Console.WriteLine("There is no exit " + direction + "!");
			return;
		}

		player.CurrentLocation = nextRoom;
		player.Damage(1);
		Console.WriteLine(player.CurrentLocation.GetLongDescription());
	}
	private void Look()
	{
		Console.WriteLine(player.CurrentLocation.GetLongDescription());
		string line = player.CurrentLocation.Chest.getallitems();
		if (line == null)
		{
			Console.WriteLine("Location is empty.");
		}
		else
		{
			Console.WriteLine("This location holds: \n" + line);
		}


	}
	public void Status()
	{
		string line = player.ShowBackpack();
		if (line == null)
		{
			Console.WriteLine("Backpack is empty.");
		}
		else
		{
			Console.WriteLine("Your items: \n" + line);
		}

		Console.WriteLine("Health: " + player.health);


	}
	public void Take(Command command)
	{
		if (!command.HasSecondWord())
		{

			Console.WriteLine("Take What");
			return;
		}
		string TakingItem = command.SecondWord;
		player.TakeFromChest(TakingItem);

	}
	public void Drop(Command command)
	{

		if (!command.HasSecondWord())
		{

			Console.WriteLine("Drop What");
			return;
		}
		string TakingItem = command.SecondWord;
		player.DropToChest(TakingItem);
	}
	public void KillPlayer() {
		player.health = 0;
	}
}
