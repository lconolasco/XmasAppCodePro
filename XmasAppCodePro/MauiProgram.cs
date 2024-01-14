using Camera.MAUI;
using Microsoft.Extensions.Logging;
using XmasAppCodePro.ViewModels;
using XmasAppCodePro.Views;

namespace XmasAppCodePro
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCameraView()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            //ViewModels
            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddTransient<AggiungeProdottoPageViewModel>();

            //Views
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<AggiungeProdottoPage>();
            builder.Services.AddTransient<AggiornaProdottoPage>();

            return builder.Build();
        }
    }
}
