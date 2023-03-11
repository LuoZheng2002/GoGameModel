using Contracts.BaseClasses;

namespace GoGame
{
	public enum Stone
	{
		Empty,
		Black,
		White
	}
	public class GoMove : MoveBase
	{
		public Stone Stone { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public GoMove(Stone stone, int x, int y)
		{
			Stone = stone;
			X = x;
			Y = y;
		}
	}
	public class GoGameModel : GameModelBase
	{
		public Stone[,] Chessboard { get; set; } = new Stone[19, 19];
		public Stone WhoseTurn { get; set; }
		public Stone WhoIsAI { get; set; }
		public static Stone EnemyOf(Stone piece)
		{
			if (piece == Stone.Black)
			{
				return Stone.White;
			}
			else if (piece == Stone.White)
			{
				return Stone.Black;
			}
			return Stone.Empty;
		}
		public void SelectBlock(Stack<GoMove> selectedPieces, GoMove testMove)
		{
			selectedPieces.Push(testMove);

			List<GoMove> potentialTestMove = new List<GoMove>();
			if (testMove.X < 18)
			{
				potentialTestMove.Add(new GoMove(testMove.Stone, testMove.X + 1, testMove.Y));
			}
			if (testMove.Y < 18)
			{
				potentialTestMove.Add(new GoMove(testMove.Stone, testMove.X, testMove.Y + 1));
			}
			if (testMove.X > 0)
			{
				potentialTestMove.Add(new GoMove(testMove.Stone, testMove.X - 1, testMove.Y));
			}
			if (testMove.Y > 0)
			{
				potentialTestMove.Add(new GoMove(testMove.Stone, testMove.X, testMove.Y - 1));
			}

			foreach (var testMove2 in potentialTestMove)
			{
				if (selectedPieces.Contains(testMove2))
				{
					continue;
				}
				if (Chessboard[testMove2.Y, testMove2.X] == testMove.Stone)
				{
					SelectBlock(selectedPieces, testMove2);
				}
			}
		}
		public bool haveQi(GoMove goMove)
		{
			Stack<GoMove> pieceBlock = new();
			SelectBlock(pieceBlock, goMove);
			while (pieceBlock.Count > 0)
			{
				GoMove testMove = pieceBlock.Pop();
				bool haveEmptyNeighbor = false;
				if (testMove.Y < 18 && Chessboard[testMove.Y + 1, testMove.X] == Stone.Empty)
				{
					haveEmptyNeighbor = true;
				}
				if (testMove.X < 18 && Chessboard[testMove.Y, testMove.X + 1] == Stone.Empty)
				{
					haveEmptyNeighbor = true;
				}
				if (testMove.Y > 0 && Chessboard[testMove.Y - 1, testMove.X] == Stone.Empty)
				{
					haveEmptyNeighbor = true;
				}
				if (testMove.X > 0 && Chessboard[testMove.Y, testMove.X - 1] == Stone.Empty)
				{
					haveEmptyNeighbor = true;
				}
				if (haveEmptyNeighbor)
				{
					return true;
				}
			}
			return false;
		}
		public override bool ValidMove(MoveBase move)
		{
			GoMove goMove = (GoMove)move;
			if (Chessboard[goMove.Y, goMove.X] == Stone.Empty && haveQi(goMove))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public override void DoMove(MoveBase move)
		{
			GoMove goMove = (GoMove)move;
			Chessboard[goMove.Y, goMove.X] = goMove.Stone;


			//List<GoMove> potentialEliminatedPieces = new List<GoMove>();
			//if (goMove.X < 18)
			//{
			//	potentialEliminatedPieces.Add(new GoMove(goMove.Stone, goMove.X + 1, goMove.Y));
			//}
			//if (goMove.Y < 18)
			//{
			//	potentialEliminatedPieces.Add(new GoMove(goMove.Stone, goMove.X, goMove.Y + 1));
			//}
			//if (goMove.X > 0)
			//{
			//	potentialEliminatedPieces.Add(new GoMove(goMove.Stone, goMove.X - 1, goMove.Y));
			//}
			//if (goMove.Y > 0)
			//{
			//	potentialEliminatedPieces.Add(new GoMove(goMove.Stone, goMove.X, goMove.Y - 1));
			//}

			//foreach(var potentialEliminatedPiece in  potentialEliminatedPieces) 
			//{
			//	if (Chessboard[potentialEliminatedPiece.Y, potentialEliminatedPiece.X] == EnemyOf(goMove.Stone))
			//	{
			//		if (haveQi(potentialEliminatedPiece) == false)
			//		{
			//			Stack<GoMove> selectedPiecesToBeEliminated = new();
			//			SelectBlock(selectedPiecesToBeEliminated, potentialEliminatedPiece);
			//			while (selectedPiecesToBeEliminated.Count > 0)
			//			{
			//				GoMove pieceToBeEliminated = selectedPiecesToBeEliminated.Pop();
			//				Chessboard[pieceToBeEliminated.Y, pieceToBeEliminated.X] = Stone.Empty;
			//			}
			//		}
			//	}
			//}
			WhoseTurn = GoGameModel.EnemyOf(WhoseTurn);
		}
		public override GameModelBase Copy()
		{
			GoGameModel model = new GoGameModel();
			model.WhoseTurn = WhoseTurn;
			model.WhoIsAI = WhoIsAI;
			for (int i = 0; i < 19; i++)
			{
				for (int j = 0; j < 19; j++)
				{
					model.Chessboard[i, j] = Chessboard[i, j];
				}
			}
			return model;
		}
		public GoGameModel()
		{
			WhoseTurn = Stone.Black;
		}
	}
}