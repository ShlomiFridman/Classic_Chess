using System;
using System.Collections.Generic;
using System.Text;

namespace Classic_Chess.MyClasses.Pieces
{
    class Move
    {
        public Coords before, after;
        public ChessPiece enemy;

        public Move(Coords before, Coords after, ChessPiece enemy)
        {
            this.before = before;
            this.after = after;
            this.enemy = enemy;
        }
    }
}
