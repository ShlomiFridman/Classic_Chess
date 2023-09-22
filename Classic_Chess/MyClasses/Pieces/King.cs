using System;
using System.Collections.Generic;
using System.Text;

namespace Classic_Chess.MyClasses.Pieces
{
    class King : ChessPiece
    {

        public King(Color color, Coords pos) : base(Type.King, color, pos)
        {
            this.movesOffsets = new List<Coords>();
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                    if (i != 0 || j != 0)
                        this.movesOffsets.Add(new Coords(i, j));
        }
    }
}
