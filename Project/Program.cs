using System;
using System.IO;
using NAudio.Wave;
using System.Runtime.InteropServices;
class Program	
{
	static void Main()
	{
		Console.Title = "Temple Escape";
		Game game = new Game();
		game.Play();
	}
}