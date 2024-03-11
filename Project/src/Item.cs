class Item
{
    public int Weight { get; }


    public string Func { get; }
    public string Type { get; }

    public string Description { get; }


    public Item(int weight, string func, string type, string description)
    {
        Weight = weight;
        Func = func;
        Type = type;
        Description = description;


    }

}