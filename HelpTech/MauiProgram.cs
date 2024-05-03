using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Radzen;

namespace HelpTech
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddScoped(httpClient => new HttpClient {
                BaseAddress = new Uri("https://localhost:7219")
            });
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
