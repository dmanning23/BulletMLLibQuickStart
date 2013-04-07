using System;
using System.Collections.Generic;
using BulletMLLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using HadoukInput;
using GameTimer;
using FontBuddyLib;

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

		MoverManager _moverManager;

		GameClock _clock;

		InputState _inputState;
		InputWrapper _inputWrapper;

		private FontBuddy _text = new FontBuddy();

		/// <summary>
		/// A list of all the bulletml samples we have loaded
		/// </summary>
		private List<BulletPattern> _myPatterns = new List<BulletPattern>();

		/// <summary>
		/// The names of all the bulletml patterns that are loaded, stored so we can display what is being fired
		/// </summary>
		private List<string> _patternNames = new List<string>();

		/// <summary>
		/// The current Bullet ML pattern to use to shoot bullets
		/// </summary>
		private int _CurrentPattern = 0;

		#endregion //Members

		#region Methods

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);

			Content.RootDirectory = "Content";
			myship = new Myship();

			_clock = new GameClock();
			_inputState = new InputState();
			_inputWrapper = new InputWrapper(PlayerIndex.One, _clock.GetCurrentTime);
			_moverManager = new MoverManager(myship.Position);
		}

		protected override void Initialize()
		{
			myship.Init();

			_clock.Start();

			base.Initialize();
		}

		public float GetRank() { return 0; }

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			_text.LoadContent(Content, "ArialBlack14");

			texture = Content.Load<Texture2D>("Sprites\\bullet");

//			//Get all the xml files in the Content\\Samples directory
//			foreach (var source in Directory.GetFiles("Content\\Samples", "*.xml"))
//			{
				//store the name
			string source = "Content\\Samples\\[1943]_rolling_fire.xml";
				_patternNames.Add(source);

				//load the pattern
				BulletPattern pattern = new BulletPattern();
				pattern.ParseXML(source);
				_myPatterns.Add(pattern);
//			}

			GameManager.GameDifficulty = this.GetRank;

			//?G???????????????????????A?e???f??????????
			mover = (Mover)_moverManager.CreateBullet();
			mover.pos = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
			mover.SetBullet(_myPatterns[_CurrentPattern].RootNode); //BulletML??????????????????
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

			//update the timer
			_clock.Update(gameTime);

			//update the input
			_inputState.Update();
			_inputWrapper.Update(_inputState, false);

			//check input to increment/decrement the current bullet pattern
			if (_inputWrapper.Controller.KeystrokePress[(int)EKeystroke.A])
			{
				//decrement the pattern
				if (0 >= _CurrentPattern)
				{
					//if it is at the beginning, move to the end
					_CurrentPattern = _myPatterns.Count - 1;
				}
				else
				{
					_CurrentPattern--;
				}
			}
			else if (_inputWrapper.Controller.KeystrokePress[(int)EKeystroke.X])
			{
				//increment the pattern
				if ((_myPatterns.Count - 1) <= _CurrentPattern)
				{
					//if it is at the beginning, move to the end
					_CurrentPattern = 0;
				}
				else
				{
					_CurrentPattern++;
				}
			}

			timer++;
			if (timer > 60)
			{
				timer = 0;
				if (mover.used == false)
				{
					//?G???????????????????????A?e???f??????????
					mover = (Mover)_moverManager.CreateBullet();
					mover.pos = new Vector2(graphics.PreferredBackBufferWidth / 4 + graphics.PreferredBackBufferWidth / 2 * (float)rand.NextDouble(), graphics.PreferredBackBufferHeight / 2 * (float)rand.NextDouble());
					mover.SetBullet(_myPatterns[_CurrentPattern].RootNode); //BulletML??????????????????
				}
			}

			//????????Mover???s????????
			_moverManager.Update();
			//?g????????????Mover??????
			_moverManager.FreeMovers();
			// ???@???X?V
			myship.Update();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			//?G???e???`??
			spriteBatch.Begin();

			Vector2 position = Vector2.Zero;

			//say what controller we are checking
			_text.Write(_patternNames[_CurrentPattern], position, Justify.Left, 1.0f, Color.White, spriteBatch);

			foreach (Mover mover in _moverManager.movers)
				spriteBatch.Draw(texture, mover.pos, Color.Black);

			spriteBatch.Draw(texture, myship.pos, Color.Black);

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}

	#endregion //Methods
}
