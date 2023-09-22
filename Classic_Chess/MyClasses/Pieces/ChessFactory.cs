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
    }
}
