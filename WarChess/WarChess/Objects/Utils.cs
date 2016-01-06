using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public static class Utils {
		public static List<Player> PickPriority(List<Player> players) {
			Player firstplayerlasttime = players[0];
			bool similarorder = false;
			Dictionary<Player,int> PlayerPosition = new Dictionary<Player, int>{ };
			for(int i = 0; i < players.Count; i++) {
				PlayerPosition[players[i]]= i;
			}

			Random rnd = new Random();
			List<Player> neworder = players.OrderBy(item => rnd.Next()).ToList();
			for (int i = 0; i < neworder.Count; i++) {
				if(PlayerPosition[neworder[i]] + i > neworder.Count) {//this means they were in the back half of priority twice in a row
					similarorder = true;
				}
			}

			if (firstplayerlasttime == neworder[0] || similarorder) {
				if (GenerateRandomNumber(6) == 5) {//1 out of 6 chance to get a new order. (like rolling a 6 on a d6)
					neworder = players.OrderBy(item => rnd.Next()).ToList(); //TODO does this re-randomize?
				}
			}
			return neworder;
		}

		public static int GenerateRandomNumber(int max) {
			Random rand = new Random();
			return rand.Next(0, max);
		}
	}
}
