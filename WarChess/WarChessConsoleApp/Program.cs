using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarChess.Objects;

namespace WarChessConsoleApp {
	class Program {
		static void Main(string[] args) {
			TestMethod1();
			Console.Read();
		}
		private static void TestMethod1() {
			//make this generic, add Trapping, add ability for multiple units to attack single target.
			List<Player> players = CreateDefaultPlayers();
			Game game = CreateDefaultGame(5, 5, players);

			Unit Goblin = game.CreateUnit("Goblin");
			Unit Uruk = game.CreateUnit("Uruk-hai Captain");
			Goblin.Player = players[0];
			Uruk.Player = players[1];

			game.PlaceUnit(new Position(0, 0), Goblin);
			game.PlaceUnit(new Position(0, 1), Uruk);
			int rounds = 0;
			int roundsUrukWon = 0;
			int roundsGoblinWon = 0;
			int GoblinDmgDealt = 0;
			int UrukDmgDealt = 0;
			int roundsBounced = 0;
			//for(;;) {
			//	if(Goblin.Health > 0 && Uruk.Health > 0) {
			//		game.AddCharge(Goblin, Uruk);
			//		int GoblinHealth = Goblin.Health;
			//		int UrukHealth = Uruk.Health;
			//		game.ResolveConflict(Goblin.Position);
			//		if(GoblinHealth > Goblin.Health) {
			//			roundsUrukWon++;
			//			UrukDmgDealt += GoblinHealth - Goblin.Health;
			//		} else if(UrukHealth > Uruk.Health) {
			//			roundsGoblinWon++;
			//			GoblinDmgDealt += UrukHealth - Uruk.Health;
			//		} else {
			//			roundsBounced++;
			//		}
			//		rounds++;
			//	} else {
			//		break;
			//	}
			//}
			for(;;) {
				if(!Utils.ResolveStrike(Goblin, Uruk)) {
					int GoblinHealth = Goblin.Health;
					int UrukHealth = Uruk.Health;
					if(GoblinHealth > Goblin.Health) {
						roundsUrukWon++;
						UrukDmgDealt += GoblinHealth - Goblin.Health;
					} else if(UrukHealth > Uruk.Health) {
						roundsGoblinWon++;
						GoblinDmgDealt += UrukHealth - Uruk.Health;
					} else {
						roundsBounced++;
					}
					rounds++;
				}else {
					break;
				} 
			}
			string stats = String.Format("Rounds: {0}; Uruk Dealt {1}Dmg in {2} Rounds with Ave Dmg {3}; Goblin Dealt {4}Dmg in {5} Rounds with Ave Dmg {6}; {7} bouncing rounds", rounds, UrukDmgDealt, roundsUrukWon, (double)UrukDmgDealt / roundsUrukWon, GoblinDmgDealt, roundsGoblinWon, (double)GoblinDmgDealt / roundsGoblinWon, roundsBounced);
			Console.WriteLine(stats);
		}
		private static List<Player> CreateDefaultPlayers() {
			List<Player> Players = new List<Player>();
			Players.Add(new Player("player1"));
			Players.Add(new Player("player2"));
			return Players;
		}
		private static Game CreateDefaultGame(int rows, int cols, List<Player> Players) {
			BoardManager BM;
			BM = new BoardManager(rows, cols);
			return new Game(BM, Players);
		}
	}
}
