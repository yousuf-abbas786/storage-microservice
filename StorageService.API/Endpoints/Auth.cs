using Microsoft.AspNetCore.Mvc;
using StorageService.API.Configs;
using StorageService.API.Configs.Results;
using StorageService.Application.DTOs;
using StorageService.Application.Services.Interfaces;

namespace StorageService.API.Endpoints
{
    public class Auth : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            var group = app.MapGroup(this);

            group.MapPost("/login", LoginAsync)
                 .WithName("Login")
                 .Produces<LoginResponse>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status401Unauthorized);

            group.MapPost("/register", RegisterAsync)
                 .WithName("Register")
                 .Produces<RegisterResponse>(StatusCodes.Status201Created)
                 .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        private static async Task<IResult> LoginAsync(
            [FromBody] LoginRequest request,
            [FromServices] IAuthService authService,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return Results.BadRequest("Username and password are required.");

            var result = await authService.LoginAsync(request.Username, request.Password, request.TenantId, ct);

            if (result == null)
                return Results.Unauthorized();

            return Results.Ok(result);
        }

        private static async Task<IResult> RegisterAsync(
            [FromBody] RegisterRequest request,
            [FromServices] IAuthService authService,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || 
                string.IsNullOrWhiteSpace(request.Password) || 
                string.IsNullOrWhiteSpace(request.TenantId))
                return Results.BadRequest("Username, password, and tenantId are required.");

            try
            {
                var result = await authService.RegisterAsync(request, ct);
                return Results.Extensions.APIResult_Created($"/api/Auth/login", result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
    }
}

