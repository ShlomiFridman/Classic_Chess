   M S B A M L   `   `   `  ���� H  DClassic_Chess, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:Classic_ChessClassic_Chess      Classic_Chess.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -localclr-namespace:Classic_Chess       � Icon$3  -pack://application:,,,/assets/images/icon.png�� � 
ResizeMode$ NoResize=� � SizeToContent$ WidthAndHeight=� � Title$ Classic Chess��
�������.����  �� Orientation$ Vertical=�$��0 0 40 0q�.+�� $��10q�$��Center=�$��Center=� � ColumnDefinitions �� $��* ��� $��2* �.+��� $ Vertical=�$	��200��$��0��$	��5 0q�$��Center=�.+��� $	��175��
�������
���   �$��1q�._�`� .�c� -    c� Band$ 1�� c� 	BandIndex$ 1�� `� IsLocked$
 True��.@��� -   	 � ToolTip$	 New Game��$��23��.���� $2��,pack://application:,,,/assets/images/new.png���� -   $	 	Save Game��$��23��.���� $3��-pack://application:,,,/assets/images/save.png���� -   $	 	Load Game��$��23��.���� $3��-pack://application:,,,/assets/images/load.png���� �� -   
 ��Name$
 UndoBtn��$	 	Undo Move��$��23��${�false��.���� -    ��Name$ UndoImg��$8��2pack://application:,,,/assets/images/undo_grey.png���� -   $
 RedoBtn��$	 	Redo Move��$��23��${�false��.���� -   $ RedoImg��$8��2pack://application:,,,/assets/images/redo_grey.png���� �� -	   $	 Perspective��.���� $:��4pack://application:,,,/assets/images/perspective.png��o� .@�m� -
    m�Name$ PerMenu��${�false��.@�m� ${�false������ $��Perspective��
���   �$��	Underline{�m� -   $ 
PerDefault��$��Default��m� -   $ PerWhite��$��White��m� -   $ PerBlack��$��Black���� -    ��Name$ turnText��$��0 7 0 0q�$	��100�� �� TextAlignment$ Center=�White's turn�� -   $ infoText��$ Center=�$
��Bold"��� $��	Graveyard��$��15(�$��	Underline{�$��Center=�� -    �Name$ graveyardPanel��$	��500�� � Orientation$ 
Horizontal=��� $��6q�
���OO/�$	��570��$��3q�$��1��._��� 
���E��.+�� -    �Name$ 	BoardGrid��$y�Visible=�� -   $ FlippedBoardGrid��$y�	Collapsed=