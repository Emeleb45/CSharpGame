class Player
{
    public Location CurrentLocation { get; set; }
    public int health;
    private Inventory backpack;

    public string Name;
    public int Armor;
    public bool bleeding;
    public bool InCombat;


    // methods
    public int Hit(int amount)
    {
        int damage = amount;
        if (Armor > 0)
        {
            damage -= Armor;

            if (damage < 2)
            {
                damage = 2;
            }
            return damage;
        }


        return damage;
    }
    public void Damage(int amount)
    {
        health -= Hit(amount);
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


    public Player()
    {
        Name = "Blank";
        Armor = 0;
        backpack = new Inventory(75);
        health = 75;
        bleeding = true;
        CurrentLocation = null;
        InCombat = false;
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

                backpack.Put(itemName, item, 1);
                CurrentLocation.Chest.del(itemName, 1);
                Console.WriteLine(itemName + " added to inventory");
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
                CurrentLocation.Chest.Put(itemName, item, 1);
                backpack.Get(itemName);
                backpack.del(itemName, 1);
                Console.WriteLine("Dropped " + itemName);
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
                int DamageValue;
                if (int.TryParse(item.Func, out DamageValue))
                {
                    if (CurrentLocation.enemies.ContainsKey(InteractedPart))
                    {

                        Enemy enemy = CurrentLocation.enemies[InteractedPart];
                        if (enemy.IsAlive())
                        {
                            EnemiesAttack();
                            enemy.Damage(DamageValue);
                            Console.WriteLine($"Attacked {InteractedPart}.");
                            if (AreAllEnemiesDead())
                            {
                                InCombat = false;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{InteractedPart} is already dead.");
                            if (AreAllEnemiesDead())
                            {
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

            case "healingpotion":
                Console.WriteLine("Unfinishied healuse");
                break;

            case "unlock":
                Console.WriteLine("Unfinishied unlock");
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
                // At least one enemy is alive, return false
                return false;
            }
        }

        // All enemies are dead
        return true;
    }
    public void EnemiesAttack()
    {
        foreach (var enemy in CurrentLocation.enemies.Values)
        {
            if (enemy.IsAlive())
            {
                Damage(enemy.attack());


            }
        }
    }
}