using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleGame
{
    class Piece : asd.TextureObject2D
    {
        public static float AdjustWidth { get; private set; }//大きさ調整
        public Piece(asd.Vector2DF pos)
        {
            AdjustWidth = 0.975f;
            Position = pos;
        }

        public void SwapPos(Piece p)
        {
            var tempPos = Position;
            Position = p.Position;
            p.Position = tempPos;
        }
    }
}
