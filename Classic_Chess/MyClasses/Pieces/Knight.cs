using System;
using System.Collections.Generic;
using System.Text;

namespace Classic_Chess.MyClasses.Pieces
{
    class Knight : ChessPiece
    {
        public Knight(Color color, Coords pos) : base(Type.Knight, color, pos)
        {
            this.movesOffsets = new List<Coords>();
            this.movesOffsets.AddRange(new List<Coords>
            {
                new Coords(2,1),
                new Coords(2,-1),
                new Coords(1,2),
                new Coords(1,-2),
                new Coords(-2,1),
                new Coords(-2,-1),
                new Coords(-1,2),
                new Coords(-1,-2),
            });
        }
    }
}
