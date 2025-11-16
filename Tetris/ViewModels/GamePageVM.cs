using System.Collections.ObjectModel;
using Tetris.Models;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels
{
    public partial class GamePageVM : ObservableObject
    {
        public GridLength UserScreenHeight => ConstData.UserScreenHeight;

        public ObservableCollection<ColumnDefinition> ColumnDefinitions { get; } = [];
        public ObservableCollection<RowDefinition> RowDefinitions { get; } = [];

        public Game CurrentGame { get; }
        public GameBoard GameBoard { get; }
        public Grid? GameBoardGrid { get; set; }
        public GamePageVM(Game game)
        {
            CurrentGame = game;
            GameBoard = new();

            // Build grid definitions from the cube sizes
            for (int c = 0; c < ConstData.GameGridColumnCount; c++)
                ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(GameBoard.Board![0, c].Width) });

            for (int r = 0; r < ConstData.GameGridRowCount; r++)
                RowDefinitions.Add(new RowDefinition { Height = new GridLength(GameBoard.Board![r, 0].Height) });
        }
    }
}
