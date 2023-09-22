﻿using Classic_Chess.MyClasses;
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
using Color = Classic_Chess.MyClasses.Pieces.Color;

namespace Classic_Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Board gameBoard;

        private ChessPiece selected;

        private Color turn;  // true = white , false = black

        public MainWindow()
        {
            int i,j;
            // init game board
            this.gameBoard = new Board();
            this.turn = Color.White;
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
                    stackPanel.Margin = new Thickness(3);
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
                // if move isn't null, than its a move
                if (move != null)
                    makeMove(move);
                // else show the moves of the position
                else
                    showMoves(pos);
            }
            // else show moves
            else
                showMoves(pos);
        }

        private void makeMove(Move move)
        {
            var beforePanel = getPanel(move.before);
            var afterPanel = getPanel(move.after);
            UIElement img;
            // make move on gameBoard
            gameBoard.makeMove(move);
            // if need be remove enemy image
            if (move.enemy != null)
               afterPanel.Children.Clear();
            // move the piece image
            img = beforePanel.Children[0];
            beforePanel.Children.Clear();
            afterPanel.Children.Add(img);
            // change turn
            ChangeTurn();
            // check checkmate
            if (gameBoard.check_Checkmate(turn))
            {
                infoText.Text = ((turn == Color.White) ? Color.Black.ToString() : Color.White.ToString()) + " Wins";
                resetBoard();
            }
        }

        private void showMoves(Coords pos)
        {
            // if there isn't a piece at given position, or if its the same piece, or wrong color for the turn, then null selected and return
            var piece = gameBoard.getPieceAt(pos);
            if (piece == null || piece == selected || piece.color != turn)
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
                panel.Background = move.enemy != null ? Brushes.DarkRed : Brushes.DeepSkyBlue;
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
            turn = Color.White;
            turnText.Text = turn.ToString() + "'s turn";
            selected = null;
        }

        private void ChangeTurn()
        {
            // flip turn, update text, and null select
            turn = turn == Color.White ? Color.Black : Color.White;
            turnText.Text = turn.ToString() + "'s turn";
            selected = null;
            // delete info text if there is any
            if (infoText.Text.Length != 0)
                infoText.Text = "";
        }
    }
}