﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Tetris.Models;

namespace Tetris
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont(TechnicalConsts.MaterialSymbolsFontPath, Keys.MaterialSymbolsFontName);
                    fonts.AddFont(TechnicalConsts.OpenSansRegular, Keys.OpenSansRegularFontName);
                    fonts.AddFont(TechnicalConsts.OpenSansSemiBold, Keys.OpenSansSemiboldFontName);
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
