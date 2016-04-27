<<<<<<< HEAD
//////////////////////////////////////////////////////////
// TODO:
// Add in black spirit skill
// Add in timed kicks
// LOS checking?
//////////////////////////////////////////////////////////

using System;
using Viper.Scripting.Core.Classes;
using Viper.Scripting.Core.Interfaces;

namespace SorcTestOverride
{
    class ShadowSorc: CCombat
    {
        public IGame MyHelper = null;//Access to bot API
		public int CombatState = 0;

        public override string Name
        {
            get { return "SorcTest"; }
        }

        public override string Author
        {
            get { return "ShadowBot"; }
        }

        public override string Description
        {
            get{ return "Basic Combat Test Script - For Sorc!"; }
        }

        public override float Version
        {
            get { return 0.1f; }
        }

        //Optional web url for plugin description
        public override string Url
        {
            get { return "http://www.mmoviper.com"; }
        }

		public class ATimer
        {
            private long ticks;

            public ATimer()
            {
                ticks = DateTime.Now.Ticks;
            }

            public void Reset()
            {
                ticks = DateTime.Now.Ticks;
            }

            public double ElapsedMilliseconds
            {
                get
                {
                    return Convert.ToDouble(new System.TimeSpan(DateTime.Now.Ticks).TotalMilliseconds - new System.TimeSpan(ticks).TotalMilliseconds);
                }
            }

            public double ElapsedSeconds
            {
                get { return Convert.ToDouble(new System.TimeSpan(DateTime.Now.Ticks).TotalSeconds - new System.TimeSpan(ticks).TotalSeconds); }
            }

        }
		
		
        public override void OnPulse()
        {
            
        }

		////////////////////////////////////////////// Spells ////////////////////////////////////////////////
		ISpell Spell_NightCrow = null;
		ISpell Spell_Claws = null;
		ISpell Spell_DarkFlame = null;
		ISpell Spell_DarkSplit = null;
		ISpell Spell_DarknessReleased = null;
		ISpell Spell_HighKick = null;
		ISpell Spell_FlowOfDark = null;
		ISpell Spell_RushingCrow = null;
		ISpell Spell_AbsDark = null;
		ISpell Spell_BlackWave = null;
		ISpell Spell_Agony = null;
		ISpell Spell_Shield = null;

		////////////////////////////////////////////// Timers ////////////////////////////////////////////////
        ATimer Shard_Cooldown = new ATimer();
		ATimer Agony_Cooldown = new ATimer();
		ATimer Absorb_Cooldown = new ATimer();
		ATimer DarkFlame_Cooldown = new ATimer();
		ATimer BlackWave_Cooldown = new ATimer();
		ATimer RushingCrow_Cooldown = new ATimer();
		ATimer Shield_Cooldown = new ATimer();
		
