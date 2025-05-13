using MAUIClient.Services;
using MAUIClient.Views;
using Microsoft.Extensions.Logging;

namespace MAUIClient;

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
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        builder.Services.AddSingleton<IRestService, RestService>();
        builder.Services.AddSingleton<IInvoiceService, InvoiceService>();

		builder.Services.AddSingleton<InvoiceListPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
