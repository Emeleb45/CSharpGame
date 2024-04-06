class Player
{
    private Game game;

    public Location CurrentLocation { get; set; }
    public Location PreviousLocation { get; set; }
    public int health;
    private Inventory backpack;

    public string Name;
    public int Armor;
    public bool bleeding;
    public bool InCombat;
    public bool CanRun;


    // methods
    public void Hit(int amount)
    {
        int damage = amount;
        if (Armor > 0)
        {
            damage -= Armor;

            if (damage < 2)
            {
                damage = 2;
            }
        }
        health -= damage;

    }
    public void Damage(int amount)
    {
        health -= amount;
        if (health < 0)
        {
            health = 0;
        }
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > 100)
        {
            health = 100;
        }
    }

    public bool IsAlive()
    {
        return health > 0;
    }


    public Player(Game game)
    {
        Name = "Blank";
        Armor = 0;
        backpack = new Inventory(75);
        health = 75;
        bleeding = true;
        CurrentLocation = null;
        PreviousLocation = null;
        InCombat = false;
        CanRun = true;
        this.game = game;
    }
    public void UpdateArmor()
    {
        int totalArmor = 0;

        foreach (var item in backpack.items.Values)
        {

            if (item.Type == "headgear" || item.Type == "chestgear" || item.Type == "leggear" || item.Type == "footgear")
            {
                if (int.TryParse(item.Func, out int funcValue))
                {
                    totalArmor += funcValue;
                }
                else
                {
                    Console.WriteLine($"Invalid func value for item {item.Type}: {item.Func}");
                }
            }
        }


        Armor = totalArmor;
    }

    public string ShowBackpack()
    {
        int FreeWeight = backpack.FreeWeight();

        Console.WriteLine($"Available weight: {FreeWeight}");


        string allitems = backpack.getallitems();

        if (allitems != null)
        {

            return allitems;
        }
        else
        {
            return null;
        }


    }

    public bool TakeFromChest(string itemName)
    {
        if (CurrentLocation != null)
        {
            Item item = CurrentLocation.Chest.Get(itemName);
            if (item != null)
            {
                if (item.Type == "headgear" || item.Type == "chestgear" || item.Type == "leggear" || item.Type == "footgear")
                {
                    if (backpack.items.Values.Any(backpackItem => backpackItem.Type == item.Type))
                    {
                        Console.WriteLine("You already have that type of armor.");
                    }
                    else
                    {
                        backpack.Put(itemName, item);
                        CurrentLocation.Chest.del(itemName);
                        UpdateArmor();
                        game.audioManager.PlayEffect("assets/audio/TakeItem.wav");
                        Console.WriteLine(itemName + " added to inventory");
                    }
                }
                else
                {
                    backpack.Put(itemName, item);
                    CurrentLocation.Chest.del(itemName);
                    game.audioManager.PlayEffect("assets/audio/TakeItem.wav");
                    Console.WriteLine(itemName + " added to inventory");
                }
            }
        }
        return false;
    }
    public bool DropToChest(string itemName)
    {
        if (CurrentLocation != null)

        {
            Item item = backpack.Get(itemName);
            if (item != null)
            {
                CurrentLocation.Chest.Put(itemName, item);
                backpack.Get(itemName);
                backpack.del(itemName);
                Console.WriteLine("Dropped " + itemName);
                game.audioManager.PlayEffect("assets/audio/DropItem.wav");
                UpdateArmor();
            }

        }

        return false;
    }
    public bool UseItem(string itemName, string InteractedPart)
    {
        Item item = backpack.Get(itemName);
        if (item == null)
        {
            Console.WriteLine("You dont have that item.");
            return false;
        }
        switch (item.Type)
        {
            case "weapon":
                if (InteractedPart == "player")
                {
                    Console.WriteLine("Use on what?");
                    break;
                }
                int DamageValue;
                if (int.TryParse(item.Func, out DamageValue))
                {
                    if (CurrentLocation.enemies.ContainsKey(InteractedPart))
                    {

                        Enemy enemy = CurrentLocation.enemies[InteractedPart];
                        if (enemy.IsAlive())
                        {
                            Random random = new Random();
                            double randomValue = random.NextDouble();
                            EnemiesAttack();
                            if (randomValue <= 0.15) // <-- 15% change
                            {
                                bleeding = true;

                                Console.WriteLine("You have started to bleed.");
                            }
                            enemy.Damage(DamageValue);
                            game.audioManager.PlayEffect("assets/audio/PlayerAttack.wav");
                            Console.WriteLine($"Attacked {InteractedPart}.");
                            if (AreAllEnemiesDead())
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                InCombat = false;
                                game.audioManager.PlayBackgroundMusic("assets/audio/BackMusic.wav");

                                CurrentLocation.enemies.Clear();

                            }
                        }
                        else
                        {
                            Console.WriteLine($"{InteractedPart} is already dead.");
                            if (AreAllEnemiesDead())
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                game.audioManager.PlayBackgroundMusic("assets/audio/BackMusic.wav");
                                CurrentLocation.enemies.Clear();

                                InCombat = false;
                            }



                        }
                    }
                    else
                    {
                        Console.WriteLine($"Cannot attack {InteractedPart}.");
                    }
                }


                break;

            case "healingitem":
                int HealValue;
                if (int.TryParse(item.Func, out HealValue))
                    if (InteractedPart != "player")
                    {
                        Console.WriteLine("You can only heal yourself.");
                        break;
                    }
                    else
                    {
                        Heal(HealValue);
                        bleeding = false;
                        backpack.del(itemName);
                        Console.WriteLine($"Healed {HealValue}Hp.");
                    }

                break;

            case "keyitem":
                if (InteractedPart == "player")
                {
                    Console.WriteLine("Use on what?");
                    break;
                }
                if (!CurrentLocation.lockedExits.ContainsKey(InteractedPart) && !CurrentLocation.exits.ContainsKey(InteractedPart))
                {
                    Console.WriteLine("There is nothing there!");
                }
                if (CurrentLocation.exits.ContainsKey(InteractedPart))
                {
                    Console.WriteLine("Thats already unlocked!");
                }
                if (CurrentLocation.lockedExits.ContainsKey(InteractedPart))
                {
                    (Location lockedExitLocation, Item requiredKey) = CurrentLocation.lockedExits[InteractedPart];
                    if (requiredKey.Func == item.Func)
                    {
                        // Unlock the locked exit
                        CurrentLocation.AddExit(InteractedPart, lockedExitLocation);
                        CurrentLocation.lockedExits.Remove(InteractedPart);
                        backpack.del(itemName);
                        game.audioManager.PlayEffect("assets/audio/UseKey.wav");
                        Console.WriteLine("Opened the door!");
                    }
                    else
                    {
                        Console.WriteLine("The key does not fit this door.");
                    }
                }

                break;

            default:
                Console.WriteLine("Invalid action type.");
                break;
        }



        return false;
    }
    private bool AreAllEnemiesDead()
    {
        foreach (var enemy in CurrentLocation.enemies.Values)
        {
            if (enemy.IsAlive())
            {
                return false;
            }
        }


        return true;
    }
    public void EnemiesAttack()
    {
        foreach (var enemy in CurrentLocation.enemies.Values)
        {
            if (enemy.IsAlive())
            {
                Hit(enemy.attack());


            }
        }
    }
}