using System.Collections.Generic;

class CommandLibrary
{

	private readonly List<string> validCommands;


	public CommandLibrary()
	{
		validCommands = new List<string>();

		validCommands.Add("help");
		validCommands.Add("go");
		validCommands.Add("look");
		validCommands.Add("status");
		validCommands.Add("quit");
		validCommands.Add("take");
		validCommands.Add("drop");
		validCommands.Add("die");
		validCommands.Add("use");
		validCommands.Add("run");
	}


	public bool IsValidCommandWord(string instring)
	{
		for (int i = 0; i < validCommands.Count; i++)
		{
			if (validCommands[i] == instring)
			{
				return true;
			}
		}

		return false;
	}


	public string GetCommandsString()
	{
		string str = "";
		for (int i = 0; i < validCommands.Count; i++)
		{
			str += validCommands[i];

			if (i < validCommands.Count - 1)
			{
				str += ", ";
			}
		}
		return str;
	}
}
