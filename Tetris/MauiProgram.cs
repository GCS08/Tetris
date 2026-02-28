using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Tetris.Interfaces;
using Tetris.Models;
using Tetris.ModelsLogic;

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

            builder.Services.AddSingleton<INotificationManagerService, Tetris.Platforms.Android.NotificationManagerService>();
            builder.Services.AddSingleton<IFbData, FbData>();
            builder.Services.AddSingleton<IUser, User>();

            return builder.Build();
        }
    }
}
