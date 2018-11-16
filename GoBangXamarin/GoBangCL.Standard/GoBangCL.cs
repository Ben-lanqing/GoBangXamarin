using GoBangCL.Standard.Model;
using System;
using System.Collections.Generic;

namespace GoBangCL.Standard
{
    public class GoBangHepler
    {

        private Board Board;
        private int Lev = 0;
        private int Step;
        //private ColourEnum Colour;

        public GoBangHepler(Board board)
        {
            Board = board;
            Step = board.Step;
            //Colour = board.Colour;
        }

        /// <summary>
        /// AI取点
        /// </summary>
        /// <param name="board"></param>
        /// <param name="step"></param>
        /// <param name="Lev"></param>
        /// <returns></returns>
        public PieceInfo AIGetNext(Board board, int Lev = 0)
        {
            List<PieceInfo> returnLt = new List<PieceInfo>();
            BoardInfo boardInfo = new BoardInfo(board);
            StateEnum flag = boardInfo.DeepAnalyze();
            //StateEnum flag = boardInfo.Result;
            returnLt = boardInfo.MaxLt;

            PieceInfo point = returnLt[0];
            if (boardInfo.MaxLev == LevelEnum.Five_1)
            {
                return point;
            }
            if (flag == StateEnum.Loss || flag == StateEnum.D)
            {
                point = MoreDefend(boardInfo, board);
            }
            else
            {
                point = GetBest(returnLt);
            }

            return point;
        }
        #region
        /// <summary>
        /// 在点列中取双方级别最高的点(按级别取点)
        /// </summary>
        /// <param name="List">点列</param>
        /// <returns></returns>
        private PieceInfo GetBest(List<PieceInfo> List)
        {
            if (List.Count == 1)
                return List[0];
            List<PieceInfo> ReturnDl = new List<PieceInfo>();
            double maxLev = 9999999;
            foreach (PieceInfo d in List)
            {
                double LevelB = (double)d.Levels[ColourEnum.Black];
                double LevelW = (double)d.Levels[ColourEnum.White];

                for (int i = 0; i < 4; i++)
                {
                    if (d.Names[ColourEnum.Black][i].Equals("t3"))
                        LevelB = LevelB + 0.1;
                    if (d.Names[ColourEnum.White][i].Equals("t3"))
                        LevelW = LevelW + 0.1;
                }

                if (LevelB + LevelW == maxLev)
                    ReturnDl.Add(d);
                if (LevelB + LevelW < maxLev)
                {
                    maxLev = LevelB + LevelW;
                    ReturnDl.Clear();
                    ReturnDl.Add(d);
                }
            }
            if (ReturnDl.Count == 1)
                return ReturnDl[0];
            Random random = new Random();
            int m = random.Next(ReturnDl.Count - 1);
            return ReturnDl[m];
        }

