using System;

class Parser
{

	private readonly CommandLibrary commandLibrary;

	// Constructor
	public Parser()
	{
		commandLibrary = new CommandLibrary();
	}


	public Command GetCommand()
	{
		Console.Write("> ");

		string word1 = null;
		string word2 = null;
		string word3 = null;

		string[] words = Console.ReadLine().Split(' ');
		if (words.Length > 0) { word1 = words[0]; }
		if (words.Length > 1) { word2 = words[1]; }
		if (words.Length > 2) { word3 = words[2]; }


		if (commandLibrary.IsValidCommandWord(word1))
		{
			return new Command(word1, word2, word3);
		}

		return new Command(null, null, null);
	}


	public void PrintValidCommands()
	{
		Console.WriteLine("Your command are: ");
		Console.WriteLine(commandLibrary.GetCommandsString());
	}
}
