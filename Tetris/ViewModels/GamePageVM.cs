using System.Collections.ObjectModel;
using Tetris.Models;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels
{
    public class GamePageVM : ObservableObject
    {
        public GridLength UserScreenHeight => ConstData.UserScreenHeight;

        public ObservableCollection<ColumnDefinition> ColumnDefinitions { get; } = new();
        public ObservableCollection<RowDefinition> RowDefinitions { get; } = [];

        public Game CurrentGame { get; }
        public GameBoard GameBoard { get; }

        // Flat collection for binding to Grid children
        public ObservableCollection<BoxView> BoxViews { get; } = new();

        public GamePageVM(Game game)
        {
            CurrentGame = game;
            GameBoard = new();

            // Build grid definitions from the cube sizes
            for (int c = 0; c < ConstData.GameGridColumnCount; c++)
                ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(GameBoard.Board![0, c].Width) });

            for (int r = 0; r < ConstData.GameGridRowCount; r++)
                RowDefinitions.Add(new RowDefinition { Height = new GridLength(GameBoard.Board![r, 0].Height) });

            // Flatten BoxViews for binding
            BoxView[,] boxViewsArray = GameBoard.ToBoxViews(); // returns BoxView[,]
            for (int r = 0; r < boxViewsArray.GetLength(0); r++)
                for (int c = 0; c < boxViewsArray.GetLength(1); c++)
                    BoxViews.Add(boxViewsArray[r, c]);
        }

        // Optional: call this to refresh colors instead of recreating
        public void RefreshBoard()
        {
            var board = GameBoard.Board;
            int rows = board!.GetLength(0);
            int cols = board!.GetLength(1);

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    int index = r * cols + c; // flatten index
                    BoxViews[index].Color = board[r, c].Color;
                }
        }
    }

}
