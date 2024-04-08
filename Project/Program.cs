using System;
using System.IO;
using NAudio.Wave;
using System.Runtime.InteropServices;
class Program	
{
	static void Main()
	{
		Console.Title = "ZuulGame";
		Game game = new Game();
		game.Play();
	}
}