        /// <summary>
        /// 多点防守（无禁手）
        /// </summary>
        /// <param name="board"></param>
        /// <param name="List"></param>
        /// <returns></returns>
        private PieceInfo MoreDefend(BoardInfo boardInfo, Board board)
        {
            List<PieceInfo> maxLT = boardInfo.MaxLt;
            PieceInfo ReturnCrossPoint = new PieceInfo();
            // 活三11时
            if (boardInfo.MaxLev == LevelEnum.LF_2)// 活三11时
            {
                ReturnCrossPoint = TryVCF(maxLT, board);
                return ReturnCrossPoint;
            }

            // 杀棋Lev < 7
            if ((int)boardInfo.MaxLev <= (int)LevelEnum.TrTr_6)
            {
                // 多个杀点时
                if (maxLT.Count > 1)
                    return maxLT[0];

                // 1子3杀时
                ReturnCrossPoint = maxLT[0];
                var info = boardInfo.GetPieceInfo(ReturnCrossPoint.X, ReturnCrossPoint.Y);
                string[] strNameD = new string[4];// 对方4个方向的棋形
                if (board.Step % 2 == 1)
                    strNameD = info.Names[ColourEnum.White];
                else
                    strNameD = info.Names[ColourEnum.White];
                List<int> ltstr = new List<int>();
                for (int i = 0; i < 4; i++)
                {
                    if (strNameD[i].Equals("h3")
                            || strNameD[i].Equals("t3")
                            || strNameD[i].Equals("m4"))
                        ltstr.Add(i);
                }
                if (ltstr.Count > 2)// 杀棋线路多于2个（三杀）
                    return ReturnCrossPoint;
            }

            // 添加多防守点各个测试
            int DColour = (board.Step) % 2;// 被守方棋色
            List<PieceInfo> MdArrayList = new List<PieceInfo>();
            MdArrayList = Defend(maxLT, DColour, boardInfo);// 获得多点防守点列
            foreach (PieceInfo d in MdArrayList)
                maxLT.Add(d);
            ReturnCrossPoint = TryVCF(maxLT, board);

            return ReturnCrossPoint;

        }
        /// <summary>
        /// LTCrossPoint里的点走后是否对方有胜？有返回没有返回GetBestCrossPoint(无禁手)
        /// </summary>
        /// <param name="LTCrossPoint"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        private PieceInfo TryVCF(List<PieceInfo> LTCrossPoint, Board board)
        {
            List<PieceInfo> returnDLt = new List<PieceInfo>();
            int maxNu = 0;
            foreach (PieceInfo d in LTCrossPoint)
            {
                Board Tboard = board.ChangeBoard(d.X, d.Y,
                        board.Step + 1);
                BoardInfo Bi = new BoardInfo(Tboard);

                StateEnum f = Bi.Result;
                int nu = (int)f;
                if (nu == maxNu)
                {
                    returnDLt.Add(d);
                }
                if (nu > maxNu)
                {
                    maxNu = nu;
                    returnDLt.Clear();
                    returnDLt.Add(d);
                }

                // if (Bi.Result == "A" || Bi.Result == "C") return d;
                // if (Bi.StrFlag == "C")
                // {
                // Piece Piece = VCF(Tboard, Bi.MaxLt);
                // if (Piece.StrFlag == "A") return d;
                // }
            }
            PieceInfo ReturnCrossPoint = GetBest(returnDLt);

            return ReturnCrossPoint;
        }
        /// <summary>
        /// 添加多点
        /// </summary>
        /// <param name="LTCrossPoint">原点列</param>
        /// <param name="player">对方棋色</param>
        /// <param name="board"></param>
        /// <returns>多点防守点列</returns>
        private List<PieceInfo> Defend(List<PieceInfo> LTCrossPoint, int player, BoardInfo board)
        {
            List<PieceInfo> NewLTCrossPoint = new List<PieceInfo>();
            foreach (PieceInfo d in LTCrossPoint)
            {
                int i = d.X;
                int j = d.Y;
                int mini = i - 4, maxi = i + 4;
                int minj = j - 4, maxj = j + 4;
                // line 0
                for (int x = mini; x < maxi; x++)
                {
                    if (x < 1 || x > 15 || x == i)
                        continue;
                    PieceInfo info = board.GetPieceInfo(x, j);
                    Boolean isd = IsDefendCrossPoint(info, player, 0);

                    if (isd)
                    {
                        NewLTCrossPoint.Add(info);
                    }
                }

                // line 1
                for (int y = minj; y < maxj; y++)
                {
                    if (y < 1 || y > 15 || y == j)
                        continue;
                    PieceInfo info = board.GetPieceInfo(i, y);
                    Boolean isd = IsDefendCrossPoint(info, player, 1);

                    if (isd)
                    {
                        NewLTCrossPoint.Add(info);
                    }
                }

                // line 2
                int ly = minj;
                for (int x = mini; x < maxi; x++)
                {
                    if (x < 1 || x > 15 || x == i)
                    {
                        ly++;
                        continue;
                    }
                    if (ly < 1 || ly > 15 || ly == j)
                    {
                        ly++;
                        continue;
                    }
                    PieceInfo info = board.GetPieceInfo(x, ly);
                    Boolean isd = IsDefendCrossPoint(info, player, 2);

                    if (isd)
                    {
                        NewLTCrossPoint.Add(info);
                    }
                }
                ly++;

                // line 3
                int rx = maxi;
                for (int y = minj; y < maxj; y++)
                {
                    if (y < 1 || y > 15 || y == j)
                    {
                        rx--;
                        continue;
                    }
                    if (rx < 1 || rx > 15 || rx == i)
                    {
                        rx--;
                        continue;
                    }
                    PieceInfo info = board.GetPieceInfo(rx, y);
                    Boolean isd = IsDefendCrossPoint(info, player, 3);

                    if (isd)
                    {
                        NewLTCrossPoint.Add(info);
                    }
                }
                rx--;

            }
            return NewLTCrossPoint;
        }

        /**
         * 是否为扩充防守点
         * 
         * @param tmpCrossPointN
         *            防守点
         * @param player
         *            棋色
         * @param index
         *            方向
         * @return
         */
        private Boolean IsDefendCrossPoint(PieceInfo tmpCrossPointN, int player, int index)
        {

            //if (tmpCrossPointN.Colour != ColourEnum.Empty)
            //    return false;

            string lev = tmpCrossPointN.Names[ColourEnum.Black][index];
            if (player == 2)
                lev = tmpCrossPointN.Names[ColourEnum.White][index];
            if (lev == "m4" || lev == "h3" || lev == "t3")
                return true;
            else
                return false;
        }

        /**
         * 获得标志的级别
         * 
         * @param flag
         *            标志
         * @return
         */
        private int GetNuOfFlag(StateEnum flag)
        {
            int returnNu = 0;
            //if (flag == "D")
            //    returnNu = 5;
            //if (flag == "DC")
            //    returnNu = 4;
            //if (flag == "ED")
            //    returnNu = 3;
            //if (flag == "EA")
            //    returnNu = 2;
            //if (flag == "C")
            //    returnNu = 1;
            //if (flag == "A")
            //    returnNu = 0;
            return returnNu;
        }
        #endregion
    }
}
