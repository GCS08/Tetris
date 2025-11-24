using Tetris.Models;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels;

public partial class GamePageVM : ObservableObject
{
    public GridLength UserScreenHeight => ConstData.UserScreenHeight;

    public Game CurrentGame { get; }
    public GameBoard GameBoard { get; } = new();
    public Grid? GameBoardGrid { get; set; }
    public GamePageVM(Game game)
    {
        CurrentGame = game;
        GameBoard.ShowShape(CurrentGame.ShapesQueue!);
    }
    public void InitializeGrid()
    {
        // Create RowDefinitions and ColumnDefinitions
        for (int r = 0; r < ConstData.GameGridRowCount; r++)
            GameBoardGrid!.RowDefinitions.Add(new RowDefinition { Height = ConstData.GameGridRowHeight });

        for (int c = 0; c < ConstData.GameGridColumnCount; c++)
            GameBoardGrid!.ColumnDefinitions.Add(new ColumnDefinition { Width = ConstData.GameGridColumnWidth });

        // Add BoxViews and bind BackgroundColor to CubeModel.Color
        for (int r = 0; r < ConstData.GameGridRowCount; r++)
        {
            for (int c = 0; c < ConstData.GameGridColumnCount; c++)
            {
                Cube cube = GameBoard.Board![r, c];

                BoxView boxView = new()
                {
                    WidthRequest = cube.Width,
                    HeightRequest = cube.Height,
                    BackgroundColor = cube.Color
                };

                // Listen for color changes
                cube.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(cube.Color))
                        boxView.BackgroundColor = cube.Color;
                };

                // Wrap in a Border
                Border border = new()
                {
                    Margin = -0.5 * ConstData.BetweenCubesBorderWidth,
                    Stroke = Colors.Gray,
                    StrokeThickness = ConstData.BetweenCubesBorderWidth,
                    Background = Colors.Transparent,
                    Content = boxView
                };

                GameBoardGrid.Add(border, c, r);
            }
        }
    }
}
