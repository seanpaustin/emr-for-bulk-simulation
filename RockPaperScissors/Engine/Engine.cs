using System;

namespace Engine
{
	public class Engine
	{
		public enum Players { NONE, RED, BLUE };
		public enum Actions { ROCK, PAPER, SCISSORS };

		public Players Throw(Actions redAction, Actions blueAction)
		{
			if (redAction == blueAction) {
				return Players.NONE;
			} else {
				switch (redAction) {
				case Actions.ROCK: return blueAction == Actions.PAPER ? Players.BLUE : Players.RED;
				case Actions.PAPER: return blueAction == Actions.SCISSORS ? Players.BLUE : Players.RED;
				case Actions.SCISSORS: return blueAction == Actions.ROCK ? Players.BLUE : Players.RED;
				default: return Players.NONE;
				}
			}
		}
	}

	public interface RPSAI
	{
		Engine.Actions GetNextThrow(Engine.Players player);
		void AddOutcome(Engine.Actions redAction, Engine.Actions blueAction, Engine.Players winner);
	}

	public class RandomAI : RPSAI
	{
		private Random stream;

		public RandomAI(int seed) {
			stream = new Random(seed);
		}

		public virtual Engine.Actions GetNextThrow (Engine.Players player)
		{
			return ChooseRandom ();
		}

		protected Engine.Actions ChooseRandom()
		{
			switch (stream.Next (0, 3)) {
			case 0: return Engine.Actions.ROCK;
			case 1: return Engine.Actions.PAPER;
			case 2: return Engine.Actions.SCISSORS;
			}

			return Engine.Actions.ROCK;
		}

		public virtual void AddOutcome (Engine.Actions redAction, Engine.Actions blueAction, Engine.Players winner)
		{
		}
	}

	public class LetItRideAI : RandomAI
	{
		private Engine.Players lastWinner;
		private Engine.Actions lastRedAction;
		private Engine.Actions lastBlueAction;
		private int draws = 3;

		public LetItRideAI(int seed) : base(seed) {
		}

		public override Engine.Actions GetNextThrow (Engine.Players player)
		{
			if (lastWinner == player) {
				return lastWinner == Engine.Players.RED ? lastRedAction : lastBlueAction;
			} else {
				if (draws > 2)
					return ChooseRandom ();
				else
					return player == Engine.Players.RED ? WhatBeats (lastBlueAction) : WhatBeats (lastRedAction);
			}
		}

		private Engine.Actions WhatBeats(Engine.Actions play)
		{
			switch (play) {
			case Engine.Actions.ROCK: return Engine.Actions.PAPER;
			case Engine.Actions.PAPER: return Engine.Actions.SCISSORS;
			case Engine.Actions.SCISSORS: return Engine.Actions.ROCK;
			}

			return Engine.Actions.ROCK;
		}

		public override void AddOutcome (Engine.Actions redAction, Engine.Actions blueAction, Engine.Players winner)
		{
			lastRedAction = redAction;
			lastBlueAction = blueAction;
			lastWinner = winner;

			if (winner == Engine.Players.NONE)
				++draws;
			else
				draws = 0;
		}
	}

	public class TestHarness
	{
		public static void Main (string[] args)
		{
			if (args.Length == 3) {
				string redAI = args [0];
				string blueAI = args [1];
				int count = int.Parse (args [2]);

				Random stream = new Random ();

				Engine engine = new Engine ();
				LetItRideAI letItRideAI = new LetItRideAI (stream.Next());
				RandomAI randomAI = new RandomAI (stream.Next());

				RPSAI red = (redAI == "LetItRide" ? letItRideAI : randomAI);
				RPSAI blue = (blueAI == "LetItRide" ? letItRideAI : randomAI);

				int redWins = 0;
				int blueWins = 0;
				int draws = 0;

				for (int index = 0; index < count; ++index) {
					Engine.Actions redAction = red.GetNextThrow (Engine.Players.RED);
					Engine.Actions blueAction = blue.GetNextThrow (Engine.Players.BLUE);

					Engine.Players winner = engine.Throw (redAction, blueAction);

					switch (winner) {
					case Engine.Players.NONE:
						++draws;
						break;
					case Engine.Players.RED:
						++redWins;
						break;
					case Engine.Players.BLUE:
						++blueWins;
						break;
					}

					red.AddOutcome (redAction, blueAction, winner);
					blue.AddOutcome (redAction, blueAction, winner);

					string outcome = (winner == Engine.Players.NONE ? "Draw" : (winner == Engine.Players.RED ? "Red" : "Blue"));
					Console.WriteLine (redAI + " " + blueAI + " " + count + " " + outcome);
				}

			} else {
				Console.WriteLine ("Usage: Engine.exe {LetItRide|Random} {LetItRide|Random} {count}");
			}
		}
	}
}

