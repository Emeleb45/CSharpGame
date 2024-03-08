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
    public bool Put(string itemName, Item item, int count)
    {

        if (item == null || count <= 0)
        {
            return false;
        }

        if (FreeWeight() < item.Weight * count)
        {
            Console.WriteLine(itemName + " didt not fit.");
            return false;
        }
        if (items.ContainsKey(itemName))
        {
            items[itemName].Count += count;
        }
        else
        {
            items.Add(itemName, item);
            item.Count = count;
        }




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
            result += $"{itemEntry.Key} [Weight: {itemEntry.Value.Weight}, || Description: {itemEntry.Value.Description}, || Count: {itemEntry.Value.Count}]\n";
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
    public Item del(string itemName, int count)
    {
        if (items.ContainsKey(itemName))
        {
            Item removedItem = items[itemName];
            if (removedItem.Count > count)
            {
                removedItem.Count -= count;
            }
            else
            {
                items.Remove(itemName);
            }

            return null;
        }
        else
        {
            Console.WriteLine("There is no " + itemName + "dumass");
            return null;
        }
    }

}
