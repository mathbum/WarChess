using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public static class Utils {
		public static List<Player> PickPriority(List<Player> players) {
			Random rnd = new Random();
			return players.OrderBy(item => rnd.Next()).ToList();
		}

		public static int GenerateRandomNumber(int max) {
			Random rand = new Random();
			return rand.Next(0, max);
		}
	}
}
