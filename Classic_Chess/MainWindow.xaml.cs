using Classic_Chess.MyClasses;
using Classic_Chess.MyClasses.Pieces;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
                FlippedBoardGrid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(62)
                });
                FlippedBoardGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(62)
                });
            }
            for (i = 0; i < 8; i++)
                for (j = 0; j < 8; j++) 
                {
                    var piece = gameBoard.getPieceAt(new Coords(j, i));
                    // create the reguler panel
                    var panel = createStackPanel(new Coords(j, i));
                    // add the click event
                    panel.MouseLeftButtonUp += panelMouseLeftButtonUp;
                    // add the panel to the board
                    BoardGrid.Children.Add(panel);
                    // create the flipped panel
                    var flippedPanel = createStackPanel(new Coords(j, (7-i)%8));
                    // set tag to the regular panel
                    flippedPanel.Tag = panel;
                    // add the flipped click event
                    flippedPanel.MouseLeftButtonUp += flippedPanelMouseLeftButtonUp;
                    // add the fpanel to the fboard
                    FlippedBoardGrid.Children.Add(flippedPanel);
                    // add image, if needed
                    if (piece != null)
                        setImage(panel, piece, true);
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

        private void flippedPanelMouseLeftButtonUp(object obj, MouseButtonEventArgs e)
        {
            StackPanel fp = (StackPanel)obj;
            panelMouseLeftButtonUp(fp.Tag, e);
        }

        private void makeMove(Move move, bool isRedo)
        {
            int checkFlag;
            StackPanel panel;
            var piece = move.piece;
            MyType promotSelection;
            // make move on gameBoard
            gameBoard.makeMove(move, isRedo);
            // move the piece image
            movePiece(getPanel(move.before), getPanel(move.after), false);
            movePiece(getFPanel(move.before), getFPanel(move.after), true);
            // change enabled status for the redo and undo buttons
            setUndoBtnStatus(true);
            setRedoBtnStatus(false);
            // if pawn, check if need promotion
            if (gameBoard.needPromotion(piece))
            {
                // show dialog
                var dialog = new PromotionDialog();
                if (dialog.ShowDialog() == true)
                {
                    promotSelection = dialog.Selection;
                    // promote the pawn
                    gameBoard.promotePawnAt(piece.pos, promotSelection);
                    // get updated piece
                    piece = gameBoard.getPieceAt(piece.pos);
                    // update image in the panel
                    panel = getPanel(piece.pos);
                    // add new image
                    setImage(panel, piece, true);
                }
                // if no option was selected, undo the move
                else
                {
                    UndoBtn_Click(null, null);
                    return;
                }
            }
            // check checkmate
            checkFlag = gameBoard.check_Checkmate();
            if (checkFlag != 0)
            {
                // show message and change UI flags
                infoText.Text = ((checkFlag == 2) ? MyColor.Black.ToString() : MyColor.White.ToString()) + " Wins";
                //resetBoard();
                BoardGrid.IsHitTestVisible = false;
                FlippedBoardGrid.IsHitTestVisible = false;
                return;
            }
            // change turn
            UpdateTurn();
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
                //var panel = (gameBoard.turn == MyColor.White)? getPanel(move.after) : getFPanel(move.after);
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
                // show the moves on the flipped board
                var fpanel = getFPanel(move.after);
                if (move.pieceAt == null)
                    fpanel.Background = Brushes.DeepSkyBlue;
                // if ally
                else if (move.piece.isAlly(move.pieceAt))
                    fpanel.Background = Brushes.DarkSlateBlue;
                // else enemy
                else
                    fpanel.Background = Brushes.DarkRed;
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
                getFPanel(move.after).Background = ((move.after.x + move.after.y) % 2 != 0) ? Brushes.GhostWhite : Brushes.DarkGray;
            });
        }

        private StackPanel getPanel(Coords pos)
        {
            return (StackPanel) BoardGrid.Children.Cast<UIElement>().First((el) => (Grid.GetRow(el) == pos.y && Grid.GetColumn(el) == pos.x));
        }

        private StackPanel getFPanel(Coords pos)
        {
            return (StackPanel)FlippedBoardGrid.Children.Cast<UIElement>().First((el) => ((7 - Grid.GetRow(el)%8) == pos.y && Grid.GetColumn(el) == pos.x));
        }

        private Coords getPos(Object obj)
        {
            return (Coords) ((StackPanel) obj).Tag;
        }

        private void setImage(StackPanel panel, ChessPiece piece, bool addToFlipped)
        {
            // create the image
            Image img = getImage(piece);
            // if there was already a child to the panel, remove it
            if (panel.Children.Count != 0)
                panel.Children.Clear();
            // add the image
            panel.Children.Add(img);
            // if the flag is true, add to flipped board
            if (addToFlipped)
            {
                var fpanel = getFPanel((Coords)panel.Tag);
                setImage(fpanel, piece, false);
            }
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
            // clear flipped board, O(1)
            foreach (var child in FlippedBoardGrid.Children)
            {
                StackPanel fp = (StackPanel)child;
                fp.Children.Clear();
            }
            // clear graveyard
            graveyardPanel.Children.Clear();
            // reset game
            gameBoard.resetBoard();
            // set the new pieces
            foreach (var kv in gameBoard.getActivePieces())
            {
                setImage(getPanel(kv.Key), kv.Value, true);
            }
            // reset turn flag and selected
            gameBoard.turn = MyColor.White;
            UpdateTurnUI();
            selected = null;
            // clear info text
            infoText.Text = "";
            // change UI flags
            setUndoBtnStatus(false);
            setRedoBtnStatus(false);
            BoardGrid.IsHitTestVisible = true;
            FlippedBoardGrid.IsHitTestVisible = true;
        }

        private void UpdateTurn()
        {
            // flip turn, update text, and null select
            gameBoard.turn = (gameBoard.turn == MyColor.White) ? MyColor.Black : MyColor.White;
            UpdateTurnUI();
        }

        private void UpdateTurnUI()
        {
            // update text, and null select
            turnText.Text = gameBoard.turn.ToString() + "'s turn";
            selected = null;
            // show the right board
            if (gameBoard.turn == MyColor.White)
            {
                BoardGrid.Visibility = Visibility.Visible;
                FlippedBoardGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                BoardGrid.Visibility = Visibility.Collapsed;
                FlippedBoardGrid.Visibility = Visibility.Visible;
            }
        }

        private void movePiece(StackPanel fromPanel, StackPanel toPanel, bool isFlipped)
        {
            UIElement enImg;
            UIElement img;
            // if need be remove enemy image to the graveyard
            if (toPanel.Children.Count != 0)
            {
                enImg = toPanel.Children[0];
                toPanel.Children.Clear();
                // add to graveyard only if not flipped panel
                if (!isFlipped)
                    graveyardPanel.Children.Add(enImg);
            }
            img = fromPanel.Children[0];
            fromPanel.Children.Clear();
            toPanel.Children.Add(img);
        }

        private void UndoBtn_Click(object sender, RoutedEventArgs e)
        {
            Move move;
            UIElement enImg;
            // if there is nothing to undo, show message and return
            if (gameBoard.haveUndo() == false)
            {
                infoText.Text = "There are no moves to undo";
                return;
            }
            // hide shown moves
            hideMoves();
            // make the move, and get it from board
            move = gameBoard.undo();
            // update piece image and position
            getPanel(move.after).Children.Clear();
            getFPanel(move.after).Children.Clear();
            setImage(getPanel(move.before), move.piece, true);
            // if there was an enemy, re-add its image from graveyard
            if (move.pieceAt != null)
            {
                enImg = graveyardPanel.Children[graveyardPanel.Children.Count - 1];
                graveyardPanel.Children.Remove(enImg);
                setImage(getPanel(move.after), move.pieceAt, true);
            }
            // change turn back
            UpdateTurn();
            // enable redo button
            setRedoBtnStatus(true);
            // check if need to disable undo button
            if (gameBoard.haveUndo() == false)
                setUndoBtnStatus(false);
            // change if need to make the grid hit-able
            if (!BoardGrid.IsHitTestVisible)
            {
                BoardGrid.IsHitTestVisible = true;
                FlippedBoardGrid.IsHitTestVisible = true;
            }
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
                setUndoBtnStatus(true);
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            // create save data
            string saveData = gameBoard.getSaveData();
            // save the string data
            Properties.Settings.Default.SaveData1 = saveData;
            Properties.Settings.Default.Save();
            // show message
            infoText.Text = "Saved Board";
        }

        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            int checkFlag;
            // get save data
            string data = Properties.Settings.Default.SaveData1;
            // build the object
            var saveData = Board.getFromSave(data);
            // clear info text
            // if the data is null, print message and return
            if (saveData == null)
            {
                infoText.Text = "No save data";
                return;
            }
            // hide moves if there are any
            if (selected != null)
                hideMoves();
            // clear graveyard
            graveyardPanel.Children.Clear();
            // clear the board and load the save
            foreach (var kv in gameBoard.getActivePieces())
            {
                getPanel(kv.Key).Children.Clear();
                getFPanel(kv.Key).Children.Clear();
            }
            this.gameBoard = saveData;
            UpdateTurnUI();
            // load active pieces to board
            foreach (var kv in gameBoard.getActivePieces())
                setImage(getPanel(kv.Key), kv.Value, true);
            // load graveyard
            foreach (var piece in gameBoard.getGraveyard())
                graveyardPanel.Children.Add(getImage(piece));
            // show message
            infoText.Text = "Board loaded";
            // change buttons status
            setUndoBtnStatus(false);
            setRedoBtnStatus(false);
            // if in checkmate, change flags
            checkFlag = gameBoard.check_Checkmate();
            if (checkFlag != 0)
            {
                // show message and change UI flags
                infoText.Text += ", " + ((checkFlag == 2) ? MyColor.Black.ToString() : MyColor.White.ToString()) + " Wins";
                //resetBoard();
                BoardGrid.IsHitTestVisible = false;
                FlippedBoardGrid.IsHitTestVisible = false;
            }
            else
            {
                BoardGrid.IsHitTestVisible = true;
                FlippedBoardGrid.IsHitTestVisible = true;
            }
        }

        private void NewGameBtn_Click(object sender, RoutedEventArgs e)
        {
            resetBoard();
        }

        private StackPanel createStackPanel(Coords pos)
        {
            var stackPanel = new StackPanel();
            int i = pos.y, j = pos.x;
            Grid.SetRow(stackPanel, i);
            Grid.SetColumn(stackPanel, j);
            stackPanel.Background = ((i + j) % 2 == 0) ? Brushes.GhostWhite : Brushes.DarkGray;
            stackPanel.Tag = pos;
            stackPanel.Margin = new Thickness(1);
            stackPanel.MinHeight = stackPanel.MinWidth = 50;
            return stackPanel;
        }

        /// <summary>
        /// hides the overflow button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }

            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness(0);
            }
        }

        private void setUndoBtnStatus(bool status)
        {
            UndoBtn.IsEnabled = status;
            UndoImg.Source = new BitmapImage(new Uri("pack://application:,,,/assets/images/undo"+ (UndoBtn.IsEnabled? "": "_grey") + ".png"));
        }
        private void setRedoBtnStatus(bool status)
        {
            RedoBtn.IsEnabled = status;
            RedoImg.Source = new BitmapImage(new Uri("pack://application:,,,/assets/images/redo" + (RedoBtn.IsEnabled ? "" : "_grey") + ".png"));
        }
    }
}