		////////////////////////////////////////////// Main Attack Block /////////////////////////////////////
        public override void OnAttack()
        {
             // What Mob is it?
            IMob mob = MyHelper.Target;
            int mobCount = MyHelper.GetAttackers.Count;

            // Grab Player Info
            IMob Player = MyHelper.Player;
            IPlayer PlayerStamina = MyHelper.Player;

            // Debug Code
            MyHelper.Log.WriteLine("Attackers = " + MyHelper.GetAttackers.Count);
            MyHelper.Log.WriteLine("OnAttack Called for: " + mob.Name + " at distance: " + mob.DistanceTo(MyHelper.Player));
			
			///////////////////////////////// Check MP //////////////////////////////////////////////////////
			if (Player.CurMP < 50 && Absorb_Cooldown.ElapsedSeconds > 30)
			{
                MyHelper.Log.WriteLine("Absorb Shards!");
                MyHelper.Input.keysDown("{q}");		
				System.Threading.Thread.Sleep(500);
				MyHelper.Input.keysUp("{q}");	
				Absorb_Cooldown.Reset();
			}
			///////////////////////////////// Check Shield //////////////////////////////////////////////////////
			if (Spell_Shield != null && Shield_Cooldown.ElapsedMilliseconds > Spell_Shield.Cooldown)
			{
                MyHelper.Log.WriteLine("Shield Up!");
                MyHelper.Input.keysDown("{QUICKSLOT3}");
				System.Threading.Thread.Sleep(500);
				MyHelper.Input.keysUp("{QUICKSLOT3}");	
				Absorb_Cooldown.Reset();
			}
			///////////////////////////////// If mob is at range, between 6 and 15 then pull with agony /////////////////////////////////
			if (mob.DistanceTo(MyHelper.Player) > 6 && mob.DistanceTo(MyHelper.Player) < 15 && Agony_Cooldown.ElapsedSeconds > 10)
            {
                MyHelper.Log.WriteLine("Pulling!");
                MyHelper.Navigation.Stop();
                MyHelper.Input.keysDown("{S}{E}");

                ATimer faceT = new ATimer();
                while (faceT.ElapsedMilliseconds < 500)
				{
					MyHelper.Navigation.FaceMob(mob);
					System.Threading.Thread.Sleep(10);
					if (mob.HP == 0)
					{
						MyHelper.Input.keysUp("{S}{E}");
						return;
					}
				}

				MyHelper.Input.keysUp("{S}{E}");
				Agony_Cooldown.Reset();
                return;
            }
				
				
			///////////////////////////////// If mob is still at range, or agony is on CD then close distance with rushing crow /////////////////////////////////	
            if (mob.DistanceTo(MyHelper.Player) > 3 && mob.DistanceTo(MyHelper.Player) < 12 && RushingCrow_Cooldown.ElapsedMilliseconds > Spell_RushingCrow.Cooldown)
            {
				MyHelper.Log.WriteLine("Closing!");
                MyHelper.Navigation.Stop();
                MyHelper.Input.keysDown("{W}{RMB}");

                ATimer faceT = new ATimer();
                while (faceT.ElapsedMilliseconds < 500)
				{
					MyHelper.Navigation.FaceMob(mob);
					System.Threading.Thread.Sleep(10);
					if (mob.HP == 0)
					{
						MyHelper.Input.keysUp("{W}{RMB}");
						return;
					}
				}

                MyHelper.Input.keysUp("{W}{RMB}");
				RushingCrow_Cooldown.Reset();
                return;
			}
			
			///////////////////////////////// Still at range? Get your butt in there! /////////////////////////////////
            else if (mob.DistanceTo(MyHelper.Player) > 3)
            {
                MyHelper.Log.WriteLine("Moving to Mob!");
                MyHelper.Navigation.MoveTo(mob, 2, true);
            }
			else
			{
				MyHelper.Navigation.Stop();

                ///////////////////////////////// Melee Begin ////////////////////////////////////
				
                //////////////////////////////////////////////////////////////////////////////////
				///////////////////////////////// Single Mob /////////////////////////////////////
				//////////////////////////////////////////////////////////////////////////////////
				if(mobCount < 3)
				{
				///////////////////////////////// Claws and Dark Flame ///////////////////////////
					if (Spell_Claws != null)
					{
						MyHelper.Log.WriteLine("Claws and Dark Flame");
						MyHelper.Input.keysDown("{S}{LMB}");
						System.Threading.Thread.Sleep(500);
						ATimer faceT = new ATimer();
						while (faceT.ElapsedMilliseconds < 500)
						{
							MyHelper.Navigation.FaceMob(mob);
							System.Threading.Thread.Sleep(10);
							if (mob.HP == 0)
							{
								MyHelper.Input.keysUp("{S}{LMB}");
								return;
							}
						}
						faceT.Reset();
						
						MyHelper.Input.keysUp("{S}{LMB}");
						if (Spell_DarkFlame != null && (DarkFlame_Cooldown.ElapsedMilliseconds > Spell_DarkFlame.Cooldown))
						{
							MyHelper.Log.WriteLine("Dark Flame Combo");
							MyHelper.Input.keysDown("{S}{LMB}{RMB}");
							System.Threading.Thread.Sleep(500);
							MyHelper.Input.keysUp("{S}");
							while (faceT.ElapsedMilliseconds < 500)
							{
								MyHelper.Navigation.FaceMob(mob);
								System.Threading.Thread.Sleep(10);
								if (mob.HP == 0)
								{
									MyHelper.Input.keysUp("{LMB}{RMB}");
									return;
								}
							}

							System.Threading.Thread.Sleep(500);
							MyHelper.Input.keysUp("{LMB}{RMB}");
							
							DarkFlame_Cooldown.Reset();
							
							return;
						}
						return;
	
					}
				
				}
				
				//////////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////// Multiple Mobs //////////////////////////////////
				//////////////////////////////////////////////////////////////////////////////////
				if(mobCount >=3)
				{
				///////////////////////////////// Shard Explosion ////////////////////////////////
					if (Shard_Cooldown.ElapsedSeconds > 29)
					{
	                    MyHelper.Log.WriteLine("Explode all the shards!");
						MyHelper.Input.keysDown("{QUICKSLOT1}");
						System.Threading.Thread.Sleep(500);
						MyHelper.Input.keysUp("{QUICKSLOT1}");
						Shard_Cooldown.Reset();
					}
				///////////////////////////////// Black Wave /////////////////////////////////////
					if (Spell_AbsDark != null && (BlackWave_Cooldown.ElapsedMilliseconds > Spell_BlackWave.Cooldown)) 
						{
	                    MyHelper.Log.WriteLine("Absolute Darkness and Black Wave Combo");
						MyHelper.Navigation.FaceMob(mob);
						if (mob.DistanceTo(MyHelper.Player) < 5)
						{
							MyHelper.Log.WriteLine("Backing up a little");
							MyHelper.Input.keysDown("{Shift}{S}");
							System.Threading.Thread.Sleep(500);
							MyHelper.Input.keysUp("{Shift}{S}");
						}
						if (mob.DistanceTo(MyHelper.Player) > 10)
						{
							MyHelper.Log.WriteLine("Moving up a little");
							MyHelper.Input.keysDown("{Shift}{W}{LMB}");
							System.Threading.Thread.Sleep(500);
							MyHelper.Input.keysUp("{Shift}{W}{LMB}");
						}

						if (Spell_BlackWave != null && (BlackWave_Cooldown.ElapsedMilliseconds > Spell_BlackWave.Cooldown))
						{
							MyHelper.Input.keysDown("{S}{RMB}");
							System.Threading.Thread.Sleep(500);
							MyHelper.Input.keysDown("{S}{LMB}");
							
							ATimer faceT = new ATimer();
							while (faceT.ElapsedMilliseconds < 5000)
							{
								MyHelper.Navigation.FaceMob(mob);
								System.Threading.Thread.Sleep(10);
								if (mob.HP == 0)
								{
									MyHelper.Input.keysUp("{S}{LMB}{RMB}");
									return;
								}
							}				
							MyHelper.Input.keysUp("{S}{LMB}{RMB}");
							BlackWave_Cooldown.Reset();
							return;
						}
						
						BlackWave_Cooldown.Reset();
						return;
					}
					///////////////////////////////// Claws and Dark Flame ///////////////////////////
					if (Spell_Claws != null)
					{
						MyHelper.Log.WriteLine("Claws and Dark Flame");
						MyHelper.Input.keysDown("{S}{LMB}");
						System.Threading.Thread.Sleep(500);
						ATimer faceT = new ATimer();
						while (faceT.ElapsedMilliseconds < 500)
						{
							MyHelper.Navigation.FaceMob(mob);
							System.Threading.Thread.Sleep(10);
							if (mob.HP == 0)
							{
								MyHelper.Input.keysUp("{S}{LMB}");
								return;
							}
						}
						faceT.Reset();
						
						MyHelper.Input.keysUp("{S}{LMB}");
						if (Spell_DarkFlame != null && (DarkFlame_Cooldown.ElapsedMilliseconds > Spell_DarkFlame.Cooldown))
						{
							MyHelper.Log.WriteLine("Dark Flame Combo");
							MyHelper.Input.keysDown("{S}{LMB}{RMB}");
							System.Threading.Thread.Sleep(500);
							MyHelper.Input.keysUp("{S}");
							while (faceT.ElapsedMilliseconds < 500)
							{
								MyHelper.Navigation.FaceMob(mob);
								System.Threading.Thread.Sleep(10);
								if (mob.HP == 0)
								{
									MyHelper.Input.keysUp("{LMB}{RMB}");
									return;
								}
							}

							System.Threading.Thread.Sleep(500);
							MyHelper.Input.keysUp("{LMB}{RMB}");
							
							DarkFlame_Cooldown.Reset();
							
							return;
						}
						return;
	
					}
					
					return;
				}
				return;				
			}

            //If we made it here, then we have nothing else to cast!
            // MyHelper.Input.keysDown("{S}{LMB}");
            // System.Threading.Thread.Sleep(300);
            // MyHelper.Input.keysUp("{S}{LMB}}");

            
            //Comment this out, to bypass the built in OnAttack function (hotkey system)
           // base.OnAttack();
            
        }
       
