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
            builder.Services.AddScoped<MainPageViewModel>();
            builder.Services.AddScoped<AggiungeProdottoPageViewModel>();
            builder.Services.AddScoped<AggiornaProdottoPageViewModel>();
            builder.Services.AddScoped<AggiungeCategoriaPageViewModel>();
            builder.Services.AddScoped<AggiornaCategoriaPageViewModel>();

            //Views
            builder.Services.AddScoped<MainPage>();
            builder.Services.AddScoped<AggiungeProdottoPage>();
            builder.Services.AddScoped<AggiornaProdottoPage>();
            builder.Services.AddScoped<AggiungeCategoriaPage>();
            builder.Services.AddScoped<AggiornaCategoriaPage>();

            return builder.Build();
        }
    }
}
