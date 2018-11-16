using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoBangCL.Standard.Model
{
    public class BoardInfo
    {
        #region public
        public StateEnum Result { set; get; }
        /// <summary>
        /// 全局最大级别
        /// </summary>
        public LevelEnum MaxLev { set; get; }
        /// <summary>
        /// 最优点
        /// </summary>
        public List<PieceInfo> MaxLt { set; get; }
        public Board CurrentBoard { set; get; }
        #endregion

        #region private
        private int MaxLooplev = 10;
        private int currentLooplev = 0;
        private List<PieceInfo> relatedPieceInfos = new List<PieceInfo>();

        private List<PieceInfo> m4LtS = new List<PieceInfo>();
        private List<PieceInfo> m4LtD = new List<PieceInfo>();
        /// <summary>
        /// 跳转深度
        /// </summary>
        private LevelEnum highLevB = LevelEnum.Loss_99999;
        private LevelEnum highLevW = LevelEnum.Loss_99999;
        /// <summary>
        /// 己方最大级别
        /// </summary>
        private LevelEnum highLevS = LevelEnum.Loss_99999;
        /// <summary>
        /// 对方最大级别
        /// </summary>
        private LevelEnum highLevD = LevelEnum.Loss_99999;
        /// <summary>
        /// 己方点列
        /// </summary>
        private List<PieceInfo> highLtS = new List<PieceInfo>();
        /// <summary>
        /// 对方点列
        /// </summary>
        private List<PieceInfo> highLtD = new List<PieceInfo>();
        private List<PieceInfo> highLtB = new List<PieceInfo>();
        private List<PieceInfo> highLtW = new List<PieceInfo>();
        private List<PieceInfo> m4LtB = new List<PieceInfo>();
        private List<PieceInfo> m4LtW = new List<PieceInfo>();

        private List<PieceInfo> forbiddenLt = new List<PieceInfo>();
        #endregion

        public BoardInfo(Board currentBoard, int looplev = 0)
        {
            Intit();
            currentLooplev = looplev;
            CurrentBoard = currentBoard.Clone();
            BasicAnalyze();
        }

        private void Intit()
        {
            MaxLev = LevelEnum.Loss_99999;
            MaxLt = new List<PieceInfo>();
            Result = StateEnum.D;

        }


        #region Analyze
        private void BasicAnalyze()
        {
            try
            {
                //foreach (var relatedPiece in CurrentBoard.RelatedPieces)
                Parallel.ForEach(CurrentBoard.RelatedPieces, relatedPiece =>
                {
                    try
                    {
                        if (relatedPiece.Colour != ColourEnum.Empty) return;//非空点跳过
                        var info = GetPieceInfo(relatedPiece.X, relatedPiece.Y);
                        relatedPieceInfos.Add(info);
                        #region highLtB、highLevB
                        if (info.Levels[ColourEnum.Black] == highLevB)
                        {
                            highLtB.Add(info);
                        }
                        else if ((int)info.Levels[ColourEnum.Black] < (int)highLevB)
                        {
                            highLevB = info.Levels[ColourEnum.Black];
                            highLtB.Clear();
                            highLtB.Add(info);
                        }
                        #endregion
                        #region highLtW、highLevW
                        if (info.Levels[ColourEnum.White] == highLevW)
                        {
                            highLtW.Add(info);
                        }
                        else if ((int)info.Levels[ColourEnum.White] < (int)highLevW)
                        {
                            highLevW = info.Levels[ColourEnum.White];
                            highLtW.Clear();
                            highLtW.Add(info);
                        }
                        #endregion
                        #region m4LtB、m4LtW、forbiddenLt
                        if (info.Names[ColourEnum.Black].Contains("m4"))
                            m4LtB.Add(info);
                        if (info.Names[ColourEnum.White].Contains("m4"))
                            m4LtW.Add(info);
                        if ((int)info.Levels[ColourEnum.Black] >= 9990)
                            forbiddenLt.Add(info);

                        #endregion
                    }
                    catch (Exception e)
                    {
                    }
                });

                int nextStep = CurrentBoard.Step + 1;
                ColourEnum nextColour = nextStep % 2 == 0 ? ColourEnum.White : ColourEnum.Black;
                #region 转换成攻防信息
                if (nextColour == ColourEnum.Black)
                {
                    highLevS = highLevB;
                    highLtS = highLtB;
                    m4LtS = m4LtB;
                    highLevD = highLevW;
                    highLtD = highLtW;
                    m4LtD = m4LtW;
                }
                else
                {
                    highLevS = highLevW;
                    highLtS = highLtW;
                    m4LtS = m4LtW;
                    highLevD = highLevB;
                    highLtD = highLtB;
                    m4LtD = m4LtB;
                }
                #endregion
                Result = Analyze(highLevS, highLevD, highLtS, highLtD, m4LtS.Count, m4LtD.Count);
            }
            catch (Exception e)
            {

            }
        }
        /// <summary>
        /// 深度分析（VCF）
        /// </summary>
        public StateEnum DeepAnalyze()
        {
            #region 待VCF时，判断己VCF是否成立
            if (Result == StateEnum.WVCF)
            {
                PieceInfo VCFPoint = TryGetVCFPiece(CurrentBoard, m4LtS, CurrentBoard.Step + 1);
                //VCF成立返回VCF点
                if (VCFPoint != null)
                {
                    MaxLev = LevelEnum.VCF_5;
                    MaxLt.Add(VCFPoint);
                    Result = StateEnum.Win;
                    return Result;
                }//无VCF继续分析
                else
                {
                    Result = Analyze(highLevS, highLevD, highLtS, highLtD, m4LtS.Count, m4LtD.Count, ProcessEnum.己无VCF);
                }
            }
            #endregion
            #region 待敌VCF时，判断敌VCF是否成立
            if (Result == StateEnum.WAVCF)
            {
                PieceInfo VCFPoint_D = TryGetVCFPiece(CurrentBoard, m4LtD, CurrentBoard.Step + 2);
                //敌VCF成立返回VCF点
                if (VCFPoint_D != null)
                {
                    MaxLev = LevelEnum.VCF_5;
                    MaxLt.Add(VCFPoint_D);
                    Result = StateEnum.Loss;
                    return Result;
                }//无VCF继续分析
                else
                {
                    Result = Analyze(highLevS, highLevD, highLtS, highLtD, m4LtS.Count, m4LtD.Count, ProcessEnum.敌无VCF);
                }
            }
            #endregion
            #region 双方无VCF
            if (Result == StateEnum.Win || Result == StateEnum.A)
            {
                MaxLev = highLevS;
                MaxLt = highLtS;
            }
            else if (Result == StateEnum.Loss || Result == StateEnum.D)
            {
                MaxLev = highLevD;
                MaxLt = highLtD;
            }
            else
            {
                MaxLev = highLevS;
                MaxLt = highLtS;
            }
            return Result;
            #endregion
        }

        private StateEnum Analyze(LevelEnum highLevS, LevelEnum highLevD, List<PieceInfo> highLtS, List<PieceInfo> highLtD, int LtS, int LtD, ProcessEnum flag = ProcessEnum.起始)
        {
            #region 起始全流程
            if (flag == ProcessEnum.起始)
            {
                if (highLevS == LevelEnum.Five_1)
                {
                    MaxLev = highLevS;
                    MaxLt = highLtS;
                    return StateEnum.Win;
                }
                if (highLevD == LevelEnum.Five_1)
                {
                    MaxLev = highLevD;
                    MaxLt = highLtD;
                    return StateEnum.Loss;
                }
                if (highLevS == LevelEnum.LF_2 || highLevS == LevelEnum.FF_3 || highLevS == LevelEnum.FTr_4)
                {
                    MaxLev = highLevS;
                    MaxLt = highLtS;
                    return StateEnum.Win;
                }
                if (LtS != 0)
                    return StateEnum.WVCF;
            }
            #endregion
            #region 己无VCF
            if (flag == ProcessEnum.起始 || flag == ProcessEnum.己无VCF)
            {
                if (highLevD == LevelEnum.LF_2 || highLevD == LevelEnum.FF_3 || highLevD == LevelEnum.FTr_4)
                {
                    MaxLev = highLevD;
                    MaxLt = highLtD;
                    return StateEnum.Loss;
                }
                if (LtD != 0)
                    return StateEnum.WAVCF;
            }
            #endregion
            #region 己无、敌无VCF
            if (flag == ProcessEnum.起始 || flag == ProcessEnum.己无VCF || flag == ProcessEnum.敌无VCF)
            {
                if (highLevS == LevelEnum.TrTr_6)
                {
                    MaxLev = highLevS;
                    MaxLt = highLtS;
                    return StateEnum.Win;
                }
                if (highLevD == LevelEnum.TrTr_6)
                {
                    MaxLev = highLevD;
                    MaxLt = highLtD;
                    return StateEnum.Loss;
                }
                if (highLevS <= highLevD)
                {
                    if (highLevS == LevelEnum.SF_13 && highLevD == LevelEnum.SF_13)
                    {
                        MaxLev = highLevD;
                        MaxLt = highLtD;
                        return StateEnum.D;
                    }
                    else
                    {
                        MaxLev = highLevS;
                        MaxLt = highLtS;
                        return StateEnum.A;
                    }
                }
            }
            #endregion
            MaxLev = highLevD;
            MaxLt = highLtD;
            return StateEnum.D;
        }
        private PieceInfo TryGetVCFPiece(Board board, List<PieceInfo> m4Lt, int step)
        {
            PieceInfo returnPoint = null;
            if (currentLooplev < MaxLooplev)
            {
                Parallel.ForEach(m4Lt, (m4Point, ParallelLoopState) =>
                {
                    if (ChackIsVCFPiece(board, m4Point, step))
                    {
                        returnPoint = m4Point;
                        ParallelLoopState.Stop();
                        return;
                    }
                });
            }
            return returnPoint;
        }
        private bool ChackIsVCFPiece(Board board, PieceInfo m4Point, int step)
        {
            //冲4点落子棋盘
            Board V1Board = board.ChangeBoard(m4Point.X, m4Point.Y, step);
            BoardInfo V1BoardInfo = new BoardInfo(V1Board, currentLooplev + 1);
            //冲4必防点
            PieceInfo V2Piece = V1BoardInfo.MaxLt[0];
            //冲4防守点落子棋盘
            Board V2Board = V1Board.ChangeBoard(V2Piece.X, V2Piece.Y, step + 1);
            BoardInfo V2BoardInfo = new BoardInfo(V2Board, currentLooplev + 1);
            StateEnum flag = V2BoardInfo.GetVCFResult();
            return flag == StateEnum.Win;
        }

        private StateEnum GetVCFResult()
        {
            if (this.Result == StateEnum.Win)
            {
                return StateEnum.Win;
            }
            if (this.Result != StateEnum.WVCF)
            {
                return StateEnum.NVCF;
            }

            PieceInfo VCFCrossPoint = TryGetVCFPiece(this.CurrentBoard, this.m4LtS, this.CurrentBoard.Step + 1);
            if (VCFCrossPoint == null)
            {
                return StateEnum.NVCF;
            }
            else
            {
                return StateEnum.Win;
            }
        }
        #endregion

        public PieceInfo GetPieceInfo(int x, int y)
        {
            PieceInfo info = new PieceInfo();
            for (int i = 0; i < 4; i++)
            {
                info.X = x; info.Y = y;
                info.PLine[i] = CurrentBoard.GetLinePieces(x, y, i);
                info.Names[ColourEnum.Black][i] = GetName(info.PLine[i], ColourEnum.Black, ColourEnum.Empty);
                info.Names[ColourEnum.White][i] = GetName(info.PLine[i], ColourEnum.White, ColourEnum.Empty);
            }
            info.Levels[ColourEnum.Black] = GetLevByName(info.Names[ColourEnum.Black]);
            info.Levels[ColourEnum.White] = GetLevByName(info.Names[ColourEnum.White]);

            return info;
        }
        private string GetName(ColourEnum[] line, ColourEnum colour, ColourEnum emp)
        {
            int Bs = 4, Bd = -1, Es = 4, Ed = 9, n = 1;
            string name = "";
            //char[] line = str.ToCharArray();// 字符串拆成字符组
            #region 遍历前后8点获得棋型信息
            for (int i = 0; i <= 8; i++)
            {
                if (line[i] == emp || i == 4)
                    continue;// 如果空点或中点下一个
                if (line[i] != colour)// 为异色点时
                {
                    if (i < 4)// 中点之前
                    {
                        Bd = i;// Bd为最后一个异色点坐标
                        if (Bs < i)// 如果之前有同色点
                        {
                            Bs = 4;
                            n = 1;
                        }// 还原标志值
                    }
                    else
                    {
                        Ed = i;
                        break;
                    }// 中点之后出现异色点跳出循环
                }
                else// 为同色点时
                {
                    if (i < 4)// 中点之前
                    {
                        if (Bs > i)
                            Bs = i;// Bs取第一个同色点坐标
                    }
                    else
                    {
                        Es = i;
                    }// 中点之后取最后同色点坐标
                    n++;// n为同色计数器
                }
            }
            #endregion
            #region 根据棋型信息返回名称
            #region if (Es - Bs > 4) 如果是长连
            if (Es - Bs > 4)// 长连
            {
                int Scor = 0;// 分数标志
                for (int i = Bs; i <= Es - 4; i++)// 长连中各个5个棋形组合
                {
                    int TempScor = 10;
                    int bs = 0;
                    int es = 0;// 临时分数、起始点、中止点
                    for (int j = 0; j <= 4; j++)
                    {
                        if (line[i + j] != emp)
                        {
                            TempScor = TempScor + 10;// 有一个点+10分
                            es = i + j; // 设中止点
                            if (bs == 0)
                                bs = i + j;// 设起始点
                            if (bs - Bd == 1 || es + 1 == Ed)
                                TempScor = TempScor - 5;// 为“m”时-5分
                            if (bs == 0 || es == 8)
                                TempScor = TempScor - 5;
                        }
                    }
                    if (TempScor > Scor)
                        Scor = TempScor;// 取最大的分值
                }
                if (Scor < 20)
                    return "";
                switch (Scor)
                {
                    case 20:
                        name = "h2";
                        break;
                    case 25:
                        name = "m3";
                        break;
                    case 30:
                        name = "h3";
                        break;
                    case 35:
                        name = "m4";
                        break;
                    case 40:
                        name = "h4";
                        break;
                    case 45:
                        name = "h5";
                        break;
                    case 50:
                        name = "h5";
                        break;
                    default:
                        break;
                }
                return name;
            }
            #endregion
            if (Ed - Bd < 6)
                return name;// 异色起止点距离小于6时不成棋形返回“”
            if (Ed - Bd == 6)
                return "m" + n;// 异色起止点距离为6时“M”
            if (Bs - Bd == 1 || Es + 1 == Ed)
                return "m" + n;// 同色异色有相邻时“M”
            if (Bs == 0 || Es == 8)// 同色在起点、中点或中点、止点时“M”
            {
                if (n == 2)
                    return "";
                return "m" + n;
            }
            if (n == 2)
            {
                if (Es - Bs >= n + 1)
                    return "t" + n;
                return "h" + n;
            }
            if (Es - Bs >= n)
                return "t" + n;
            return "h" + n;
            #endregion
        }

        private LevelEnum GetLevByName(string[] StrName)
        {
            int V = 0, IV = 0, IVp = 0, III = 0, IIIp = 0, II = 0, IIp = 0, IIAll = 0;
            #region 遍历所有方向棋型名称获取信息
            foreach (string i in StrName)
            {
                if (i.Equals("L"))//长连
                    return LevelEnum.LL_9999;
                if (i.Equals("h5") || i.Equals("m5"))
                    V = V + 1;
                if (i.Equals("h4"))
                    IV = IV + 1;
                if (i.Equals("m4"))
                    IVp = IVp + 1;
                if (i.Equals("h3") || i.Equals("t3"))
                    III = III + 1;
                if (i.Equals("m3"))
                    IIIp = IIIp + 1;
                if (i.Equals("h2"))
                    II = II + 1;
                if (i.Equals("t2"))
                    IIp = IIp + 1;
            }
            #endregion
            #region 根据棋型返回级别值
            IIAll = II + IIp / 2;
            if (V > 0)
                return LevelEnum.Five_1; // 5 xxxxx
            if (IV > 0)
                return LevelEnum.LF_2; // 己活4 对方活3 xxx
            if (IVp > 1)
                return LevelEnum.FF_3; // 4-4，4-4-4 xx
            if (IVp == 1 && III > 0)
                return LevelEnum.FTr_4; // 4-3，4-3-3 xx
            if (III > 1)
                return LevelEnum.TrTr_6; // 3-3，3-3-3

            // 422,4222 (2||2.5) 322,3222 (2||2.5)
            if (IVp == 1 && IIAll >= 2)
                return LevelEnum.FToTo_8;
            if (IVp == 1 && IIIp >= 2)
                return LevelEnum.FToTo_8;
            if (IVp == 1 && IIIp == 1 && IIAll == 1)
                return LevelEnum.FToTo_8;
            if (III == 1 && IIAll >= 2)
                return LevelEnum.FToTo_8;
            if (III == 1 && IIIp >= 2)
                return LevelEnum.FToTo_8;
            if (III == 1 && IIIp == 1 && IIAll == 1)
                return LevelEnum.FToTo_8;
            // 2222,222 (2||2.5)
            if (IIAll >= 3)
                return LevelEnum.ToToTo_9;
            if (IIIp >= 3)
                return LevelEnum.ToToTo_9;
            if (IIAll == 2 && IIIp >= 1)
                return LevelEnum.ToToTo_9;
            if (IIIp == 2 && IIAll >= 1)
                return LevelEnum.ToToTo_9;
            // 42,42.5 32,32.5
            if (IVp == 1 && IIAll == 1)
                return LevelEnum.FTo_10;
            if (IVp == 1 && IIIp == 1)
                return LevelEnum.FTo_10;
            if (III == 1 && IIAll == 1)
                return LevelEnum.FTo_10;
            if (III == 1 && IIIp == 1)
                return LevelEnum.FTo_10;
            // 3
            if (III == 1)
                return LevelEnum.LT_11;
            // 22, 2.5 2.5
            if (IIAll == 2)
                return LevelEnum.ToTo_12;
            if (IIIp == 2)
                return LevelEnum.ToTo_12;
            if (IIAll == 1 && IIIp == 1)
                return LevelEnum.ToTo_12;
            // p4
            if (IVp == 1)
                return LevelEnum.SF_13;

            if (IIAll == 1)
                return LevelEnum.STrSTo_14;
            if (IIIp == 1)
                return LevelEnum.STrSTo_14;
            return LevelEnum.E_99;
            #endregion
        }

    }
}
