using System;
using System.Collections.Generic;

namespace Classic_Chess.MyClasses.Pieces
{
    abstract class ChessPiece
    {
        public Type type;
        public Color color;
        public Coords pos;
        public List<Coords> movesOffsets;
        public List<Move> validMoves;


        protected ChessPiece(Type type, Color color, Coords pos)
        {
            this.type = type;
            this.color = color;
            this.pos = pos;
            this.validMoves = new List<Move>();
        }

        protected virtual List<Move> getMoves(Board board)
        {
            List<Move> moves = new List<Move>();
            this.movesOffsets.ForEach((offset) =>
            {
                if (this.checkOffset(offset))
                {
                    Coords after = this.getAfterPos(offset);
                    ChessPiece pieceAt = board.getPieceAt(after);
                    // add to moves if not an enemy
                    if (!isAlly(pieceAt)) // if empty or has enemy
                        moves.Add(new Move(this.pos, after, pieceAt));
                }
            });
            return moves;
        }

        public void updateMoves(Board board)
        {
            this.validMoves = this.getMoves(board);
        }

        public virtual Move getMoveAt(Board board, Coords offset)
        {
            if (!checkOffset(offset))
                return null;
            var after = getAfterPos(offset);
            var pieceAt = board.getPieceAt(after);
            return new Move(this.pos, after, pieceAt);
        }

        public virtual bool MoveTo(Coords newPos)
        {
            this.pos = newPos;
            return true;
        }

        /// <summary>
        /// return true if the move is in bounds, else return false
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected bool checkOffset(Coords offset)
        {
            Coords after = getAfterPos(offset);
            if (after.x < 0 || after.x > 7)
                return false;
            else if (after.y < 0 || after.y > 7)
                return false;
            return true;
        }

        protected virtual List<Coords> getInBoundsOffsets()
        {
            var inBounds = new List<Coords>();
            this.movesOffsets.ForEach((offset) =>
            {
                if (checkOffset(offset))
                    inBounds.Add(offset);
            });
            return inBounds;
        }

        protected Coords getAfterPos(Coords offset)
        {
            return new Coords(this.pos.x + offset.x, this.pos.y + offset.y);
        }

        protected Coords getAfterPos(int x, int y)
        {
            return new Coords(this.pos.x + x, this.pos.y + y);
        }

        protected bool isEnemy(ChessPiece other)
        {
            return (other != null) ? other.color != this.color : false;
        }

        protected bool isAlly(ChessPiece other)
        {
            return (other != null) ? other.color == this.color : false;
        }
    }

    enum Type
    {
        King,Queen,Rook,Bishop,Knight,Pawn
    }

    enum Color
    {
        White,Black
    }

}
