using MAUIClient.Models.InvoiceAggregate;
using MAUIClient.Services;
using System.Security.Cryptography.X509Certificates;

namespace MAUIClient.Views;

[QueryProperty(nameof(Invoice), "Invoice")]
public partial class InvoiceListPage : ContentPage
{

	IInvoiceService _invocieService;
	Invoice _invoice;
        
	public Invoice Invoice
	{
		get => _invoice;
		set
		{
			_invoice = value;
			OnPropertyChanged();
		}
	}
}
