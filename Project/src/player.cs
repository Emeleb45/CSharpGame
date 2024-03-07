class Player
{
    public Location CurrentLocation { get; set; }
    public int health;
    private Inventory backpack;

    public bool bleeding;



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
        bleeding = true;
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



        Console.WriteLine("Used Item but its not implemented yet so L.");
        
        return false;
    }
}