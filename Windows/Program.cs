#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using BulletMLSample;

#endregion

namespace BulletMLLibQuickStart.Windows
{
	static class Program
	{
		private static Game1 game;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main ()
		{
			game = new Game1 ();
			game.Run ();
		}
	}
}
