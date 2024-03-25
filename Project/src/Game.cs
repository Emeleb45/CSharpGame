using System;

class Game
{
	public AudioPlayer audioPlayer = new AudioPlayer();
	private Parser parser;
	private Player player;
	public bool quitRequested = false;

	public Game()
	{
		parser = new Parser();
		player = new Player(this);
		CreateLocations();
	}


	private void CreateLocations()
	{
		// ITEMS -- Weight, Function, Type, Description, Count



		// Add your Items     \/Weight\/Func\/Type          \/Desc                           \/ammt always 1
		Item Sword = new Item(15, "50", "weapon", "A rusted egyptian sword."); //<-- amnt for putcmd could be more only rly for healing and stuff 
		Item Bandage = new Item(5, "20", "healingitem", "Stops bleeding and heals you by 20hp.");
		Item Lifekey = new Item(2, "keylife", "keyitem", "A key with the symbol of life.");
		// Create the rooms
		Location mainent = new Location("in the main entrance you can still see the hole you fell trough.");
		Location mainhall = new Location("in the main hallway that can lead you to most places, its so dark you cant see the end.");
		Location mummyworkshop = new Location("in a wide room full of mummies it seems they are made here.");

		// Initialise room exits
		mainent.AddExit("east", mainhall);
		mainent.AddExit("north", mainhall);
		mainent.AddExit("west", mainhall);

		mainhall.AddExit("south", mainent);
		mainhall.AddExit("southwest", mummyworkshop);

		mummyworkshop.AddExit("east", mainhall);


		mainent.Chest.Put("sword", Sword);
		mummyworkshop.Chest.Put("bandage", Bandage);
		mummyworkshop.Chest.Put("lifekey", Lifekey);


		// Add Enemies 
		mummyworkshop.AddEnemy("sislo", new Enemy(0, "Snake"));
		mummyworkshop.AddEnemy("poingo", new Enemy(0, "Snake"));

		// Start game Location
		player.CurrentLocation = mainent;
	}

	public void Play()
	{
		Console.ForegroundColor = ConsoleColor.DarkYellow;
		PrintWelcome();
		Thread BackMusic = audioPlayer.PlayAudioAsync("assets/audio/BackMusic.wav", true);
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
			audioPlayer.StopAllAudioThreads();
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
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("You died");
			Console.WriteLine("Press [Enter] to quit.");

		}
		Console.ReadLine();
		Environment.Exit(0);
	}

	private void PrintWelcome()
	{
		Console.WriteLine("Your eyes are getting used to the dark now.");
		Console.WriteLine("You fell into an ancient temple and are now bleeding.");
		Console.WriteLine($"Your only objective:\nEscape");
		Console.WriteLine("Type 'help' for a controls.");
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
			audioPlayer.StopAllAudioThreads();
			Thread BackMusic = audioPlayer.PlayAudioAsync("assets/audio/BattleMain.wav", true);
			player.InCombat = true;
			Console.ForegroundColor = ConsoleColor.Red;
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
