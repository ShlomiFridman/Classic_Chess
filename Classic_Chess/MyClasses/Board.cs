using Classic_Chess.MyClasses.Pieces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Classic_Chess.MyClasses
{
    class Board
    {

        private Dictionary<Coords,ChessPiece> activePieces;
        private List<ChessPiece> blackPieces;
        private List<ChessPiece> whitePieces;
        private List<ChessPiece> graveyard;
        private Stack<Move> history, future;
        public Color turn;


        public Board()
        {
            // init properties
            activePieces = new Dictionary<Coords, ChessPiece>();
            blackPieces = new List<ChessPiece>();
            whitePieces = new List<ChessPiece>();
            graveyard = new List<ChessPiece>();
            history = new Stack<Move>();
            future = new Stack<Move>();
        }

        public static Board getSetBoard()
        {
            var board = new Board();
            board.resetBoard();
            return board;
        }

        public ChessPiece getPieceAt(Coords pos)
        {
            if (this.activePieces.ContainsKey(pos))
                return this.activePieces[pos];
            else
                return null;
        }

        public bool makeMove(Move move, bool isRedo)
        {
            var piece = move.piece;
            // if hasEnemy is true, remove enemy from active
            if (move.enemy != null)
                this.removeFromActive(move.enemy);
            // move the piece to the new position
            this.movePiece(piece, move.after);
            // add move to history
            history.Push(move);
            // clear future, if not a redo
            if (!isRedo)
                future.Clear();
            // get updated moves
            updateMoves();
            // made the move, return true
            return true;
        }

        private bool movePiece(ChessPiece piece, Coords newPos)
        {
            // remove from old position
            this.removeFromActive(piece);
            // update position
            piece.MoveTo(newPos);
            // readd to active
            this.addToActive(piece);
            return true;
        }

        public Move undo()
        {
            // if the history is empty, return false
            if (history.Count == 0)
                return null;
            // get move
            var move = history.Pop();
            var piece = move.piece;
            var pieceAt = getPieceAt(move.after);
            // if its not the same piece, happens when a pawn is promoted
            if (piece != pieceAt)
            {
                // change back
                removeFromActive(pieceAt);
                graveyard.Remove(pieceAt);
                addToActive(piece);
            }
            // move the piece to the old position
            this.movePiece(piece, move.before);
            // if had an enemy, revive him
            if (move.enemy != null)
                this.addToActive(move.enemy);
            // push move to future
            future.Push(move);
            // update moves
            this.updateMoves();
            // undone successfully, return the move
            return move;
        }

        public bool haveUndo()
        {
            // if there are any moves to undo, return true (else false)
            return this.history.Count != 0;
        }

        public Move getRedo()
        {
            // if future is empty, return false
            if (future.Count == 0)
                return null;
            // return redo move
            return future.Pop(); ;
        }

        public bool haveRedo()
        {
            // if there are any moves to undo, return true (else false)
            return this.future.Count != 0;
        }

        public void resetBoard()
        {
            int i;
            Coords pos;
            Pieces.Type[] arr = { Pieces.Type.Rook, Pieces.Type.Knight, Pieces.Type.Bishop };
            // clear lists/dictionaries
            activePieces.Clear();
            blackPieces.Clear();
            whitePieces.Clear();
            graveyard.Clear();
            history.Clear();
            future.Clear();
            // add the pieces to the board
            // the rooks, knights, and bishops
            for (i = 0; i < arr.Length; i++)
            {
                pos = new Coords(i, 0);
                this.addToActive(ChessFactory.createPiece(arr[i], Color.Black, pos));
                pos = new Coords(7-i, 0);
                this.addToActive(ChessFactory.createPiece(arr[i], Color.Black, pos));
                pos = new Coords(i, 7);
                this.addToActive(ChessFactory.createPiece(arr[i], Color.White, pos));
                pos = new Coords(7 - i, 7);
                this.addToActive(ChessFactory.createPiece(arr[i], Color.White, pos));
            }
            // the pawns
            for (i = 0; i < 8; i++)
            {
                pos = new Coords(i, 1);
                this.addToActive(ChessFactory.createPiece(Pieces.Type.Pawn, Color.Black, pos));
                pos = new Coords(i, 6);
                this.addToActive(ChessFactory.createPiece(Pieces.Type.Pawn, Color.White, pos));
            }
            // the queens
            pos = new Coords(4, 0);
            this.addToActive(ChessFactory.createPiece(Pieces.Type.King, Color.Black, pos));
            pos = new Coords(4, 7);
            this.addToActive(ChessFactory.createPiece(Pieces.Type.King, Color.White, pos));
            // the kings
            pos = new Coords(3, 0);
            this.addToActive(ChessFactory.createPiece(Pieces.Type.Queen, Color.Black, pos));
            pos = new Coords(3, 7);
            this.addToActive(ChessFactory.createPiece(Pieces.Type.Queen, Color.White, pos));
            // get updated moves
            updateMoves();
        }

        private void updateMoves()
        {
            // update the valid moves of each piece
            foreach (var kv in this.activePieces)
                kv.Value.updateMoves(this);
        }

        public bool needPromotion(ChessPiece piece)
        {

            // check conditions
            if (piece.pos.y != 0 && piece.pos.y != 7)   // wrong row
                return false;
            if (piece == null || piece.type != Pieces.Type.Pawn)  // if there isn't a pawn at given position
                return false;
            if (piece.pos.y != (piece.color == Color.Black ? 7 : 0))   // if the color doesn't match the right row
                return false;
            // met conditions, return true
            return true;
        }

        public bool promotePawnAt(Coords pos, Pieces.Type promoteTo)
        {
            var pawn = getPieceAt(pos);
            // check conditions
            if (!needPromotion(pawn))
                return false;
            // every condition was met, promote the pawn
            this.removeFromActive(pawn);
            // remove from graveyard
            this.graveyard.Remove(pawn);
            // add promoted piece
            this.addToActive(ChessFactory.createPiece(promoteTo, pawn.color, pos));
            return true;
        }

        public bool check_Checkmate(Color color)
        {
            // get the king
            var kingPiece = (color == Color.Black ? blackPieces : whitePieces).Find(piece => piece.type == Pieces.Type.King);
            // if the king isn't active, he is in the graveyard, so its a win
            if (kingPiece == null)
                return true;
            // get his moves
            var moves = kingPiece.validMoves;
            // get the enemy moves
            var enMoves = getAllMovesOf((color == Color.Black) ? Color.White : Color.Black);
            // if atleast he have 1 free move, return false
            foreach (var move in moves)
            {
                // if there is a move that the enemy doesn't have, than the king can move safely
                if (!enMoves.Contains(move.after))
                    return false;   // at least 1 safe move
            }
            // check if the king himself is in danger, and return result
            return enMoves.Contains(kingPiece.pos);
        }

        public List<Coords> getAllMovesOf(Color color)
        {
            List<Coords> moves = new List<Coords>();
            var colorList = (color == Color.Black) ? blackPieces : whitePieces;
            colorList.ForEach((piece) =>
            {
                piece.validMoves.ForEach((move) =>
                {
                    moves.Add(move.after);
                });
            });
            return moves;
        }

        private bool addToActive(ChessPiece piece)
        {
            // if already in active, return false
            if (this.activePieces.ContainsKey(piece.pos))
                return false;
            // add to active list and the color list
            this.activePieces.Add(piece.pos, piece);
            if (piece.color == Color.Black)
                blackPieces.Add(piece);
            else
                whitePieces.Add(piece);
            // if the piece was in the graveyard, remove it from there
            if (graveyard.Contains(piece))
                graveyard.Remove(piece);
            // piece added, return true
            return true;
        }

        private bool removeFromActive(ChessPiece piece)
        {
            // if the piece isn't active, return false
            if (!this.activePieces.ContainsKey(piece.pos))
                return false;
            // remove from active list and the color list
            this.activePieces.Remove(piece.pos);
            if (piece.color == Color.Black)
                blackPieces.Remove(piece);
            else
                whitePieces.Remove(piece);
            // add to graveyard
            graveyard.Add(piece);
            // piece added, return true
            return true;
        }

        public Dictionary<Coords,ChessPiece> getActivePieces()
        {
            return this.activePieces;
        }

        public string getSaveData()
        {
            List<long> data = new List<long>();
            // first add the turn 
            data.Add((int) turn);
            // add the data of all pieces (add to each piece a digit for if its active or not), there is always a total of 32 pieces
            foreach (var kv in activePieces)
                data.Add(kv.Value.getSaveValue()*10+1);
            foreach (var val in graveyard)
                data.Add(val.getSaveValue()*10);
            // add the count of future moves, and the moves themselves
            data.Add(future.Count);
            foreach (var move in future)
                data.Add(move.getSaveValue());
            // add the count of history moves, and the moves themselves
            data.Add(history.Count);
            foreach (var move in history)
                data.Add(move.getSaveValue());
            // convert the list into a string, and return it
            return string.Join(" ", data);
        }

        public static Board getFromSave(string data)
        {
            try
            {
                // convert data to int list, and init board
                List<long> vals = new List<long>(Array.ConvertAll(data.Split(' '), long.Parse));
                Board board = new Board();
                ChessPiece piece;
                int i = 1;
                long count;
                // get turn
                board.turn = (Color)vals[0];
                // get all pieces
                for (; i <= 32; i++)
                {
                    piece = ChessFactory.createPiece(vals[i] / 10);
                    // if the first digit is 1, add to active pieces, else add to graveyard
                    if (vals[i] % 10 == 1)
                        board.addToActive(piece);
                    else
                        board.graveyard.Add(piece);
                }
                // get the count of future moves
                count = vals[i++];
                // get all future moves
                while (count>0)
                {
                    board.future.Push(Move.getFromSave(vals[i++]));
                    count--;
                }
                // get the count of history moves
                count = vals[i++];
                // get all history moves
                while (count > 0)
                {
                    board.history.Push(Move.getFromSave(vals[i++]));
                    count--;
                }
                // reverse the stacks
                revStack(board.future);
                revStack(board.history);
                // update moves
                board.updateMoves();
                // return the result
                return board;
            } catch (Exception e)
            {
                // if failed at any point, return null
                return null;
            }
        }

        private static void revStack(Stack<Move> st)
        {
            Queue<Move> qu = new Queue<Move>();
            while (st.Count != 0)
                qu.Enqueue(st.Pop());
            while (qu.Count != 0)
                st.Push(qu.Dequeue());
        }
    }
}
