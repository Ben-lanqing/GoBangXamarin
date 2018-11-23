using System;
using System.Collections.Generic;
using System.Text;

namespace GoBangCL.Standard.Model
{
    public class Board
    {
        /// <summary>
        /// 二维点集（0-14）
        /// </summary>
        public Piece[,] Table { private set; get; }
        public int Step { private set; get; }
        public List<Piece> DownPieces { private set; get; }
        public List<Piece> RelatedPieces { private set; get; }
        public bool IsWin { private set; get; }
        public Board()
        {
            Table = new Piece[15, 15];
            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 15; y++)
                {
                    this.Table[x, y] = new Piece(x, y, ColourEnum.Empty);
                }
            }
            Step = 0;
            DownPieces = new List<Piece>();
            RelatedPieces = new List<Piece>();
        }


        public Board Clone()
        {
            var item = new Board();
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    item.Table[i, j] = Table[i, j].Clone();
                }
            }
            item.Step = Step;
            item.DownPieces = Piece.ClonePieceList(DownPieces);
            item.RelatedPieces = Piece.ClonePieceList(RelatedPieces);
            return item;

        }
        public Board ChangeBoard(int x, int y, int step)
        {
            var newBoard = Clone();
            newBoard.Step = step;

            bool isBlack = step % 2 == 1;
            var colour = isBlack ? ColourEnum.Black : ColourEnum.White;
            Piece p = newBoard.Table[x, y];
            p.Colour = colour;
            p.Step = step;

            newBoard.DownPieces.Add(p);
            newBoard.UpdateRelatedPieces(x, y);
            IsWin = CheckIsWin(x, y);
            return newBoard;
        }

        public ColourEnum[] GetLinePieces(int x, int y, int direction)
        {
            ColourEnum[] line = new ColourEnum[9];
            try
            {
                int index = 0; ColourEnum colour = ColourEnum.Empty;
                for (int i = -4; i <= 4; i++)
                {
                    index = 4 + i;
                    if (i == 0) continue;
                    if (direction == 0)
                    {
                        if (x + i >= 0 && x + i < 15) // -向
                        {
                            colour = Table[x + i, y].Colour;
                        }
                        else
                        {
                            colour = ColourEnum.Out;
                        }

                    }
                    if (direction == 1)
                    {
                        if (y + i >= 0 && y + i < 15) // |向
                        {
                            colour = Table[x, y + i].Colour;
                        }
                        else
                        {
                            colour = ColourEnum.Out;
                        }

                    }
                    if (direction == 2)
                    {
                        if (x - i >= 0 && x - i < 15 && y + i >= 0 && y + i < 15) // /向
                        {
                            colour = Table[x - i, y + i].Colour;
                        }
                        else
                        {
                            colour = ColourEnum.Out;
                        }

                    }
                    if (direction == 3)
                    {
                        if (x + i >= 0 && x + i < 15 && y + i >= 0 && y + i < 15) // \向
                        {
                            colour = Table[x + i, y + i].Colour;
                        }
                        else
                        {
                            colour = ColourEnum.Out;
                        }

                    }
                    line[index] = colour;
                }
            }
            catch (Exception e)
            {
            }
            return line;
        }

        private void UpdateRelatedPieces(int x, int y)
        {
            RelatedPieces.RemoveAll(a => a.X == x && a.Y == y);
            for (int i = -5; i <= 5; i++)
            {
                if (i == 0) continue;
                //相关点不超出棋盘范围
                if (x + i >= 0 && x + i < 15) // -向
                {
                    var item = Table[x + i, y];
                    if (!RelatedPieces.Exists(a => a.X == item.X && a.Y == item.Y))
                        RelatedPieces.Add(item);
                }
                if (y + i >= 0 && y + i < 15) // |向
                {
                    var item = Table[x, y + i];
                    if (!RelatedPieces.Exists(a => a.X == item.X && a.Y == item.Y))
                        RelatedPieces.Add(item);

                }
                if (x + i >= 0 && x + i < 15 && y + i >= 0 && y + i < 15) // \向
                {
                    var item = Table[x + i, y + i];
                    if (!RelatedPieces.Exists(a => a.X == item.X && a.Y == item.Y))
                        RelatedPieces.Add(item);

                }
                if (x - i >= 0 && x - i < 15 && y + i >= 0 && y + i < 15) // /向
                {
                    var item = Table[x - i, y + i];
                    if (!RelatedPieces.Exists(a => a.X == item.X && a.Y == item.Y))
                        RelatedPieces.Add(item);

                }
            }
        }

        private bool CheckIsWin(int x, int y)
        {
            //4个方向
            for (int i = 0; i < 4; i++)
            {
                int count = 0;
                ColourEnum[] line = GetLinePieces(x, y, i);
                //落子点棋色
                ColourEnum colour = Step % 2 == 1? ColourEnum.Black:ColourEnum.White;
                for (int j = 0; j < 9; j++)
                {
                    //不同色时计数清0
                    if (line[j] != colour)
                    {
                        count = 0;
                    }
                    else
                    {
                        count++;
                        if (count == 5)
                        {
                            //白5连赢
                            if (colour == ColourEnum.White)
                            {
                                return true;
                            }
                            //黑长连禁手
                            if (line[j + 1] != colour)
                            {
                                return true;
                            }

                        }

                    }
                }

            }
            return false;
        }
    }
}
