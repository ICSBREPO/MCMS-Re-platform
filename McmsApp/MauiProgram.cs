using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace McmsApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
            builder.ConfigureSyncfusionCore();
#endif

            return builder.Build();
        }
    }
}
