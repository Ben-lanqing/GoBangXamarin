using System;
using System.Collections.Generic;
using System.Text;

namespace GoBangCL.Standard.Model
{
    public class Piece
    {
        /// <summary>
        /// 点横向坐标（左0-右14）
        /// </summary>
        public int X { set; get; }
        /// <summary>
        /// 点纵向坐标（上0-下14）
        /// </summary>
        public int Y { set; get; }
        /// <summary>
        /// 1黑，2白, 空点0
        /// </summary>
        public int Step { set; get; }
        public ColourEnum Colour { set; get; }

        public string Title
        {
            get
            {
                return $"Step:{Step} {X},{Y} C:{Colour.ToString()}";
            }
        }


        public Piece()
        {
            X = 0;
            Y = 0;
            Step = 0;
            Colour = ColourEnum.Empty;
            //Title = "X:0 Y:0 Colour:Empty";
        }
        public Piece(int x, int y, ColourEnum colour,int step=0)
        {
            X = x;
            Y = y;
            Step = step;

            Colour = colour;
            //Title = $"X:{X} Y:{Y} Colour:{Colour.ToString()}";

        }

        public Piece Clone()
        {
            var item = new Piece();
            item.X = X;
            item.Y = Y;
            item.Step = Step;
            item.Colour = Colour;
            return item;
        }
        public static List<Piece> ClonePieceList(List<Piece> source)
        {
            var list = new List<Piece>();
            if (source == null || source.Count == 0) return list;
            foreach (var item in source)
            {
                list.Add(item.Clone());
            }
            return list;
        }
    }
}
