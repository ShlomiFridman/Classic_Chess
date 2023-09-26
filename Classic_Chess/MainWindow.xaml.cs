using Classic_Chess.MyClasses;
using Classic_Chess.MyClasses.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyColor = Classic_Chess.MyClasses.Pieces.Color;
using MyType = Classic_Chess.MyClasses.Pieces.Type;

namespace Classic_Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Board gameBoard;

        private ChessPiece selected;

        public MainWindow()
        {
            int i,j;
            // init game board
            this.gameBoard = Board.getSetBoard();
            this.gameBoard.turn = MyColor.White;
            // init window elements
            InitializeComponent();
            // build default grid
            for (i = 0; i < 8; i++)
            {
                BoardGrid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(62)
                });
                BoardGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(62)
                });
            }
            for (i = 0; i < 8; i++)
                for (j = 0; j < 8; j++) 
                {
                    var pos = new Coords(j, i);
                    var piece = gameBoard.getPieceAt(pos);
                    var stackPanel = new StackPanel();
                    Grid.SetRow(stackPanel, i);
                    Grid.SetColumn(stackPanel, j);
                    stackPanel.Background = ((i + j) % 2 == 0)? Brushes.GhostWhite : Brushes.DarkGray;
                    stackPanel.Tag = pos;
                    stackPanel.MouseLeftButtonUp += panelMouseLeftButtonUp;
                    stackPanel.Margin = new Thickness(1);
                    stackPanel.MinHeight = stackPanel.MinWidth = 50;
                    // if there is a piece there, add img
                    if (piece != null)
                        stackPanel.Children.Add(getImage(piece));
                    BoardGrid.Children.Add(stackPanel);
                }
        }

        private void panelMouseLeftButtonUp(object obj, MouseButtonEventArgs e)
        {
            Coords pos = getPos(obj);
            Move move;
            // hide previous moves
            hideMoves();
            // if selected isn't null, check if its a move
            if (selected != null)
            {
                move = selected.validMoves.Find((move) => move.after.Equals(pos));
                // if move isn't null, and not an ally, then its a move
                if (move != null && !move.piece.isAlly(move.pieceAt))
                    makeMove(move, false);
                // else show the moves of the position
                else
                    showMoves(pos);
            }
            // else show moves
            else
                showMoves(pos);
        }

        private void makeMove(Move move, bool isRedo)
        {
            StackPanel panel;
            var piece = move.piece;
            MyType promotSelection;
            // make move on gameBoard
            gameBoard.makeMove(move, isRedo);
            // move the piece image
            movePiece(move.before, move.after);
            // change turn
            UpdateTurn();
            // change enabled status for the redo and undo buttons
            UndoBtn.IsEnabled = true;
            RedoBtn.IsEnabled = false;
            // if pawn, check if need promotion
            if (gameBoard.needPromotion(piece))
            {
                // show dialog
                var dialog = new PromotionDialog();
                if (dialog.ShowDialog() == true)
                {
                    promotSelection = dialog.Selection;
                    // TODO promote pawn
                    // promote the pawn
                    gameBoard.promotePawnAt(piece.pos, promotSelection);
                    // get updated piece
                    piece = gameBoard.getPieceAt(piece.pos);
                    // update image
                    panel = getPanel(piece.pos);
                    // clear previous image
                    panel.Children.Clear();
                    // add new image
                    panel.Children.Add(getImage(piece));
                }
                // if no option was selected, undo the move
                else
                {
                    UndoBtn_Click(null, null);
                    return;
                }
            }
            // check checkmate
            if (gameBoard.check_Checkmate(gameBoard.turn))
            {
                infoText.Text = ((gameBoard.turn == MyColor.White) ? MyColor.Black.ToString() : MyColor.White.ToString()) + " Wins";
                resetBoard();
            }
        }

        private void showMoves(Coords pos)
        {
            // if there isn't a piece at given position, or if its the same piece, or wrong color for the turn, then null selected and return
            var piece = gameBoard.getPieceAt(pos);
            if (piece == null || piece == selected || piece.color != gameBoard.turn)
            {
                this.selected = null;
                return;
            }
            // set selected
            this.selected = piece;
            // highlight all moves
            this.selected.validMoves.ForEach((move) =>
            {
                var panel = getPanel(move.after);
                // if empty
                if (move.pieceAt == null)
                    panel.Background = Brushes.DeepSkyBlue;
                // if ally
                else if (move.piece.isAlly(move.pieceAt))
                    panel.Background = Brushes.DarkSlateBlue;
                // else enemy
                else
                    panel.Background = Brushes.DarkRed;
            });
        }

        private void hideMoves()
        {
            // if selected is null, return
            if (selected == null)
                return;
            this.selected.validMoves.ForEach((move) =>
            {
                getPanel(move.after).Background = ((move.after.x + move.after.y) % 2 == 0) ? Brushes.GhostWhite : Brushes.DarkGray;
            });
        }

        private StackPanel getPanel(Coords pos)
        {
            return (StackPanel) BoardGrid.Children.Cast<UIElement>().First((el) => (Grid.GetRow(el) == pos.y && Grid.GetColumn(el) == pos.x));
        }

        private Coords getPos(Object obj)
        {
            return (Coords) ((StackPanel) obj).Tag;
        }

        private Image getImage(ChessPiece piece)
        {
            Image img = new Image();
            img.Width = img.Height = 50;
            img.IsHitTestVisible = false;
            img.Source = new BitmapImage(new Uri($"pack://application:,,,/assets/pieces/{piece.color.ToString()}_{piece.type.ToString()}.png"));
            return img;
        }

        private void resetBoard()
        {
            // clear board
            foreach (var kv in gameBoard.getActivePieces())
            {
                getPanel(kv.Key).Children.Clear();
            }
            // reset game
            gameBoard.resetBoard();
            // set the new pieces
            foreach (var kv in gameBoard.getActivePieces())
            {
                getPanel(kv.Key).Children.Add(getImage(kv.Value));
            }
            // reset turn flag and selected
            gameBoard.turn = MyColor.White;
            turnText.Text = gameBoard.turn.ToString() + "'s turn";
            selected = null;
            // change buttons status
            UndoBtn.IsEnabled = false;
            RedoBtn.IsEnabled = false;
        }

        private void UpdateTurn()
        {
            // flip turn, update text, and null select
            gameBoard.turn = gameBoard.turn == MyColor.White ? MyColor.Black : MyColor.White;
            turnText.Text = gameBoard.turn.ToString() + "'s turn";
            selected = null;
            // delete info text if there is any
            if (infoText.Text.Length != 0)
                infoText.Text = "";
        }

        private void movePiece(Coords from, Coords to)
        {

            var fromPanel = getPanel(from);
            var toPanel = getPanel(to);
            UIElement img;
            // if need be remove enemy image
            if (toPanel.Children.Count != 0)
                toPanel.Children.Clear();
            img = fromPanel.Children[0];
            fromPanel.Children.Clear();
            toPanel.Children.Add(img);
        }

        private void UndoBtn_Click(object sender, RoutedEventArgs e)
        {
            // if there is nothing to undo, show message and return
            if (gameBoard.haveUndo() == false)
            {
                infoText.Text = "There are no moves to undo";
                return;
            }
            // hide shown moves
            hideMoves();
            // make the move, and get it from board
            var move = gameBoard.undo();
            // update piece image and position
            getPanel(move.after).Children.Clear();
            getPanel(move.before).Children.Add(getImage(move.piece));
            // if there was an enemy, re-add its image
            if (move.pieceAt != null)
            {
                getPanel(move.after).Children.Add(getImage(move.pieceAt));
            }
            // change turn back
            UpdateTurn();
            // enable redo button
            RedoBtn.IsEnabled = true;
            // check if need to disable undo button
            if (gameBoard.haveUndo() == false)
                UndoBtn.IsEnabled = false;
        }

        private void RedoBtn_Click(object sender, RoutedEventArgs e)
        {
            // if there is nothing to redo, show message and return
            if (gameBoard.haveRedo() == false)
            {
                infoText.Text = "There are no moves to redo";
                return;
            }
            // make the move on board, and get it
            var move = gameBoard.getRedo();
            // make the move on screen
            this.makeMove(move, true);
            // if there are more redos, re-enable the button
            if (gameBoard.haveRedo() == true)
                RedoBtn.IsEnabled = true;
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            // create save data
            string saveData = gameBoard.getSaveData();
            // save the string data
            Properties.Settings.Default.SaveData1 = saveData;
            Properties.Settings.Default.Save();
            // show message
            infoText.Text = "Saved data";
        }

        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            // save the old turn, for later use
            MyColor oldTurn = gameBoard.turn;
            // get save data
            string data = Properties.Settings.Default.SaveData1;
            // build the object
            var saveData = Board.getFromSave(data);
            // if the data is null, print message and return
            if (saveData == null)
            {
                infoText.Text = "No save data";
                return;
            }
            // clear the board and load the save
            foreach (var kv in gameBoard.getActivePieces())
                getPanel(kv.Key).Children.Clear();
            this.gameBoard = saveData;
            if (this.gameBoard.turn != oldTurn)
                UpdateTurn();
            foreach (var kv in gameBoard.getActivePieces())
                getPanel(kv.Key).Children.Add(getImage(kv.Value));
            // show message
            infoText.Text = "Save loaded";
            // change buttons status
            UndoBtn.IsEnabled = false;
            RedoBtn.IsEnabled = false;
        }
    }
}
