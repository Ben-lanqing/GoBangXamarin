using System;
using System.Collections.Generic;
using System.Text;

namespace GoBangCL.Standard.Model
{
    public class PieceInfo
    {
        public int X { set; get; }
        public int Y { set; get; }

        public Dictionary<ColourEnum, LevelEnum> Levels { set; get; }
        public Dictionary<ColourEnum, string[]> Names { set; get; }
        public ColourEnum[][] PLine { set; get; }

        public PieceInfo()
        {
            Levels = new Dictionary<ColourEnum, LevelEnum>();
            Names = new Dictionary<ColourEnum, string[]>();
            PLine = new ColourEnum[4][];
            Levels.Add(ColourEnum.Black, LevelEnum.E_99);
            Levels.Add(ColourEnum.White, LevelEnum.E_99);
            Names.Add(ColourEnum.Black, new string[4] { "", "", "", "" });
            Names.Add(ColourEnum.White, new string[4] { "", "", "", "" });
        }
    }
}
