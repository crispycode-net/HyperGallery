using Center.MAUI.Data;
using MediaSchnaff.Shared.DBAccess;
using MediaSchnaff.Shared.LocalData;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

namespace Center.MAUI
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
            builder.Services.AddSingleton<IDirectories, Directories>();
            builder.Services.AddDbContext<MainContext>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<WeatherForecastService>();

#if WINDOWS10_0_18362_0_OR_GREATER
            //builder.ConfigureLifecycleEvents(events =>
            //{
            //    events.AddWindows(wndLifeCycleBuilder =>
            //    {
            //        wndLifeCycleBuilder.OnWindowCreated(window =>
            //        {
            //            window.ExtendsContentIntoTitleBar = false;
            //            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            //            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            //            var _appWindow = AppWindow.GetFromWindowId(myWndId);
            //            _appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            //        });
            //    });
            //});
#endif

            return builder.Build();
        }
    }
}