        public override void OnBotStart(IGame PluginHelper)
        {
            //Save this to get access to bot API
	        MyHelper = PluginHelper;
           //Demo on how to dump spells/abilities... (Not needed or useful unless you dont now the IDS)
           // MyHelper.Log.WriteLine("Dumping all known Spells/Skills/Abilities");
           //Loop through ALL spells in the game.. this can take a few seconds... (about 10 or so normally)
           // ISpells Spells = MyHelper.Spells;
           // foreach (ISpell spell in Spells)
           // {
               // if (spell.isKnown)
               // {
                   // MyHelper.Log.WriteLine(spell.SpellID + "->" + spell.Name + "    /Cooldown: " + spell.Cooldown);
               // }
           // }
		   
            //What Skills do we have available?
			Spell_NightCrow = MyHelper.Spells.GetMaxLevel(1200,1201);
			Spell_Claws = MyHelper.Spells.GetMaxLevel(1202,1203);
			Spell_DarkFlame = MyHelper.Spells.GetMaxLevel(1204,1206);
			Spell_DarkSplit = MyHelper.Spells.GetMaxLevel(1206,1214);
			Spell_DarknessReleased = MyHelper.Spells.GetMaxLevel(1359,1361);
			Spell_HighKick = MyHelper.Spells.GetMaxLevel(1412,1412);
			Spell_FlowOfDark = MyHelper.Spells.GetMaxLevel(1413,1413);
			Spell_RushingCrow = MyHelper.Spells.GetMaxLevel(1417,1419);
			Spell_AbsDark = MyHelper.Spells.GetMaxLevel(1430,1430);
			Spell_BlackWave = MyHelper.Spells.GetMaxLevel(585,588);
			Spell_Shield = MyHelper.Spells.GetMaxLevel(310,311);
			
			//Shard Explosion will be on quickslot 1
			//Shield will be on quickslot 3
			
        }

