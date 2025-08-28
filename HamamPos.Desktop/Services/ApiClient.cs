// API ile konuşan ortak sınıf. Tüm HTTP istekleri tek yerde toplansın diye.
// - BaseAddress: API adresi (geliştirirken localhost, üretimde IP/DNS)
// - Login: JWT alır ve SessionState'e yazar
// - ApplyAuthHeader: token varsa Authorization header'ını ekler
// - İleride: /units, /products, /tickets gibi çağrıları da burada toplayacağız.

using HamamPos.Desktop.State;
using HamamPos.Shared.Dtos;
using HamamPos.Shared.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HamamPos.Desktop.Services;

public partial class ApiClient
{
    private readonly HttpClient _http;
    private readonly SessionState _session;

    public string BaseUrl { get; }

    public ApiClient(SessionState session, string? baseUrl = null)
    {
        _session = session;
        BaseUrl = baseUrl ?? "http://localhost:5005";
        _http = new HttpClient { BaseAddress = new Uri(BaseUrl) };
    }

    // Her istekten önce token'ı header'a koy
    private void ApplyAuthHeader()
    {
        _http.DefaultRequestHeaders.Authorization = _session.IsAuthenticated
            ? new AuthenticationHeaderValue("Bearer", _session.Token)
            : null;
    }

    public async Task<LoginResponse?> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        var req = new LoginRequest(username, password);

        // POST /auth/login
        var res = await _http.PostAsJsonAsync("/auth/login", req, ct);
        if (!res.IsSuccessStatusCode) return null;

        var body = await res.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: ct);
        if (body is null) return null;

        // Session'ı doldur + sonraki isteklerde header ekleyelim
        _session.Set(body.Username, body.Role, body.Token);
        ApplyAuthHeader();
        return body;
    }

    // Örnek GET (ileride POS listeleri için kullanacağız)
    public async Task<T?> GetAsync<T>(string path, CancellationToken ct = default)
    {
        ApplyAuthHeader();
        return await _http.GetFromJsonAsync<T>(path, ct);
    }

    // Ürün listesi
    public async Task<List<Product>?> GetProductsAsync(CancellationToken ct = default)
        => await GetAsync<List<Product>>("/products", ct);

    // Birim listesi
    public async Task<List<ServiceUnit>?> GetUnitsAsync(CancellationToken ct = default)
        => await GetAsync<List<ServiceUnit>>("/units", ct);

    //public async Task<TicketDto?> OpenTicketAsync(int serviceUnitId, CancellationToken ct = default)
    //{
    //    ApplyAuthHeader();
    //    var req = new CreateTicketRequest(serviceUnitId);
    //    var res = await _http.PostAsJsonAsync("/tickets/open", req, ct);
    //    if (!res.IsSuccessStatusCode) return null;
    //    return await res.Content.ReadFromJsonAsync<TicketDto>(cancellationToken: ct);
    //}

    //public async Task<TicketDto?> AddItemAsync(int ticketId, int productId, decimal quantity = 1, CancellationToken ct = default)
    //{
    //    ApplyAuthHeader();
    //    var req = new AddItemRequest(ticketId, productId, quantity);
    //    var res = await _http.PostAsJsonAsync($"/tickets/{ticketId}/items", req, ct);
    //    if (!res.IsSuccessStatusCode) return null;
    //    return await res.Content.ReadFromJsonAsync<TicketDto>(cancellationToken: ct);
    //}

    //public async Task<TicketDto?> PayAsync(int ticketId, PayMethod method, CancellationToken ct = default)
    //{
    //    ApplyAuthHeader();
    //    var req = new PayTicketRequest(ticketId, method);
    //    var res = await _http.PostAsJsonAsync($"/tickets/{ticketId}/pay", req, ct);
    //    if (!res.IsSuccessStatusCode) return null;
    //    return await res.Content.ReadFromJsonAsync<TicketDto>(cancellationToken: ct);
    //}

    public async Task<TicketDto?> OpenTicketAsync(int serviceUnitId, CancellationToken ct = default)
    {
        ApplyAuthHeader();
        var res = await _http.PostAsJsonAsync("/tickets/open", new OpenTicketRequest(serviceUnitId), ct);
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<TicketDto>(cancellationToken: ct);
    }

    public async Task<TicketDto?> AddItemAsync(int ticketId, int productId, decimal qty, CancellationToken ct = default)
    {
        ApplyAuthHeader();
        var res = await _http.PostAsJsonAsync($"/tickets/{ticketId}/items", new AddItemRequest(ticketId, productId, qty), ct);
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<TicketDto>(cancellationToken: ct);
    }

    public async Task<TicketDto?> PayAsync(int ticketId, PayMethod method, CancellationToken ct = default)
    {
        ApplyAuthHeader();
        var res = await _http.PostAsJsonAsync($"/tickets/{ticketId}/pay", new PayTicketRequest(ticketId, method), ct);
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<TicketDto>(cancellationToken: ct);
    }

    public async Task<TicketDto?> GetTicketAsync(int ticketId, CancellationToken ct = default)
    {
        ApplyAuthHeader();
        return await _http.GetFromJsonAsync<TicketDto>($"/tickets/{ticketId}", ct);
    }
}
