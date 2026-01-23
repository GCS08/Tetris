using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Tetris.Models;
using static Microsoft.Maui.ApplicationModel.Permissions;

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

#if ANDROID
            builder.Services.AddSingleton<INotificationManagerService, Platforms.Android.NotificationManagerService>();
#endif
            return builder.Build();
        }
    }
}