        public override void OnBotStop()
        {
        }
    }
}
=======
//////////////////////////////////////////////////////////
// TODO:
// Add in black spirit skill
// Add in timed kicks
// LOS checking?
//////////////////////////////////////////////////////////

using System;
using Viper.Scripting.Core.Classes;
using Viper.Scripting.Core.Interfaces;

namespace SorcTestOverride
{
    class ShadowSorc: CCombat
    {
        public IGame MyHelper = null;//Access to bot API
		public int CombatState = 0;

        public override string Name
        {
            get { return "SorcTest"; }
        }

        public override string Author
        {
            get { return "ShadowBot"; }
        }

        public override string Description
        {
            get{ return "Basic Combat Test Script - For Sorc!"; }
        }

        public override float Version
        {
            get { return 0.1f; }
        }

        //Optional web url for plugin description
        public override string Url
        {
            get { return "http://www.mmoviper.com"; }
        }

		public class ATimer
        {
            private long ticks;

            public ATimer()
            {
                ticks = DateTime.Now.Ticks;
            }

            public void Reset()
            {
                ticks = DateTime.Now.Ticks;
            }

            public double ElapsedMilliseconds
            {
                get
                {
                    return Convert.ToDouble(new System.TimeSpan(DateTime.Now.Ticks).TotalMilliseconds - new System.TimeSpan(ticks).TotalMilliseconds);
                }
            }

            public double ElapsedSeconds
            {
                get { return Convert.ToDouble(new System.TimeSpan(DateTime.Now.Ticks).TotalSeconds - new System.TimeSpan(ticks).TotalSeconds); }
            }

        }
		
		
        public override void OnPulse()
        {
            
        }

		////////////////////////////////////////////// Spells ////////////////////////////////////////////////
		ISpell Spell_NightCrow = null;
		ISpell Spell_Claws = null;
		ISpell Spell_DarkFlame = null;
		ISpell Spell_DarkSplit = null;
		ISpell Spell_DarknessReleased = null;
		ISpell Spell_HighKick = null;
		ISpell Spell_FlowOfDark = null;
		ISpell Spell_RushingCrow = null;
		ISpell Spell_AbsDark = null;
		ISpell Spell_BlackWave = null;
		ISpell Spell_Agony = null;
		ISpell Spell_Shield = null;

