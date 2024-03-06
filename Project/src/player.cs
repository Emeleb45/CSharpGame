class Player
{
    public Location CurrentLocation { get; set; }
    public int health;
    private Inventory backpack;



    // methods
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


    public Player()
    {
        backpack = new Inventory(75);
        health = 100;
        CurrentLocation = null;
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

                backpack.Put(itemName, item);
                CurrentLocation.Chest.del(itemName);
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
                CurrentLocation.Chest.Put(itemName, item);
                backpack.Get(itemName);
                backpack.del(itemName);
                Console.WriteLine("Dropped "+itemName);
            }

        }

        return false;
    }
}