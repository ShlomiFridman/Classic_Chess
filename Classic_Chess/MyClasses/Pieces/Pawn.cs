using System;
using System.Collections.Generic;
using System.Text;

namespace Classic_Chess.MyClasses.Pieces
{
    class Pawn : ChessPiece
    {

        private int colorMul;

        public Pawn(Color color, Coords pos) : base(Type.Pawn, color, pos)
        {
            colorMul = (color == Color.White)? -1:1;
            this.movesOffsets = new List<Coords>();
            this.movesOffsets.AddRange(new List<Coords>
            {
                new Coords(0,1*colorMul),
                new Coords(-1*colorMul,1*colorMul),
                new Coords(1*colorMul,1*colorMul),
            });
        }

        protected override List<Move> getMoves(Board board)
        {
            var moves = new List<Move>();
            ChessPiece pieceAt;
            Coords after;
            if (checkOffset(this.movesOffsets[0]))
            {
                after = getAfterPos(this.movesOffsets[0]);
                pieceAt = board.getPieceAt(after);
                // if the space is empty, than the move is legal
                if (pieceAt == null)
                {
                    moves.Add(new Move(this.pos, after, this, null));
                    // check second move option if the pawn didn't moved from his starting position
                    if ((color == Color.Black && pos.y == 1) || (color == Color.White && pos.y == 6))
                    {
                        after = getAfterPos(new Coords(0, 2 * colorMul));
                        pieceAt = board.getPieceAt(after);
                        if (pieceAt == null)
                            moves.Add(new Move(this.pos, after, this, null));
                    }
                }
            }
            if (checkOffset(this.movesOffsets[1]))
            {
                after = getAfterPos(this.movesOffsets[1]);
                pieceAt = board.getPieceAt(after);
                if (isEnemy(pieceAt))
                    moves.Add(new Move(this.pos, after, this, pieceAt));
            }
            if (checkOffset(this.movesOffsets[2]))
            {
                after = getAfterPos(this.movesOffsets[2]);
                pieceAt = board.getPieceAt(after);
                if (isEnemy(pieceAt))
                    moves.Add(new Move(this.pos, after, this, pieceAt));
            }
            return moves;
        }
    }
}