		////////////////////////////////////////////// Timers ////////////////////////////////////////////////
        ATimer Shard_Cooldown = new ATimer();
		ATimer Agony_Cooldown = new ATimer();
		ATimer Absorb_Cooldown = new ATimer();
		ATimer DarkFlame_Cooldown = new ATimer();
		ATimer BlackWave_Cooldown = new ATimer();
		ATimer RushingCrow_Cooldown = new ATimer();
		ATimer Shield_Cooldown = new ATimer();
		
		////////////////////////////////////////////// Main Attack Block /////////////////////////////////////
        public override void OnAttack()
        {
             // What Mob is it?
            IMob mob = MyHelper.Target;
            int mobCount = MyHelper.GetAttackers.Count;

            // Grab Player Info
            IMob Player = MyHelper.Player;
            IPlayer PlayerStamina = MyHelper.Player;

            // Debug Code
            MyHelper.Log.WriteLine("Attackers = " + MyHelper.GetAttackers.Count);
            MyHelper.Log.WriteLine("OnAttack Called for: " + mob.Name + " at distance: " + mob.DistanceTo(MyHelper.Player));
			
			///////////////////////////////// Check MP //////////////////////////////////////////////////////
			if (Player.CurMP < 50 && Absorb_Cooldown.ElapsedSeconds > 30)
			{
                MyHelper.Log.WriteLine("Absorb Shards!");
                MyHelper.Input.keysDown("{q}");		
				System.Threading.Thread.Sleep(500);
				MyHelper.Input.keysUp("{q}");	
				Absorb_Cooldown.Reset();
			}
			///////////////////////////////// Check Shield //////////////////////////////////////////////////////
			if (Spell_Shield != null && Shield_Cooldown.ElapsedMilliseconds > Spell_Shield.Cooldown)
			{
                MyHelper.Log.WriteLine("Shield Up!");
                MyHelper.Input.keysDown("{QUICKSLOT3}");
				System.Threading.Thread.Sleep(500);
				MyHelper.Input.keysUp("{QUICKSLOT3}");	
				Absorb_Cooldown.Reset();
			}
			///////////////////////////////// If mob is at range, between 6 and 15 then pull with agony /////////////////////////////////
			if (mob.DistanceTo(MyHelper.Player) > 6 && mob.DistanceTo(MyHelper.Player) < 15 && Agony_Cooldown.ElapsedSeconds > 10)
            {
                MyHelper.Log.WriteLine("Pulling!");
                MyHelper.Navigation.Stop();
                MyHelper.Input.keysDown("{S}{E}");

                ATimer faceT = new ATimer();
                while (faceT.ElapsedMilliseconds < 500)
				{
					MyHelper.Navigation.FaceMob(mob);
					System.Threading.Thread.Sleep(10);
					if (mob.HP == 0)
					{
						MyHelper.Input.keysUp("{S}{E}");
						return;
					}
				}

				MyHelper.Input.keysUp("{S}{E}");
				Agony_Cooldown.Reset();
                return;
            }
				
				
			///////////////////////////////// If mob is still at range, or agony is on CD then close distance with rushing crow /////////////////////////////////	
            if (mob.DistanceTo(MyHelper.Player) > 3 && mob.DistanceTo(MyHelper.Player) < 12 && RushingCrow_Cooldown.ElapsedMilliseconds > Spell_RushingCrow.Cooldown)
            {
				MyHelper.Log.WriteLine("Closing!");
                MyHelper.Navigation.Stop();
                MyHelper.Input.keysDown("{W}{RMB}");

                ATimer faceT = new ATimer();
                while (faceT.ElapsedMilliseconds < 500)
				{
					MyHelper.Navigation.FaceMob(mob);
					System.Threading.Thread.Sleep(10);
					if (mob.HP == 0)
					{
						MyHelper.Input.keysUp("{W}{RMB}");
						return;
					}
				}

                MyHelper.Input.keysUp("{W}{RMB}");
				RushingCrow_Cooldown.Reset();
                return;
			}
			
			///////////////////////////////// Still at range? Get your butt in there! /////////////////////////////////
            else if (mob.DistanceTo(MyHelper.Player) > 3)
            {
                MyHelper.Log.WriteLine("Moving to Mob!");
                MyHelper.Navigation.MoveTo(mob, 2, true);
            }
			else
			{
				MyHelper.Navigation.Stop();

                ///////////////////////////////// Melee Begin ////////////////////////////////////
				
                //////////////////////////////////////////////////////////////////////////////////
				///////////////////////////////// Single Mob /////////////////////////////////////
				//////////////////////////////////////////////////////////////////////////////////
				if(mobCount < 3)
				{
				///////////////////////////////// Claws and Dark Flame ///////////////////////////
					if (Spell_Claws != null)
					{
						MyHelper.Log.WriteLine("Claws and Dark Flame");
						MyHelper.Input.keysDown("{S}{LMB}");
						System.Threading.Thread.Sleep(500);
						ATimer faceT = new ATimer();
						while (faceT.ElapsedMilliseconds < 500)
						{
							MyHelper.Navigation.FaceMob(mob);
							System.Threading.Thread.Sleep(10);
							if (mob.HP == 0)
							{
								MyHelper.Input.keysUp("{S}{LMB}");
								return;
							}
						}
						faceT.Reset();
						
						MyHelper.Input.keysUp("{S}{LMB}");
						if (Spell_DarkFlame != null && (DarkFlame_Cooldown.ElapsedMilliseconds > Spell_DarkFlame.Cooldown))
						{
							MyHelper.Log.WriteLine("Dark Flame Combo");
							MyHelper.Input.keysDown("{S}{LMB}{RMB}");
							System.Threading.Thread.Sleep(500);
							MyHelper.Input.keysUp("{S}");
							while (faceT.ElapsedMilliseconds < 500)
							{
								MyHelper.Navigation.FaceMob(mob);
								System.Threading.Thread.Sleep(10);
								if (mob.HP == 0)
								{
									MyHelper.Input.keysUp("{LMB}{RMB}");
									return;
								}
							}

							System.Threading.Thread.Sleep(500);
							MyHelper.Input.keysUp("{LMB}{RMB}");
							
							DarkFlame_Cooldown.Reset();
							
							return;
						}
						return;
	
					}
				
				}
				
				//////////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////// Multiple Mobs //////////////////////////////////
				//////////////////////////////////////////////////////////////////////////////////
				if(mobCount >=3)
				{
				///////////////////////////////// Shard Explosion ////////////////////////////////
					if (Shard_Cooldown.ElapsedSeconds > 29)
					{
	                    MyHelper.Log.WriteLine("Explode all the shards!");
						MyHelper.Input.keysDown("{QUICKSLOT1}");
						System.Threading.Thread.Sleep(500);
						MyHelper.Input.keysUp("{QUICKSLOT1}");
						Shard_Cooldown.Reset();
					}
				///////////////////////////////// Black Wave /////////////////////////////////////
					if (Spell_AbsDark != null && (BlackWave_Cooldown.ElapsedMilliseconds > Spell_BlackWave.Cooldown)) 
						{
	                    MyHelper.Log.WriteLine("Absolute Darkness and Black Wave Combo");
						MyHelper.Navigation.FaceMob(mob);
						if (mob.DistanceTo(MyHelper.Player) < 5)
						{
							MyHelper.Log.WriteLine("Backing up a little");
							MyHelper.Input.keysDown("{Shift}{S}");
							System.Threading.Thread.Sleep(500);
							MyHelper.Input.keysUp("{Shift}{S}");
						}
						if (mob.DistanceTo(MyHelper.Player) > 10)
						{
							MyHelper.Log.WriteLine("Moving up a little");
							MyHelper.Input.keysDown("{Shift}{W}{LMB}");
							System.Threading.Thread.Sleep(500);
							MyHelper.Input.keysUp("{Shift}{W}{LMB}");
						}

						if (Spell_BlackWave != null && (BlackWave_Cooldown.ElapsedMilliseconds > Spell_BlackWave.Cooldown))
						{
							MyHelper.Input.keysDown("{S}{RMB}");
							System.Threading.Thread.Sleep(500);
							MyHelper.Input.keysDown("{S}{LMB}");
							
							ATimer faceT = new ATimer();
							while (faceT.ElapsedMilliseconds < 5000)
							{
								MyHelper.Navigation.FaceMob(mob);
								System.Threading.Thread.Sleep(10);
								if (mob.HP == 0)
								{
									MyHelper.Input.keysUp("{S}{LMB}{RMB}");
									return;
								}
							}				
							MyHelper.Input.keysUp("{S}{LMB}{RMB}");
							BlackWave_Cooldown.Reset();
							return;
						}
						
						BlackWave_Cooldown.Reset();
						return;
					}
					///////////////////////////////// Claws and Dark Flame ///////////////////////////
					if (Spell_Claws != null)
					{
						MyHelper.Log.WriteLine("Claws and Dark Flame");
						MyHelper.Input.keysDown("{S}{LMB}");
						System.Threading.Thread.Sleep(500);
						ATimer faceT = new ATimer();
						while (faceT.ElapsedMilliseconds < 500)
						{
							MyHelper.Navigation.FaceMob(mob);
							System.Threading.Thread.Sleep(10);
							if (mob.HP == 0)
							{
								MyHelper.Input.keysUp("{S}{LMB}");
								return;
							}
						}
						faceT.Reset();
						
						MyHelper.Input.keysUp("{S}{LMB}");
						if (Spell_DarkFlame != null && (DarkFlame_Cooldown.ElapsedMilliseconds > Spell_DarkFlame.Cooldown))
						{
							MyHelper.Log.WriteLine("Dark Flame Combo");
							MyHelper.Input.keysDown("{S}{LMB}{RMB}");
							System.Threading.Thread.Sleep(500);
							MyHelper.Input.keysUp("{S}");
							while (faceT.ElapsedMilliseconds < 500)
							{
								MyHelper.Navigation.FaceMob(mob);
								System.Threading.Thread.Sleep(10);
								if (mob.HP == 0)
								{
									MyHelper.Input.keysUp("{LMB}{RMB}");
									return;
								}
							}

							System.Threading.Thread.Sleep(500);
							MyHelper.Input.keysUp("{LMB}{RMB}");
							
							DarkFlame_Cooldown.Reset();
							
							return;
						}
						return;
	
					}
					
					return;
				}
				return;				
			}

            //If we made it here, then we have nothing else to cast!
            // MyHelper.Input.keysDown("{S}{LMB}");
            // System.Threading.Thread.Sleep(300);
            // MyHelper.Input.keysUp("{S}{LMB}}");

            
            //Comment this out, to bypass the built in OnAttack function (hotkey system)
           // base.OnAttack();
            
        }
       
        public override void OnBotStart(IGame PluginHelper)
        {
            //Save this to get access to bot API
	        MyHelper = PluginHelper;
           //Demo on how to dump spells/abilities... (Not needed or useful unless you dont now the IDS)
           // MyHelper.Log.WriteLine("Dumping all known Spells/Skills/Abilities");
           //Loop through ALL spells in the game.. this can take a few seconds... (about 10 or so normally)
           // ISpells Spells = MyHelper.Spells;
           // foreach (ISpell spell in Spells)
           // {
               // if (spell.isKnown)
               // {
                   // MyHelper.Log.WriteLine(spell.SpellID + "->" + spell.Name + "    /Cooldown: " + spell.Cooldown);
               // }
           // }
		   
            //What Skills do we have available?
			Spell_NightCrow = MyHelper.Spells.GetMaxLevel(1200,1201);
			Spell_Claws = MyHelper.Spells.GetMaxLevel(1202,1203);
			Spell_DarkFlame = MyHelper.Spells.GetMaxLevel(1204,1206);
			Spell_DarkSplit = MyHelper.Spells.GetMaxLevel(1206,1214);
			Spell_DarknessReleased = MyHelper.Spells.GetMaxLevel(1359,1361);
			Spell_HighKick = MyHelper.Spells.GetMaxLevel(1412,1412);
			Spell_FlowOfDark = MyHelper.Spells.GetMaxLevel(1413,1413);
			Spell_RushingCrow = MyHelper.Spells.GetMaxLevel(1417,1419);
			Spell_AbsDark = MyHelper.Spells.GetMaxLevel(1430,1430);
			Spell_BlackWave = MyHelper.Spells.GetMaxLevel(585,588);
			Spell_Shield = MyHelper.Spells.GetMaxLevel(310,311);
			
			//Shard Explosion will be on quickslot 1
			//Shield will be on quickslot 3
			
        }

        public override void OnBotStop()
        {
        }
    }
}
>>>>>>> origin/master
