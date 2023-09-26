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

        public Coords(long x, long y)
        {
            this.x = (int) x;
            this.y = (int) y;
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

        // format XY
        public long getSaveValue()
        {
            return x * 10 + y;
        }

        public static Coords getFromSave(long data)
        {
            return new Coords(data / 10, data % 10);
        }
    }
}
