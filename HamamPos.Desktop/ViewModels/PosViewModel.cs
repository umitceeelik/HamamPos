using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HamamPos.Desktop.Navigation;
using HamamPos.Desktop.Services;
using HamamPos.Desktop.State;
using HamamPos.Shared.Dtos;
using HamamPos.Shared.Models;
using System.Collections.ObjectModel;

namespace HamamPos.Desktop.ViewModels;

/// <summary>
/// POS ekranı: 
/// - Başta oda/dolap listesi görünür
/// - Birim seçilince adisyon açılır ve "sol: satırlar / sağ: ürün butonları" görünür
/// - Ödeme (nakit/kart) ve geri/ana menü komutları burada
/// </summary>
public partial class PosViewModel : ObservableObject
{
    private readonly ApiClient _api;
    private readonly SessionState _session;
    private readonly NavigationStore _store;
    private readonly INavigationService _nav;

    // Sol taraftaki "Odalar/Dolaplar" ızgarası
    public ObservableCollection<UnitVm> Units { get; } = new();

    // Sağ taraftaki Ürün butonları
    public ObservableCollection<Product> Products { get; } = new();

    // Görünüm kontrolü: false => Birim listesi, true => Adisyon görünümü
    [ObservableProperty] private bool isInTicket;

    // Hangi birim (oda/dolap) seçildi?
    [ObservableProperty] private ServiceUnit? selectedUnit;

    // Aktif adisyon ve satırları
    [ObservableProperty] private TicketDto? currentTicket;

    // Hata/uyarı mesajı (topta gösteriyoruz)
    [ObservableProperty] private string? error;

    [ObservableProperty] private int gridColumns = 10; // varsayılan; yüklemeden sonra hesaplarız

    public PosViewModel(ApiClient api, SessionState session, NavigationStore store, INavigationService nav)
    {
        _api = api;
        _session = session;
        _store = store;
        _nav = nav;
    }

    /// <summary> Ekran açıldığında çağır. Odalar ve ürünleri yükler. </summary>
    public async Task LoadAsync()
    {
        try
        {
            Error = null;

            // 1) Üniteleri yükle
            Units.Clear();
            var units = await _api.GetAsync<List<ServiceUnit>>("/units") ?? new();
            foreach (var u in units) Units.Add(new UnitVm(u));

            // 2) Açık adisyonları çek, dolu üniteleri işaretle
            var open = await _api.GetAsync<List<OpenTicketDto>>("/tickets/open") ?? new();
            var occupied = open.Select(o => o.ServiceUnitId).ToHashSet();
            foreach (var u in Units) u.IsOccupied = occupied.Contains(u.Id);

            // 3) Ekranı dolduracak kolon sayısını hesapla (en basit yaklaşım)
            // Kareye yakın bir doku için sqrt yaklaşımı
            GridColumns = (int)Math.Ceiling(Math.Sqrt(Units.Count));
            GridColumns = Math.Max(6, GridColumns); // çok küçük sayılar kötü görünmesin

            // başta oda listesi görünsün
            IsInTicket = false;
            SelectedUnit = null;
            CurrentTicket = null;
        }
        catch (Exception ex)
        {
            Error = "Listeler yüklenemedi: " + ex.Message;
        }
    }


    /// <summary> Oda/Dolap seçildiğinde çalışır. Yoksa adisyon açar. </summary>
    [RelayCommand]
    private async Task OpenUnitAsync(UnitVm unit)
    {
        Error = null;
        SelectedUnit = new ServiceUnit { Id = unit.Id, Name = unit.Name }; // API param için basit atama

        CurrentTicket = await _api.OpenTicketAsync(unit.Id);
        if (CurrentTicket is null) { Error = "Adisyon açılamadı."; return; }

        unit.IsOccupied = true;     // hemen boya
        OnPropertyChanged(nameof(Units)); // UI tetiklemesi gerekmez ama garanti
        IsInTicket = true;
    }


    /// <summary> Sağdaki ürün butonlarından birine tıklanınca bir satır ekler. </summary>
    [RelayCommand]
    private async Task AddItemAsync(Product product)
    {
        Error = null;

        if (SelectedUnit is null)
        {
            Error = "Önce bir oda/dolap seçin.";
            return;
        }

        if (CurrentTicket is null)
        {
            // Teoride olmaz ama korumacı davranalım
            CurrentTicket = await _api.OpenTicketAsync(SelectedUnit.Id);
            if (CurrentTicket is null) { Error = "Adisyon açılamadı."; return; }
        }

        var updated = await _api.AddItemAsync(CurrentTicket.Id, product.Id, 1);
        if (updated is null)
        {
            Error = "Ürün eklenemedi.";
            return;
        }

        CurrentTicket = updated; // UI otomatik güncellenir
    }

    /// <summary> Nakit veya Kart ödemeyi tamamlar. </summary>
    [RelayCommand]
    private async Task PayAsync(string method)
    {
        if (CurrentTicket is null) return;
        var m = method.Equals("Card", StringComparison.OrdinalIgnoreCase) ? PayMethod.Card : PayMethod.Cash;

        var updated = await _api.PayAsync(CurrentTicket.Id, m);
        if (updated is null) { Error = "Ödeme alınamadı."; return; }

        CurrentTicket = updated;

        if (updated.ClosedAtUtc is not null && SelectedUnit is not null)
        {
            var vm = Units.FirstOrDefault(u => u.Id == SelectedUnit.Id);
            if (vm != null) vm.IsOccupied = false;
            BackToUnits();
        }
    }

    /// <summary> POS içindeki geri: oda listesine döner. </summary>
    [RelayCommand]
    private void BackToUnits()
    {
        IsInTicket = false;
        SelectedUnit = null;
        CurrentTicket = null;
        Error = null;
    }

    /// <summary> Ana menüye dön (Shell/Home). </summary>
    [RelayCommand]
    private void GoHome()
    {
        var home = new HomeViewModel(_store, _nav, _api, _session);
        _nav.Navigate(home);
        _ = home.LoadAsync();
    }
}

public class UnitVm
{
    public int Id { get; }
    public string Name { get; }
    public bool IsOccupied { get; set; } // kırmızı boya için

    public UnitVm(ServiceUnit u)
    {
        Id = u.Id;
        Name = u.Name;
    }
}
