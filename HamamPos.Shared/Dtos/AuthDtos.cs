namespace HamamPos.Shared.Dtos;
public record LoginRequest(string Username, string Password);
public record LoginResponse(string Username, string Role, string Token);
