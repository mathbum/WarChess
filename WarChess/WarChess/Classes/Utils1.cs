using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public static class Utils {
		public static double epsilon = 0.000001;
		public static Random rand = new Random();
		public static List<Player> PickPriority(List<Player> players) {
			Player firstplayerlasttime = players[0];
			bool similarorder = false;
			Dictionary<Player,int> PlayerPosition = new Dictionary<Player, int>{ };
			for(int i = 0; i < players.Count; i++) {
				PlayerPosition[players[i]]= i;
			}

			List<Player> neworder = players.OrderBy(item => rand.Next()).ToList();
			for (int i = 0; i < neworder.Count; i++) {
				if(PlayerPosition[neworder[i]] + i > neworder.Count) {//this means they were in the back half of priority twice in a row
					similarorder = true;
				}
			}

			if (firstplayerlasttime == neworder[0] || similarorder) {
				if (RollD6(1)[0] == 6) {//1 out of 6 chance to get a new order. (like rolling a 6 on a d6)
					neworder = players.OrderBy(item => rand.Next()).ToList(); //TODO does this re-randomize?
				}
			}
			return neworder;
		}
		public static List<int> RollD6(int times) {
			List<int> rolls = new List<int>();
			for(int i=0;i< times; i++) {
				rolls.Add(GenerateRandomInt(6) + 1);
			}
			return rolls;
		}
		//public static bool ResolveStrike(int strength,int defense) {//TODO this will need to be fixed to allow for mights to alter dice rolls
		//	return RandomBoolByPercent(Config.WoundChart[strength][defense]);
		//}
		public static bool ResolveStrike(Unit defender, int strength, double effectiveStrength) {
			double defenderDefense = defender.GetDefense();
			//x+.05*x^2+.005x^3
			//x-.05*z+(x^2)/z+(.1*x^3)/z+5/z		note: this function assumes a max def of 10
			double minimunDamage = effectiveStrength + .05 * Math.Pow(effectiveStrength, 2) + .005 * Math.Pow(effectiveStrength, 3);
			double maxDamage = effectiveStrength - .05 * defenderDefense + .75 * Math.Pow(effectiveStrength, 2) / defenderDefense + .075 * Math.Pow(effectiveStrength, 3) / defenderDefense + 5.0 / defenderDefense;

			double damagePercent = GetBellCurveModifiedPercent();
			double damageDelt = (maxDamage - minimunDamage) * damagePercent + minimunDamage;
			defender.Health -= (int)Math.Round(damageDelt);
			Trace.WriteLine("Min: " + minimunDamage + ", Max: " + maxDamage + ", %dmg: " + damagePercent + ", landed: " + damageDelt);
			return defender.Health <= 0;
		}
		public static bool ResolveStrike(Unit defender, Unit attacker) {//TODO this will need to be fixed to allow for mights to alter dice rolls
			//PercentNeededToDamage(defender, defenderEffectiveness, attacker, attackerEffectiveness);
			
			double luck = (rand.NextDouble() + .5);
			double lander = (attacker.Strength * luck -Math.Sqrt(defender.GetDefense()));
			Trace.WriteLine(attacker.Player.Name + "'s " + attacker.Name + " swung at " + defender.Player.Name + "'s " + defender.Name + " and got a lander of: " + lander +" with luck: "+luck);
			bool landsDmg = lander >= 0;
			//bool landsDmg = (attacker.Strength * (rand.NextDouble() + .5) - Math.Sqrt(defender.GetDefense())) >= 0;
			if(landsDmg) {
				//double defenderEffectiveness = EffectivenessRate(defender);
				double attackerEffectiveness = EffectivenessRate(attacker);

				//effectiveness is compounding with defensivewon strengtheffectiveness bonus
				//drop sensitivity of one of them or make effectiveness only relevant for strengthbonus calculation. make effectivestrength = strength + func(x,z)

				double effectiveStrength = attackerEffectiveness * attacker.Strength;
				double defenderDefense = defender.GetDefense();
				//if(effectiveStrength > defenderDefense) {//(1.5*(x-z)/z)^.6+(x-z)/z
				//	effectiveStrength += Math.Pow(1.5 * (effectiveStrength - defenderDefense) / defenderDefense, .6) + (effectiveStrength - defenderDefense) / defenderDefense;
				//}else {//.1*z*(x-z)^.6
				//	effectiveStrength += -.1 * defenderDefense * Math.Pow((effectiveStrength - defenderDefense), .6);
				//}
				if(effectiveStrength > defenderDefense) {//(1.5*(x-z)/z)^.6+(x-z)/z
					effectiveStrength = attacker.Strength+Math.Pow(1.5 * (effectiveStrength - defenderDefense) / defenderDefense, .6) + (effectiveStrength - defenderDefense) / defenderDefense;
				} else {//.1*z*(x-z)^.6
					effectiveStrength = attacker.Strength -.1 * defenderDefense * Math.Pow((defenderDefense- effectiveStrength), .6); //switched order due to -2^.6 beging imaginary
				}
				//effectiveStrength = effectiveStrength + (effectiveStrength - defenderDefense) / defenderDefense - Math.Pow((defenderDefense - effectiveStrength) / effectiveStrength, .6) * 1.5;

				Trace.WriteLine(attacker.Player.Name + "'s " + attacker.Name + " is fighting at " + attackerEffectiveness*100 + "% and got " + effectiveStrength + " effective strength");
				return ResolveStrike(defender, attacker.Strength, effectiveStrength);
				//based on some stat (fighting) skew normal distribution to be to higher or lower?
				
			}
			return false;

		}//TODO allow loser to also deal minimum dmg?
		//public static double PercentNeededToDamage(Unit defender, double defenderEffectiveness, Unit attacker, double attackerEffectiveness) {
		//	//uses defender dexterity, and attacker fighting.

		//	throw new NotImplementedException();
		//}
		public static double GetBellCurveModifiedPercent() {//returns a double between 0,1 whoes distribution is similar to a bell curve
			//////-1/(b*(2*PI)^.5) * E^ -(x-u)^2/(2*c^2) + 1

			double width = .25;//graph y=E ^ -(x^2)/c    to find a new width if you wish. changes rate at which it will land near the middle
			//also you can do the rest of the equation * height*PI^.5     it is currently set to .4 which makes it 1 thus not requireing the calcuation
			double randDouble = rand.NextDouble();
			double modifiedPercent = Math.Pow(-Math.Log(randDouble) * width, .5);
			return 1 - (modifiedPercent <= 1 ? modifiedPercent : 1); // make sure between 0,1
		}
		public static double EffectivenessRate(Unit unit) {
			double healthPercent = (double)unit.Health / unit.TotalHealth;
			//return Math.Sqrt(healthPercent);
			//double effectiveness = (Math.Log10(healthPercent*10)+1)/2;
			//if(effectiveness <= .01) {//if effectiveness rate is <=1% then its the average of log10 and sqrt
			//	effectiveness = (effectiveness + Math.Sqrt(healthPercent))/2;
			//}
			if(healthPercent < .01){
				healthPercent = .01;//to set a minimun effectiveness (and to make sure no negative effectiveness) [minimum effeciveness is 1/3]
			}
			return Math.Log10(Math.Sqrt(healthPercent)) / 1.5 + 1;//the higher [1.5] is the less you are penalized for low hp
		}

		public static int GenerateRandomInt(int max) {
			return rand.Next(0, max);
		}
		public static bool RandomBoolByPercent(double SuccessPercent) {
			return rand.NextDouble() <= SuccessPercent;			
		}
	}
}
