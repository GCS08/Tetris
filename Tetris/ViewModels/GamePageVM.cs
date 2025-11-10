using System.Windows.Input;
using Tetris.Models;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels
{
    public class GamePageVM
    {
        public GridLength UserScreenHeight => ConstData.UserScreenHeight;
        public RowDefinitionCollection? ColumnDefinitions { get; private set; } = new();
        public Game CurrentGame { get; set; }
        public GamePageVM(Game game)
        {
            CurrentGame = game;
            for (int i = 0; i < ConstData.GameGridColumnCount; i++)
            {
                
            }
        }
    }
}
