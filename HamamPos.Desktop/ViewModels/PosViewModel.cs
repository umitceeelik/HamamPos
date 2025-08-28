using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HamamPos.Desktop.Services;
using HamamPos.Desktop.State;
using HamamPos.Shared.Dtos;
using HamamPos.Shared.Models;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HamamPos.Desktop.ViewModels;

// Seçili Oda/Dolap için POS ekranı: ürün ekle, öde, güncel ticket göster.
public partial class PosViewModel : ObservableObject
{
    private readonly ApiClient _api;
    private readonly SessionState _session;

    public ServiceUnit Unit { get; }
    public ObservableCollection<Product> Products { get; } = new();
    public ObservableCollection<TicketItemDto> Items { get; } = new();

    [ObservableProperty] private int ticketId;
    [ObservableProperty] private decimal total;
    [ObservableProperty] private decimal paid;
    [ObservableProperty] private decimal balance;
    [ObservableProperty] private bool isClosed;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string? error;

    public PosViewModel(ApiClient api, SessionState session, ServiceUnit unit, IEnumerable<Product> products)
    {
        _api = api;
        _session = session;
        Unit = unit;
        foreach (var p in products) Products.Add(p);
    }

    public async Task OpenAsync()
    {
        try
        {
            IsBusy = true;
            Error = null;

            var dto = await _api.OpenTicketAsync(Unit.Id);
            if (dto is null) { Error = "Adisyon açılamadı."; return; }
            Apply(dto);
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddItemAsync(Product p)
    {
        try
        {
            IsBusy = true;
            var dto = await _api.AddItemAsync(TicketId, p.Id, 1);
            if (dto is null) return;
            Apply(dto);
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task PayCashAsync() => await PayAsync(PayMethod.Cash);

    [RelayCommand]
    private async Task PayCardAsync() => await PayAsync(PayMethod.Card);

    private async Task PayAsync(PayMethod method)
    {
        try
        {
            IsBusy = true;
            var dto = await _api.PayAsync(TicketId, method);
            if (dto is null) return;
            Apply(dto);
        }
        finally { IsBusy = false; }
    }

    private void Apply(TicketDto dto)
    {
        TicketId = dto.Id;
        Total = dto.Total;
        Paid = dto.Paid;
        Balance = dto.Balance;
        IsClosed = dto.IsClosed;

        Items.Clear();
        foreach (var i in dto.Items) Items.Add(i);
    }
}
