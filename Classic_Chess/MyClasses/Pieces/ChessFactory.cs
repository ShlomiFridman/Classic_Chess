using System;
using System.Collections.Generic;
using System.Text;

namespace Classic_Chess.MyClasses.Pieces
{
    static class ChessFactory
    {

        public static ChessPiece createPiece(Type type, Color color, Coords pos)
        {
            switch (type)
            {
                case Type.King:
                    return new King(color, pos);
                case Type.Queen:
                    return new Queen(color, pos);
                case Type.Bishop:
                    return new Bishop(color, pos);
                case Type.Knight:
                    return new Knight(color, pos);
                case Type.Rook:
                    return new Rook(color, pos);
                case Type.Pawn:
                    return new Pawn(color, pos);
                default:
                    return null;
            }
        }

        public static ChessPiece createPiece(long data)
        {
            if (data == 0)
                return null;
            Type type = (Type)(data / 1000);
            Color color = (Color)(data / 100 %10);
            Coords pos = Coords.getFromSave(data%100);
            return createPiece(type, color, pos);
        }
    }
}
