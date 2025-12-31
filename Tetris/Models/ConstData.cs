namespace Tetris.Models
{
    public static class ConstData
    {
        public const int MinCharacterInUN = 5;
        public const int MinCharacterInPW = 8;
        public const int MinCharacterInEmail = 5;
        public const int ToastFontSize = 14;
        public static readonly GridLength UserScreenHeight = new(1.9, GridUnitType.Star);
        public const int GameGridColumnCount = 10;
        public const int GameGridRowCount = 20;
        public const double GameGridColumnWidth = 20;
        public const double GameGridRowHeight = 20;
        public const double OpGameGridColumnWidth = 12;
        public const double OpGameGridRowHeight = 12;
        public const double BetweenCubesBorderWidth = 1;
        public const int ShapesCount = 10;
        public const int ShapeFallInternalS = 1;
        public const double OpShapeFallIntervalS = 0.3;
        public const int DeleteFbDocsIntervalS = 3600;
        public const long TotalGameTimeS = 6;
        public const long GameTimeIntervalS = 1;
        public static readonly List<bool[,]>[] ShapeRotationState =
            [
                ShapeRotationStates.IShape,
                ShapeRotationStates.iShape,
                ShapeRotationStates.JShape,
                ShapeRotationStates.LShape,
                ShapeRotationStates.ZShape,
                ShapeRotationStates.Z2Shape,
                ShapeRotationStates.oShape,
                ShapeRotationStates.OShape,
                ShapeRotationStates.gShape,
                ShapeRotationStates.XShape
            ];
        public static readonly Color[] colors =
            [
                Colors.Red,
                Colors.Orange,
                Colors.Yellow,
                Colors.Green,
                Colors.Blue,
                Colors.Indigo,
                Colors.Violet,
                Colors.LightBlue
            ];

        public static class ShapeRotationStates
        {
            public static readonly List<bool[,]> IShape =
            [
                new bool[,] {
                    { true, true, true, true, true }
                },
                new bool[,] {
                    { true },
                    { true },
                    { true },
                    { true },
                    { true }
                }
            ];
            public static readonly List<bool[,]> iShape =
            [
                new bool[,] {
                    { true, true, true, true }
                },
                new bool[,] {
                    { true },
                    { true },
                    { true },
                    { true }
                }
            ];
            public static readonly List<bool[,]> JShape =
            [
                new bool[,] {
                    { true, false, false },
                    { true, true, true }
                },
                new bool[,] {
                    { true, true },
                    { true, false },
                    { true, false }
                },
                new bool[,] {
                    { true, true, true },
                    { false, false, true }
                },
                new bool[,] {
                    { false, true },
                    { false, true },
                    { true, true }
                }
            ];
            public static readonly List<bool[,]> LShape =
            [
                new bool[,] {
                    { false, false, true },
                    { true, true, true }
                },
                new bool[,] {
                    { true, false },
                    { true, false },
                    { true, true }
                },
                new bool[,] {
                    { true, true, true },
                    { true, false, false }
                },
                new bool[,] {
                    { true, true },
                    { false, true },
                    { false, true }
                }
            ];
            public static readonly List<bool[,]> ZShape =
            [
                new bool[,] {
                    { true, true, false },
                    { false, true, true }
                },
                new bool[,] {
                    { false, true },
                    { true, true },
                    { true, false }
                }
            ];
            public static readonly List<bool[,]> Z2Shape =
            [
                new bool[,] {
                    { false, true, true },
                    { true, true, false }
                },
                new bool[,] {
                    { true, false },
                    { true, true },
                    { false, true }
                }
            ];
            public static readonly List<bool[,]> oShape =
            [
                new bool[,] {
                    { true, true},
                    { true, true }
                }
            ];
            public static readonly List<bool[,]> OShape =
            [
                new bool[,] {
                    { true, true, true},
                    { true, true, true },
                    { true, true, true }
                }
            ];
            public static readonly List<bool[,]> gShape =
            [
                new bool[,] {
                    { false, true, false},
                    { true, true, true },
                },
                new bool[,] {
                    { true, false },
                    { true, true },
                    { true, false }
                },
                new bool[,] {
                    { true, true, true},
                    { false, true, false },
                },
                new bool[,] {
                    { false, true },
                    { true, true },
                    { false, true }
                }
            ];
            public static readonly List<bool[,]> XShape =
            [
                new bool[,] {
                    { false, true, false},
                    { true, true, true },
                    { false, true, false }
                }
            ];
        }
    }
}
