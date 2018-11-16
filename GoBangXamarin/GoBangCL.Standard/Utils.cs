using GoBangCL.Standard.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoBangCL.Standard
{
    public static class Utils
    {
        public static string CurrentBoardLogStr(List<Board> boardList, int currentStep)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                if (boardList == null || boardList.Count == 0)
                {
                    return "Empty BoardList";
                }
                sb.AppendLine($"BoardList count:{boardList.Count} CurrentStep:{currentStep} ");
                var CurrentBoard = boardList[currentStep];
                for (int i = 0; i < currentStep; i++)
                {
                    var p = CurrentBoard.DownPieces[i];
                    sb.AppendLine($"Step:({i + 1}) X:{p.X} Y:{p.Y} C:{p.Colour}");
                }
                return sb.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
