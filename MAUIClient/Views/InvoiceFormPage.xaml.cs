using MAUIClient.Models;
using MAUIClient.Models.InvoiceAggregate;
using MAUIClient.Services;
using System.Collections.ObjectModel;

namespace MAUIClient.Views;

public partial class InvoiceFormPage : ContentPage
{
    IInvoiceService _invoiceService;
    public ObservableCollection<InvoiceItem> InvoiceItems { get; set; }

	public Command AddItem { get; }

    public Command<InvoiceItem> DeleteItem { get; }

    public InvoiceFormPage(IInvoiceService invoiceService)
	{
		InitializeComponent();

        _invoiceService = invoiceService;

        InvoiceItems = new ObservableCollection<InvoiceItem>(_invoiceService.InvoiceItems);
        InvoiceCollection.ItemsSource = InvoiceItems;

        // Add Item
        AddItem = new Command(() =>
        {
            var newItem = new InvoiceItem();
            InvoiceItems.Add(newItem);
            _invoiceService.InvoiceItems.Add(newItem);
        });

        // Delete Item
        DeleteItem = new Command<InvoiceItem>(async (item) =>
        {
            if (item != null && InvoiceItems.Contains(item))
            {
                try
                {
                    _invoiceService.InvoiceItems.Remove(item);
                    InvoiceItems.Remove(item);

                    OnPropertyChanged(nameof(InvoiceItems));
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failde to delete item: {ex.Message}", "OK");
                }
                
            }
        });
        BindingContext = this;
    }

    private void OnExistingCustomerChanged(object sender, EventArgs e)
	{
		// Check if Checkbox is checked
		if(ExistingCustomerCheckBox.IsChecked == true)
		{
			ExistingCustomerPanel.IsVisible = true;
			AddCustomerPanel.IsVisible = false;
		}
		else 
		{
            ExistingCustomerPanel.IsVisible = false;
            AddCustomerPanel.IsVisible = true;
        }
	}
}