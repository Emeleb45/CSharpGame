using System;

class Game
{
	public AudioPlayer audioPlayer = new AudioPlayer();
	private Parser parser;
	private Player player;
	public bool quitRequested = false;
	private Thread BackAudio;

	public Game()
	{
		parser = new Parser();
		player = new Player(this);
		CreateLocations();
	}


	private void CreateLocations()
	{
		// ITEMS -- Weight, Function, Type, Description, Count


		// ENEMIES -- Armor, Type
		Enemy Crab = new Enemy(0, "Crab");


		// Create the rooms
		Location beach = new Location("on the beach you woke up on");
		Location wreck = new Location("in a big shipwreck on the beach, it looks old");
		Location wreckdeck = new Location("on the topdeck of the wreck");

		// Initialise room exits
		beach.AddExit("east", wreck);

		wreck.AddExit("west", beach);
		wreck.AddExit("up", wreckdeck);
		wreckdeck.AddExit("down", wreck);


		// Add your Items \/ Name        \/Armor\/Func\/Type          \/Desc                           \/ammt always 1
		wreck.Chest.Put("sword", new Item(15, "25", "weapon", "An old heavy sword rusted and cracked.")); //<-- amnt for putcmd could be more only rly for healing and stuff 
		wreck.Chest.Put("bandage", new Item(5, "20", "healingitem", "Stops bleeding and heals you by 20hp."));
		wreck.Chest.Put("bandage", new Item(5, "20", "healingitem", "Stops bleeding and heals you by 20hp."));
		wreck.Chest.Put("bandage", new Item(5, "20", "healingitem", "Stops bleeding and heals you by 20hp."));


		// Add Enemies       \/ Name         \/Armor \/ Type
		wreckdeck.AddEnemy("cabby", new Enemy(0, "Crab"));
		wreckdeck.AddEnemy("crabbo", new Enemy(0, "Crab"));

		// Start game Location
		player.CurrentLocation = beach;
	}

	public void Play()
	{
		PrintWelcome();
		Thread audioThread = audioPlayer.PlayAudioAsync("assets/audio/BackMusic.mp3", true);
		Thread gameThread = new Thread(() =>
		{
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

			quitRequested = true;
			audioPlayer.StopAudioThread(audioThread);
			audioPlayer.WaitForAllAudioThreads();
		});
		gameThread.Start();
		gameThread.Join();
		if (player.health > 0)
		{
			Console.WriteLine("Thank you for playing.");
			Console.WriteLine("Press [Enter] to continue.");

		}
		else
		{
			Console.WriteLine("You died");
			Console.WriteLine("Press [Enter] to quit.");

		}
		Console.ReadLine();
		Environment.Exit(0);
	}

	private void PrintWelcome()
	{
		Console.WriteLine();
		Console.WriteLine("Wake up");
		Console.WriteLine("Where are you");
		Console.WriteLine("Type 'help' if you need help.");
		Console.WriteLine();
		Look();
		Status();
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
			case "use":
				UseItem(command);
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
		if (player.InCombat == true)
		{
			Console.WriteLine("You cant run.");
			return;
		}
		if (!command.HasSecondWord())
		{

			Console.WriteLine("Go where?");
			return;
		}

		string direction = command.SecondWord;

		Location nextLocation = player.CurrentLocation.GetExit(direction);
		if (nextLocation == null)
		{
			Console.WriteLine("There is no exit " + direction + "!");
			return;
		}
		player.CurrentLocation = nextLocation;

		if (player.bleeding == true)
		{
			player.Damage(5);
		}
		if (nextLocation.enemies != null && nextLocation.enemies.Count > 0)
		{
			player.InCombat = true;
			Console.WriteLine($"Oh no! You have encountered enemies.");

			foreach (var enemyEntry in nextLocation.enemies)
			{
				string enemyName = enemyEntry.Key;
				Enemy enemy = enemyEntry.Value;

				Console.WriteLine($"{enemyName}, the {enemy.EniType}.");
			}


			return;
		}

		Look();
	}
	private void Look()
	{
		if (player.InCombat == true)
		{
			Console.WriteLine($"Enemies:");

			foreach (var enemyEntry in player.CurrentLocation.enemies)
			{
				string enemyName = enemyEntry.Key;
				Enemy enemy = enemyEntry.Value;

				Console.WriteLine($"{enemyName}, the {enemy.EniType}. || Health: {enemy.health}");
			}

		}
		else
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

		if (player.bleeding)
		{
			Console.WriteLine("You are bleeding.");
		}
		else
		{
			Console.WriteLine("Not bleeding.");
		}
		Console.WriteLine("Health: " + player.health);



	}
	public void Take(Command command)
	{
		if (player.InCombat == true)
		{
			Console.WriteLine("Focus on the fight.");
			return;
		}
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
		if (player.InCombat == true)
		{
			Console.WriteLine("Focus on the fight.");
			return;
		}
		if (!command.HasSecondWord())
		{

			Console.WriteLine("Drop What?");
			return;
		}
		string TakingItem = command.SecondWord;
		player.DropToChest(TakingItem);
	}
	public void KillPlayer()
	{
		player.health = 0;
	}
	public void UseItem(Command command)
	{

		if (!command.HasSecondWord())
		{
			Console.WriteLine("Use what how?");
			return;
		}
		string UsingItem = command.SecondWord;
		string InteractedPart = "";
		if (command.HasThirdWord())
		{
			InteractedPart = command.ThirdWord;
		}
		else
		{
			InteractedPart = "player";
		}

		player.UseItem(UsingItem, InteractedPart);
	}
}
