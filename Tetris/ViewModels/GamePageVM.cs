using Tetris.Models;
using Tetris.ModelsLogic;
using System.Windows.Input;

namespace Tetris.ViewModels;

public partial class GamePageVM : ObservableObject
{
    public GridLength UserScreenHeight => ConstData.UserScreenHeight;

    public ICommand MoveRightShapeCommand => new Command(MoveRightShape);
    public ICommand MoveLeftShapeCommand => new Command(MoveLeftShape);
    public ICommand MoveDownShapeCommand => new Command(MoveDownShape);
    public ICommand RotateShapeCommand => new Command(RotateShape);

    public Game CurrentGame { get; }
    public Grid? GameBoardGrid { get; set; }

    public GamePageVM(Game game)
    {
        CurrentGame = game;
        CurrentGame.StartGame();
    }
    private void MoveRightShape()
    {
        CurrentGame.GameBoard!.MoveRightShape();
    }
    private void MoveLeftShape()
    {
        CurrentGame.GameBoard!.MoveLeftShape();
    }
    private void MoveDownShape()
    {
        CurrentGame.GameBoard!.MoveDownShape();
    }
    private void RotateShape()
    {
        CurrentGame.GameBoard!.RotateShape();
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
                Cube cube = CurrentGame.GameBoard!.Board![r, c];

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

    public void AddListener()
    {
        CurrentGame.GameBoard!.AddListener();
    }

    public void RemoveListener()
    {
        CurrentGame.GameBoard!.RemoveListener();
    }
}
