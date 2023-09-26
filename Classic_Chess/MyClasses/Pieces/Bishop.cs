using System;
using System.Collections.Generic;
using System.Text;

namespace Classic_Chess.MyClasses.Pieces
{
    class Bishop : ChessPiece
    {
        public Bishop(Color color, Coords pos) : base(Type.Bishop, color, pos)
        {
            this.movesOffsets = new List<Coords>();
            for (int i = 1; i <= 7; i++)
            {
                this.movesOffsets.AddRange(new List<Coords>()
                {
                    new Coords(i,i),
                    new Coords(-i,i),
                    new Coords(i,-i),
                    new Coords(-i,-i)
                });
            }
        }

        protected override List<Move> getMoves(Board board)
        {
            var moves = new List<Move>();
            Move move;
            bool[] flags = { true, true, true, true };  // flags of { rightUp, leftUp, rightDown, leftDown }
            for (int i = 1; i <= 7; i++)
            {
                Coords[] offsets = { new Coords(i, i), new Coords(-i, i), new Coords(i, -i), new Coords(-i, -i) };
                for (int j = 0; j < offsets.Length; j++)
                {
                    if (!checkOffset(offsets[j]) || !flags[j])
                        continue;
                    move = getMoveAt(board, offsets[j]);
                    moves.Add(move);
                    // if the space not empty, end sequence
                    if (move.pieceAt != null)
                        flags[j] = false;
                }
            }
            return moves;
        }
    }
}
