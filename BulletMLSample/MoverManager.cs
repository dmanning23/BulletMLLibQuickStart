using System;
using System.Diagnostics;
using System.Collections.Generic;
using BulletMLLib;
using Microsoft.Xna.Framework;

namespace BulletMLSample
{
	class MoverManager : IBulletManager
	{
		public List<Mover> movers = new List<Mover>();

		public PositionDelegate GetPlayerPosition;

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
