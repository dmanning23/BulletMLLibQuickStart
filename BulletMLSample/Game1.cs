using System;
using System.Collections.Generic;
using BulletMLLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BulletMLSample
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	class Game1 : Microsoft.Xna.Framework.Game
	{
		#region Members

		static public GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Texture2D texture;
		static public Myship myship;
		static public Random rand = new Random();
		int timer = 0;
		Mover mover;

		/// <summary>
		/// A list of all the bulletml samples we have loaded
		/// </summary>
		private List<BulletMLParser> _myPatterns = new List<BulletMLParser>();

		/// <summary>
		/// The names of all the bulletml patterns that are loaded, stored so we can display what is being fired
		/// </summary>
		private List<string> _patternNames = List<string>();

		/// <summary>
		/// The current Bullet ML pattern to use to shoot bullets
		/// </summary>
		private int _CurrentPattern = 0;

		#endregion //Members

		#region Methods

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = 320;// 640;
			graphics.PreferredBackBufferHeight = 240;// 480;
			Content.RootDirectory = "Content";

			myship = new Myship();
		}

		protected override void Initialize()
		{
			base.Initialize();
			//©‹@‰Šú‰»
			
			myship.Init();
		}

		public float GetRank() { return 0; }

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			texture = Content.Load<Texture2D>("Sprites\\bullet");

			//Get all the xml files in the Content\\Samples directory
			foreach (var source in Directory.GetFiles("Content\\Samples", "*.xml"))
			{
				//store the name
				_patternNames.Add(source);

				//load the pattern
				BulletMLParser pattern = new BulletMLParser();
				pattern.ParseXML(source);
			}

			BulletMLManager.GameDifficulty = this.GetRank;
			BulletMLManager.PlayerPosition = myship.Position;

			//“G‚ğˆê‚Â‰æ–Ê’†‰›‚Éì¬‚µA’e‚ğ“f‚­‚æ‚¤İ’è
			mover = MoverManager.CreateMover();
			mover.pos = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
			mover.SetBullet(_patternNames[_CurrentPattern].tree); //BulletML‚Å“®‚©‚·‚æ‚¤‚Éİ’è
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				this.Exit();
			}

			//TODO: check input to increment/decrement the current bullet pattern

			timer++;
			if (timer > 60)
			{
				timer = 0;
				if (mover.used == false)
				{
					//“G‚ğˆê‚Â‰æ–Ê’†‰›‚Éì¬‚µA’e‚ğ“f‚­‚æ‚¤İ’è
					mover = MoverManager.CreateMover();
					mover.pos = new Vector2(graphics.PreferredBackBufferWidth / 4 + graphics.PreferredBackBufferWidth / 2 * (float)rand.NextDouble(), graphics.PreferredBackBufferHeight / 2 * (float)rand.NextDouble());
					mover.SetBullet(_patternNames[_CurrentPattern].tree); //BulletML‚Å“®‚©‚·‚æ‚¤‚Éİ’è
				}
			}

			//‚·‚×‚Ä‚ÌMover‚ğs“®‚³‚¹‚é
			MoverManager.Update();
			//g‚í‚È‚­‚È‚Á‚½Mover‚ğ‰ğ•ú
			MoverManager.FreeMovers();
			// ©‹@‚ğXV
			myship.Update();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			//“G‚â’e‚ğ•`‰æ
			spriteBatch.Begin();

			foreach (Mover mover in MoverManager.movers)
				spriteBatch.Draw(texture, mover.pos, Color.Black);

			spriteBatch.Draw(texture, myship.pos, Color.Black);

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}

	#endregion //Methods
}
