using System.Windows.Input;
using Tetris.Models;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels
{
    public class GamePageVM
    {
        public Game CurrentGame { get; set; }
        public GamePageVM(Game game)
        {
            CurrentGame = game;
        }
    }
}
