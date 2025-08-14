using Denomination.Pages;
using Denomination.Services;
using Denomination.ViewModels;
using Microsoft.Extensions.Logging;

namespace Denomination
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder(); //This creates a builder for your .NET MAUI app.
            builder
                .UseMauiApp<App>()   // Tells MAUI which class is your main App . (App.xaml and App.xaml.cs)
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");   //Registers fonts so you can use them in XAML and C# without having to specify full file paths. 
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG    // Adds debug logging only when you’re in development mode.
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<CurrencyService>(); //This is Dependency Injection (DI) — you’re telling MAUI how to create and manage instances of these classes when other parts of your app ask for them.
            builder.Services.AddTransient<AuthService>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<LoadingPage>();     // If you navigate to LoginPage twice, it will be a fresh object each time.
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<ProfilePage>();

            return builder.Build();
        }
    }
}
