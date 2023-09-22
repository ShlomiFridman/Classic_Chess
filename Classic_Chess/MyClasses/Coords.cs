using System;
using System.Collections.Generic;
using System.Text;

namespace Classic_Chess.MyClasses.Pieces
{
    struct Coords
    {
        public int x;
        public int y;

        public Coords(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is Coords coords &&
                   x == coords.x &&
                   y == coords.y;
        }

        public override string ToString()
        {
            return $"({x},{y})";
        }
    }
}
