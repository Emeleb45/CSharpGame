class Enemy
{
    public int health;
    public int Armor;
    public bool bleeding;
    public string Name;
    public string EniType;
    private Inventory backpack;
    public Location CurrentLocation { get; set; }
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
        if (health <= 0)
        {
            health = 0;
            Die();
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
    public void Die()
    {
        Console.WriteLine($"{Name} died and dropped his items.");
        DropItems();

    }

    public bool IsAlive()
    {
        return health > 0;
    }
    public Enemy(int armor, string type)
    {
        Armor = armor;
        Name = "";
        EniType = type;
        backpack = new Inventory(75);
        health = 100;
        bleeding = false;
        if (type == "Crab")
        {
            backpack.Put("crabscale", new Item(5, "0", $"Item", "Scale of a crab."));
        }


    }
    public bool DropItems()
    {

        if (CurrentLocation != null && CurrentLocation.enemies.Count > 0)
        {
            foreach (var itemEntry in backpack.items.ToList())
            {
                string itemName = itemEntry.Key;
                Item item = itemEntry.Value;





                CurrentLocation.Chest.Put(itemName, item);
                backpack.del(itemName);

            }
        }

        return false;
    }
    public int attack()
    {

        if (EniType == "Crab")
        {
            Console.WriteLine("Crab attacked you");
            return 5;
        }
        return 0;
    }
}