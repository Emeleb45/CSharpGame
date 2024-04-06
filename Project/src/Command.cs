class Command
{
	public string CommandWord { get; init; }
	public string SecondWord { get; init; }

	public string ThirdWord { get; init; }


	public Command(string first, string second, string third)
	{
		CommandWord = first;
		SecondWord = second;
		ThirdWord = third;
	}



	public bool IsUnknown()
	{
		return CommandWord == null;
	}



	public bool HasSecondWord()
	{
		return SecondWord != null;
	}
	public bool HasThirdWord()
	{
		return ThirdWord != null;
	}
}
