


using Contracts.BaseClasses;
using Contracts;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Resources;
using GoGameTester.Properties;
namespace GoGame
{
	public class GoGameTester : TesterBase
	{
		public GoGameModel Model { get; } = new GoGameModel();
		Image<Bgr, byte> Image { get; set; } = new Image<Bgr, byte>(30, 30);
		public const double XStart = 47.0;
		public const double XEnd = 1030.0;
		public const double YStart = 51.0;
		public const double YEnd = 1034.0;
		public const double ImageWidth = 1080;
		public const double ImageHeight = 1085;
		private void PutPixel(int x, int y, byte r, byte g, byte b)
		{
			try
			{
				// SendMessage($"画了一个像素({x}, {y})");
				Image.Data[y, x, 0] = b;
				Image.Data[y, x, 1] = g;
				Image.Data[y, x, 2] = r;
			}
			catch(Exception e)
			{
				throw new Exception($"绘制像素时出错: {e.Message}");
			}
		}
		//private void FillRectangle(int x1, int y1, int x2,  int y2, byte r, byte g, byte b)
		//{
		//	for(int i = y1; i < y2;i++)
		//	{
		//		for(int j = x1; j < x2;j++)
		//		{
		//			PutPixel(i, j, r, g, b);
		//		}
		//	}
		//}
		private void DrawSprite(Image<Bgr, byte> sprite, int x, int y)
		{
			try
			{
				byte[,,] data = sprite.Data;
				int rowCount = data.GetLength(0);
				int columnCount = data.GetLength(1);
				for (int i = 0; i < rowCount; i++)
				{
					for (int j = 0; j < columnCount; j++)
					{
						PutPixel(x + j, y + i, data[i, j, 2], data[i, j, 1], data[i, j, 0]);
					}
				}
			}
			catch(Exception e)
			{
				throw new Exception($"绘制精灵时出错: {e.Message}");
			}
		}
		private void DrawCircle(int x, int y, int radius, bool black)
		{
			try
			{
				for (int i = y - radius; i < y + radius + 1; i++)
				{
					for (int j = x - radius; j < x + radius + 1; j++)
					{
						if ((i - x) * (i - x) + (j - y) * (j - y) <= radius * radius)
						{
							if (black)
								PutPixel(j, i, 0, 0, 0);
							else
								PutPixel(j, i, 255, 255, 255);
						}
					}
				}
			}
			catch(Exception e ) { throw new Exception($"绘制圆圈时出错: {e.Message}"); }
		}
		private void DrawBoard()
		{
			try
			{
				Image = Resources.board2.ToImage<Bgr, byte>();
				for (int i = 0; i < 19; i++)
				{
					for (int j = 0; j < 19; j++)
					{
						int xPos = (int)(XStart + (XEnd - XStart) * j / 19.0);
						int yPos = (int)(YStart + (YEnd - YStart) * i / 19.0);
						if (Model.Chessboard[i, j] == Stone.Black)
						{
							SendMessage($"画了一个圆圈: ({xPos}, {yPos})");
							DrawCircle(xPos, yPos, 25, true);
						}
						else if (Model.Chessboard[i, j] == Stone.White)
						{
							SendMessage($"画了一个圆圈: ({xPos}, {yPos})");
							DrawCircle(xPos, yPos, 25, false);
						}
					}
				}
			}
			catch(Exception e)
			{
				throw new Exception($"tester绘图时引发异常: {e.Message}");
			}
		}
		public override void Init()
		{
			DrawBoard();
			UpdateImage(Image);
		}
		public override void OnLeftButtonDown(double x, double y)
		{
			int realX = (int)Math.Round((x*ImageWidth -XStart)/(XEnd - XStart)*18.0);
			int realY = (int)Math.Round((y * ImageHeight - YStart) / (YEnd - YStart) * 18.0);
			SendMessage($"tester收到了: 点击了({realX}, {realY})");
			if (realX >= 19 || realY >= 19)
			{
				return;
			}
			GoMove move = new GoMove(Model.WhoseTurn, realX, realY);
			bool validMove = false;
			try
			{
				validMove = Model.ValidMove(move);
			}
			catch (Exception e)
			{
				throw new Exception($"Model执行valid时出错: {e.Message}");
			}
			
			if (validMove)
			{
				try
				{
					Model.DoMove(move);
				}
				catch(Exception e)
				{
					throw new Exception($"Model执行move时出错: {e.Message}");
				}
				SendMessage($"成功执行了开发者的落子({move.Stone}, {realX}, {realY})");
				DrawBoard();
				UpdateImage(Image);

				//try
				//{
				//	MoveInfo info = ProjSlnFuncProvider!.Execute(Model);
				//	if (info.Succeeded)
				//	{
				//		SendMessage($"AI落子成功");
				//		GoMove aiMove = (GoMove)info.Move!;
				//		SendMessage($"AI的落子: ({aiMove.Stone}, {aiMove.X}, {aiMove.Y})");
				//		if (Model.ValidMove(aiMove))
				//		{
				//			Model.DoMove(aiMove);
				//			DrawBoard();
				//			UpdateImage(Image);
				//			SendMessage($"AI落子合规");
				//		}
				//		else
				//		{
				//			SendMessage("错误: AI落子不合规");
				//		}
				//	}
				//	else
				//	{
				//		SendMessage("AI无法决策如何落子");
				//	}
				//}
				//catch (Exception e)
				//{
				//	SendMessage("调用AI时引发的异常: " + e.Message);
				//}
			}
			else
			{
				SendMessage("非法的移动");
			}
		}
		public override void OnRightButtonDown(double x, double y)
		{
			throw new NotImplementedException();
		}
		public override void OnReceiveMessage(string message)
		{
			SendMessage($"收到了指令: {message}");
			if (message == "clear")
			{
				Model.Chessboard = new Stone[19, 19];
				DrawBoard();
				UpdateImage(Image);
				SendMessage("收到了重置指令");
			}
		}


		public GoGameTester()
		{

		}
	}
}