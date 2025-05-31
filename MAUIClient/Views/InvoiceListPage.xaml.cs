using MAUIClient.Models.InvoiceAggregate;
using MAUIClient.Models.RequestHelpers;
using MAUIClient.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace MAUIClient.Views;

public partial class InvoiceListPage : ContentPage
{

	IInvoiceService _invoiceService;
    private PagedResponse<Invoice> _invoices;
    public List<Invoice> Items => InvoiceData?.Items ?? new List<Invoice>();

    public PagedResponse<Invoice> InvoiceData
	{
		get => _invoices;
		set
		{
            _invoices = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Items));
        }
	}

	public InvoiceListPage(IInvoiceService invoiceService)
    {
        InitializeComponent();
        _invoiceService = invoiceService;
        BindingContext = this;

        this.Appearing += async (s, e) => await LoadInvoices();
    }

    private async Task LoadInvoices()
    {
        try
        {
            Debug.WriteLine("Starting to load invoices...");
            var response = await _invoiceService.GetInvoicesAsync();

            InvoiceData = response ?? new PagedResponse<Invoice>
            {
                Items = new List<Invoice>(),
                MetaData = new MetaData { TotalCount = 0 }
            };
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load invoices: {ex.Message}", "OK");
            Debug.WriteLine($"Error loading invoices: {ex}");
        }
    }

    private async void AddInvoice_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync("invoiceform");
        }
        catch(Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }
}
