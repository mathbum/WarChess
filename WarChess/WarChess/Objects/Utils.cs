using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public static class Utils {
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
		public static bool ResolveStrike(int strength,int defense) {
			return RandomBoolByPercent(Config.WoundChart[strength][defense]);
		}

		public static int GenerateRandomInt(int max) {
			return rand.Next(0, max);
		}
		public static bool RandomBoolByPercent(double SuccessPercent) {
			return rand.NextDouble() <= SuccessPercent;			
		}
	}
}
