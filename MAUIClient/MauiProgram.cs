using CommunityToolkit.Maui;
using MAUIClient.Models.InvoiceAggregate;
using MAUIClient.Models.RequestHelpers;
using MAUIClient.Services;
using Microsoft.Extensions.Logging;

namespace MAUIClient;

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
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        builder.Services.AddSingleton<IRestService, RestService>();
        builder.Services.AddSingleton<IInvoiceService, InvoiceService>();
        builder.Services.AddSingleton<ICustomerService, CustomerService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
