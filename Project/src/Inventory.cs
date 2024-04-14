class Inventory
{
    private int maxWeight;
    internal Dictionary<string, Item> items;

    public Inventory(int maxWeight)
    {
        this.maxWeight = maxWeight;
        this.items = new Dictionary<string, Item>();

    }
    public int TotalWeight()
    {
        int Total = 0;
        foreach (var item in items.Values)
        {
            Total += item.Weight;
        }

        return Total;
    }
    public int FreeWeight()
    {
        return maxWeight - TotalWeight();
    }
    public bool Put(string itemName, Item item)
    {

        if (item == null)
        {
            return false;
        }

        if (FreeWeight() < item.Weight)
        {
            Console.WriteLine(itemName + " didt not fit.");
            return false;
        }
        while (items.ContainsKey(itemName))
        {

            if (itemName.EndsWith("2") && int.TryParse(itemName.Substring(itemName.Length - 1), out int suffix))
            {
                itemName = itemName.Substring(0, itemName.Length - 1) + (suffix + 1);
            }
            else
            {

                itemName += "2";
            }
        }

        items.Add(itemName, item);

        return true;





    }
    public string getallitems()
    {
        string result = "";
        if (items.Count == 0)
        {
            return null;
        }
        foreach (var itemEntry in items)
        {
            result += $"{itemEntry.Key} [Weight: {itemEntry.Value.Weight}, || Description: {itemEntry.Value.Description}]\n";
        }
        return result;
    }
    public Item Get(string itemName)
    {
        if (items.ContainsKey(itemName))
        {
            Item itemthing = items[itemName];


            return itemthing;
        }
        else
        {
            Console.WriteLine("There is no " + itemName + ".");
            return null;
        }

    }
    public Item del(string itemName)
    {
        if (items.ContainsKey(itemName))
        {
            Item removedItem = items[itemName];


            items.Remove(itemName);


            return null;
        }
        else
        {
            Console.WriteLine("There is no " + itemName + ".");
            return null;
        }
    }

}
