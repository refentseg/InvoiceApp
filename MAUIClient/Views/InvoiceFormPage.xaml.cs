using MAUIClient.Models;
using MAUIClient.Models.InvoiceAggregate;
using MAUIClient.Models.RequestHelpers;
using MAUIClient.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MAUIClient.Views;

public partial class InvoiceFormPage : ContentPage
{
    IInvoiceService _invoiceService;
    ICustomerService _customerService;
    private Timer _searchTimer;

    private PagedResponse<Customer> _customers;
    public List<Customer> Items => CustomerData?.Items ?? new List<Customer>();

    public PagedResponse<Customer> CustomerData
    {
        get => _customers;
        set
        {
            _customers = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Items));
        }
    }
    public ObservableCollection<InvoiceItem> InvoiceItems { get; set; }

	public Command AddItem { get; }

    public Command<InvoiceItem> DeleteItem { get; }

    public string[] StatusOptions { get; set; }

    private InvoiceStatus _selectedStatus;
    public InvoiceStatus SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            _selectedStatus = value;
            OnPropertyChanged();
        }
    }

    public InvoiceFormPage(IInvoiceService invoiceService, ICustomerService customerService)
	{
		InitializeComponent();

        _invoiceService = invoiceService;
        _customerService = customerService;
       
        InvoiceItems = new ObservableCollection<InvoiceItem>(_invoiceService.InvoiceItems);
        InvoiceCollection.ItemsSource = InvoiceItems;

        StatusOptions = EnumHelper.GetEnumValues<InvoiceStatus>();

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


   
    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        // Avoiding too many api calls
        _searchTimer?.Dispose();
        _searchTimer = new Timer(async _ => await SearchCustomersAsync(e.NewTextValue),
                                   null, 300, Timeout.Infinite);

    }

    //Search Customers
    private async Task SearchCustomersAsync(string query)
    {
        //Checks if query is empty or has characters less than 2
        if(string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            CustomersResult.IsVisible = false;
            CustomersResult.ItemsSource = null;
            return;
        }

        CustomersResult.IsVisible = false;

        try
        {
            Debug.WriteLine($"Searching for: {query}");

            var response = await _customerService.SearchCustomersAsync(query);

            CustomerData = response ?? new PagedResponse<Customer>
            {
                Items = new List<Customer>(),
                MetaData = new MetaData { TotalCount = 0 }
            };



            Dispatcher.Dispatch(() =>
            {
                CustomersResult.ItemsSource = CustomerData.Items;
                CustomersResult.IsVisible = CustomerData.Items?.Count > 0;
            });

        }
        catch(Exception ex)
        {
            Debug.WriteLine($"FULL EXCEPTION: {ex}");

            Dispatcher.Dispatch(async () =>
            {
                await DisplayAlert("Error", ex.Message, "OK");
                CustomersResult.IsVisible = false;
            });
            
        }
    }



    //Save/Update
    private async void SaveInvoice_Clicked(object sender, EventArgs e)
    {

    }
}