using System;
using System.Collections.Generic;
using System.Text;

namespace Classic_Chess.MyClasses.Pieces
{
    class Pawn : ChessPiece
    {

        public Pawn(Color color, Coords pos) : base(Type.Pawn, color, pos)
        {
            int isBlack = (color == Color.White)? -1:1;
            this.movesOffsets = new List<Coords>();
            this.movesOffsets.AddRange(new List<Coords>
            {
                new Coords(0,1*isBlack),
                new Coords(-1*isBlack,1*isBlack),
                new Coords(1*isBlack,1*isBlack),
                new Coords(0,2*isBlack)
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
                    moves.Add(new Move(this.pos, after, null));
                    // check second move option if the pawn didn't moved from his starting position
                    if (this.movesOffsets.Count == 4 && checkOffset(this.movesOffsets[3]))
                    {
                        after = getAfterPos(this.movesOffsets[3]);
                        pieceAt = board.getPieceAt(after);
                        if (pieceAt == null)
                            moves.Add(new Move(this.pos, after, null));
                    }
                }
            }
            if (checkOffset(this.movesOffsets[1]))
            {
                after = getAfterPos(this.movesOffsets[1]);
                pieceAt = board.getPieceAt(after);
                if (isEnemy(pieceAt))
                    moves.Add(new Move(this.pos, after, pieceAt));
            }
            if (checkOffset(this.movesOffsets[2]))
            {
                after = getAfterPos(this.movesOffsets[2]);
                pieceAt = board.getPieceAt(after);
                if (isEnemy(pieceAt))
                    moves.Add(new Move(this.pos, after, pieceAt));
            }
            return moves;
        }

        public override bool MoveTo(Coords newPos)
        {
            // remove the double move option after the first move
            if (this.movesOffsets.Count == 4)
                this.movesOffsets.RemoveAt(3);
            return base.MoveTo(newPos);
        }
    }
}
