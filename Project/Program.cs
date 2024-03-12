using System;
using System.IO;
using NAudio.Wave;
using System.Runtime.InteropServices;
class Program	
{
	static void Main(string[] args)
	{
		SetConsoleFullscreen();
		ConsoleHelper.SetCurrentFont("Consolas", 40);
		Console.Title = "ZuulGame";

		Game game = new Game();
		game.Play();
	}

	private static void SetConsoleFullscreen()
	{
		Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
		Console.SetBufferSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

		// Set the console to fullscreen (maximized)
		IntPtr consoleHandle = GetConsoleWindow();
		ShowWindow(consoleHandle, SW_MAXIMIZE);
	}

	[DllImport("kernel32.dll", ExactSpelling = true)]
	private static extern IntPtr GetConsoleWindow();

	[DllImport("user32.dll")]
	private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	private const int SW_MAXIMIZE = 3;
}