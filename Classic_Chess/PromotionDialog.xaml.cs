using Classic_Chess.MyClasses.Pieces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MyType = Classic_Chess.MyClasses.Pieces.Type;

namespace Classic_Chess
{
    /// <summary>
    /// Interaction logic for PromoteWindow.xaml
    /// </summary>
    public partial class PromotionDialog : Window
    {

        public MyType Selection;

        public PromotionDialog()
        {
            this.Selection = MyType.Pawn;
            InitializeComponent();
        }

        private void Option_Click(object sender, RoutedEventArgs e)
        {
            if (sender == RookOP)
                this.Selection = MyType.Rook;
            else if (sender == BishopOP)
                this.Selection = MyType.Bishop;
            else if (sender == KnightOP)
                this.Selection = MyType.Knight;
            else if (sender == QueenOP)
                this.Selection = MyType.Queen;
            DialogResult = true;
        }
    }
}
