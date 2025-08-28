using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HamamPos.Desktop.Services;
using HamamPos.Desktop.State;
using HamamPos.Shared.Dtos;
using HamamPos.Shared.Models;
using System.Collections.ObjectModel;

namespace HamamPos.Desktop.ViewModels;

/// <summary>
/// Ana pencere ViewModel'i:
/// - Odalar (Units) görünümü
/// - POS görünümü (seçilen oda için Ticket)
/// Tek pencere içinde IsInPos bayrağıyla iki görünüm arasında geçiş yapar.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly ApiClient _api;
    private readonly SessionState _session;

    // --- Odalar (sol/ilk görünüm) ---
    public ObservableCollection<ServiceUnit> Units { get; } = new();

    // --- POS görünümündeki ürün butonları ---
    public ObservableCollection<Product> Products { get; } = new();

    // UI görünüm anahtarı: false => odalar, true => POS
    [ObservableProperty] private bool isInPos;

    // Seçili oda/dolap
    [ObservableProperty] private ServiceUnit? selectedUnit;

    // Seçili oda için açık adisyonun DTO'su (satırlar, toplam, bakiye vs.)
    [ObservableProperty] private TicketDto? currentTicket;

    // Basit hata mesajı (UI'da gösterebiliriz)
    [ObservableProperty] private string? error;

    public MainViewModel(ApiClient api, SessionState session)
    {
        _api = api;
        _session = session;
    }

    /// <summary>
    /// Uygulama açılışında çağrılır: Odalar + Ürünler gelir.
    /// </summary>
    public async Task LoadAsync()
    {
        try
        {
            Error = null;

            Units.Clear();
            var units = await _api.GetAsync<List<ServiceUnit>>("/units") ?? new();
            foreach (var u in units) Units.Add(u);

            Products.Clear();
            var prods = await _api.GetAsync<List<Product>>("/products") ?? new();
            foreach (var p in prods) Products.Add(p);
        }
        catch (Exception ex)
        {
            Error = "Liste yüklenemedi: " + ex.Message;
        }
    }

    /// <summary>
    /// Oda/dolap seçildiğinde çağrılır:
    /// - Bu birim için açık adisyon yoksa açar, varsa mevcut olanı getirir.
    /// - POS görünümüne geçer.
    /// </summary>
    [RelayCommand]
    private async Task OpenUnitAsync(ServiceUnit unit)
    {
        Error = null;
        SelectedUnit = unit;

        // "Open (or get) ticket" — /tickets/open
        CurrentTicket = await _api.OpenTicketAsync(unit.Id);
        if (CurrentTicket is null)
        {
            Error = "Adisyon açılamadı.";
            return;
        }

        IsInPos = true; // POS görünümünü aç
    }

    /// <summary>
    /// POS görünümünde ürün butonuna basıldığında:
    /// - Adisyon yoksa önce açmayı dener (tekrar güvence).
    /// - Satır ekler ve güncel TicketDto'yu ekrana uygular.
    /// </summary>
    [RelayCommand]
    private async Task AddItemAsync(Product product)
    {
        Error = null;

        if (SelectedUnit is null)
        {
            Error = "Önce bir oda seçin.";
            return;
        }

        // Emniyet: CurrentTicket yoksa yeniden open yap
        if (CurrentTicket is null)
        {
            CurrentTicket = await _api.OpenTicketAsync(SelectedUnit.Id);
            if (CurrentTicket is null)
            {
                Error = "Adisyon açılamadı.";
                return;
            }
        }

        // /tickets/{ticketId}/items
        var updated = await _api.AddItemAsync(CurrentTicket.Id, product.Id, 1);
        if (updated is null)
        {
            Error = "Ürün eklenemedi.";
            return;
        }

        CurrentTicket = updated; // UI'ı tazele
    }

    /// <summary>
    /// Ödeme (Nakit/Kart). Şimdilik tamamını kapatır.
    /// Başarılıysa Ticket kapanır ve oda listesine geri dönülür.
    /// </summary>
    [RelayCommand]
    private async Task PayAsync(string method)
    {
        Error = null;
        if (CurrentTicket is null) return;

        var m = method.Equals("Card", StringComparison.OrdinalIgnoreCase)
            ? PayMethod.Card
            : PayMethod.Cash;

        // /tickets/{ticketId}/pay
        var updated = await _api.PayAsync(CurrentTicket.Id, m);
        if (updated is null)
        {
            Error = "Ödeme alınamadı.";
            return;
        }

        CurrentTicket = updated;

        // Kapandıysa geri dön (oda listesi)
        if (updated.IsClosed || updated.ClosedAtUtc is not null)
        {
            BackToUnits();
        }
    }

    /// <summary>
    /// POS'tan geri dönüp oda listesine geç.
    /// </summary>
    [RelayCommand]
    private void BackToUnits()
    {
        IsInPos = false;
        SelectedUnit = null;
        CurrentTicket = null;
        Error = null;
    }
}