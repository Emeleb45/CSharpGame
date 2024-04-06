using System;
using NAudio.Wave;

class Game
{
	private Parser parser;
	private Player player;

	public bool quitRequested = false;
	public AudioManager audioManager = new AudioManager();
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
		Item LifeKey = new Item(2, "lifekey", "keyitem", "A key with the symbol of life.");
		Item GoldHelmet = new Item(8, "8", "headgear", "HATGEAR");
		Item GoldChestplate = new Item(8, "8", "chestgear", "CHESTGEAR");
		Item GoldLeggings = new Item(8, "8", "leggear", "LEGGEAR");
		Item GoldBoots = new Item(8, "8", "footgear", "FootGEAR");
		// Create the rooms
		Location mainent = new Location("in the main entrance you can still see the hole you fell trough.");
		Location mainhall = new Location("in the main hallway that can lead you to most places, its so dark you cant see the end.");
		Location mummyworkshop = new Location("in a wide room full of mummies it seems they are made here.");
		Location newlocation = new Location("WOW YOU GOT THEY KEY WHAT OMG.");

		// Initialise room exits
		mainent.AddExit("north", mainhall);


		mainhall.AddExit("south", mainent);
		mainhall.AddExit("southwest", mummyworkshop);
		mainhall.AddLockedExit("north", newlocation, LifeKey);

		mummyworkshop.AddExit("east", mainhall);

		newlocation.AddExit("south", mainhall);

		mainent.Chest.Put("sword", Sword);
		mummyworkshop.Chest.Put("bandage", Bandage);
		mummyworkshop.Chest.Put("lifekey", LifeKey);
		mummyworkshop.Chest.Put("goldhelmet", GoldHelmet);
		mummyworkshop.Chest.Put("goldchestplate", GoldChestplate);
		mummyworkshop.Chest.Put("goldpants", GoldLeggings);
		mummyworkshop.Chest.Put("goldboots", GoldBoots);


		// Add Enemies 
		mummyworkshop.AddEnemy("sislo", new Enemy(0, "Snake"));
		mummyworkshop.AddEnemy("poingo", new Enemy(0, "Snake"));

		newlocation.AddEnemy("gonga", new Enemy(2, "Snake"));
		newlocation.AddEnemy("donga", new Enemy(2, "Snake"));
		// Start game Location
		player.CurrentLocation = mainent;
		player.PreviousLocation = mainent;
	}

	public void Play()
	{
		Console.ForegroundColor = ConsoleColor.DarkYellow;
		PrintWelcome();
		audioManager.PlayBackgroundMusic("assets/audio/BackMusic.wav");
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
		Console.WriteLine("Everytime you move you shall lose 5HP.");
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
			case "run":
				Run();
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


	private void Run()
	{
		if (player.InCombat == true && player.CanRun == true)
		{
			Location escapingroom = player.CurrentLocation;
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			player.InCombat = false;
			audioManager.PlayBackgroundMusic("assets/audio/BackMusic.wav");
			player.CurrentLocation = player.PreviousLocation;
			Console.WriteLine("Succesfully ran away but your enemies got healed.");
			foreach (Enemy enemy in escapingroom.enemies.Values)
			{
				enemy.health = 100;
			}
			if (player.bleeding == true)
			{
				player.Damage(5);
			}
		}
		else
		{
			Console.WriteLine("There is nowhere to run...");
		}
	}
	private void GoLocation(Command command)
	{
		if (player.InCombat == true)
		{
			Console.WriteLine("Use command run if you want to!");
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
		player.PreviousLocation = player.CurrentLocation;
		player.CurrentLocation = nextLocation;

		if (player.bleeding == true)
		{
			player.Damage(5);
		}
		if (nextLocation.enemies != null && nextLocation.enemies.Count > 0)
		{
			audioManager.PlayBackgroundMusic("assets/audio/BattleMain.wav");
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
		Console.WriteLine("Armor: " + player.Armor);



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
