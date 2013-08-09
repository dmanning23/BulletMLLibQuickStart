using System;
using System.Diagnostics;
using System.Collections.Generic;
using BulletMLLib;
using Microsoft.Xna.Framework;

namespace BulletMLSample
{
	class MoverManager : IBulletManager
	{
		#region Members

		public List<Mover> movers = new List<Mover>();

		public PositionDelegate GetPlayerPosition;

		private float _timeSpeed = 1.0f;
		private float _scale = 1.0f;

		#endregion //Members

		#region Properties

		/// <summary>
		/// How fast time moves in this game.
		/// Can be used to do slowdown, speedup, etc.
		/// </summary>
		/// <value>The time speed.</value>
		public float TimeSpeed 
		{ 
			get
			{
				return _timeSpeed;
			}
			set
			{
				//set my time speed
				_timeSpeed = value;

				//set all the bullet time speeds
				foreach (Mover myDude in movers)
				{
					myDude.TimeSpeed = _timeSpeed;
				}
			}
		}

		/// <summary>
		/// Change the size of this bulletml script
		/// If you want to reuse a script for a game but the size is wrong, this can be used to resize it
		/// </summary>
		/// <value>The scale.</value>
		public float Scale
		{ 
			get
			{
				return _scale;
			}
			set
			{
				//set my scale
				_scale = value;

				//set all the bullet scales
				foreach (Mover myDude in movers)
				{
					myDude.Scale = _scale;
				}
			}
		}

		#endregion //Properties

		public MoverManager(PositionDelegate playerDelegate)
		{
			Debug.Assert(null != playerDelegate);
			GetPlayerPosition = playerDelegate;
		}

		/// <summary>
		/// a mathod to get current position of the player
		/// This is used to target bullets at that position
		/// </summary>
		/// <returns>The position to aim the bullet at</returns>
		/// <param name="targettedBullet">the bullet we are getting a target for</param>
		public Vector2 PlayerPosition(Bullet targettedBullet)
		{
			//just give the player's position
			Debug.Assert(null != GetPlayerPosition);
			return GetPlayerPosition();
		}
		
		public Bullet CreateBullet()
		{
			Mover mover = new Mover(this);
			mover.TimeSpeed = TimeSpeed;
			mover.Scale = Scale;
			movers.Add(mover); //Moverを登録
			mover.Init(); //初期化
			return mover;
		}
		
		public void RemoveBullet(Bullet deadBullet)
		{
			Mover myMover = deadBullet as Mover;
			if (myMover != null)
			{
				myMover.Used = false;
			}
		}

		public void Update()
		{
			for (int i = 0; i < movers.Count; i++)
			{
				movers[i].Update();
			}

			FreeMovers();
		}

		private void FreeMovers()
		{
			for (int i = 0; i < movers.Count; i++)
			{
				if (!movers[i].Used)
				{
					movers.Remove(movers[i]);
					i--;
				}
			}
		}
	}
}
