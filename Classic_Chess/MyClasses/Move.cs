using System;
using System.Collections.Generic;
using System.Text;

namespace Classic_Chess.MyClasses.Pieces
{
    class Move
    {
        public Coords before, after;
        public ChessPiece piece, pieceAt;

        public Move(Coords before, Coords after, ChessPiece piece, ChessPiece pieceAt)
        {
            this.before = before;
            this.after = after;
            this.piece = piece;
            this.pieceAt = pieceAt;
        }

        // format BBAAPPPPEEEE - (BB - before, AA - after, PPPP - piece, EEEE - enemy)
        public long getSaveValue()
        {
            long coords = before.getSaveValue() * 100 + after.getSaveValue();
            long pieceVal = piece.getSaveValue();
            long emVal = (pieceAt == null) ? 0 : pieceAt.getSaveValue();
            return (coords * 10000 + pieceVal) * 10000 + emVal;
        }

        /*
        public Move getFlipped()
        {
            var fm = new Move(before, after, piece, pieceAt);
            fm.before.y = (7 - fm.before.y) % 8;
            fm.after.y = (7 - fm.after.y) % 8;
            return fm;
        }
        */

        public static Move getFromSave(long data)
        {
            long emVal, pieceVal, coords;
            emVal = data % 10000;
            data /= 10000;
            pieceVal = data % 10000;
            coords = data / 10000;
            return new Move(Coords.getFromSave(coords / 100), Coords.getFromSave(coords % 100), ChessFactory.createPiece(pieceVal), ChessFactory.createPiece(emVal));
        }

    }
}
