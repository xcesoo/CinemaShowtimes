using System.Net;
using System.Net.Http.Json;
using Application.Commands.Reservations;
using Application.DTOs;
using CinemaShowtimes.Infrastructure.Persistence;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaShowtimes.Tests.Integration;

public class ReservationsControllerTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private record TestShowtimeDto(Guid Id);
    
    private readonly HttpClient _client = factory.CreateClient();
    private readonly List<Guid> _createdReservationIds = new();
    
    public Task InitializeAsync() => Task.CompletedTask;
    
    public async Task DisposeAsync()
    {
        if (_createdReservationIds.Count == 0) return;
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CinemaDbContext>();
        
        await dbContext.Reservations
            .Where(r => _createdReservationIds.Contains(r.Id))
            .ExecuteDeleteAsync();    
    }

    /*
     * This integration test verifies the standard sequential user flow.
     * It successfully reserves a seat, tries to reserve the SAME seat sequentially 
     * (expecting a 400 Bad Request domain validation error), and then confirms the ticket.
     */
    [Fact]
    public async Task SequentialReservationFlow_ShouldReturnBadRequestOnDoubleBooking()
    {
        var showtimesResponse = await _client.GetAsync("/api/showtimes");
        var showtimes = await showtimesResponse.Content.ReadFromJsonAsync<List<TestShowtimeDto>>();
        var showtimeId = showtimes!.First().Id;

        var reserveCommand = new ReserveSeatsCommand(
            showtimeId, 
            new[] { new ReserveSeatRequest(5, 5) });

        var reserveResponse = await _client.PostAsJsonAsync("/api/reservations", reserveCommand);
        var resultDto = await reserveResponse.Content.ReadFromJsonAsync<ReservationResultDto>();
        _createdReservationIds.Add(resultDto!.ReservationId);
        
        reserveResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var sequentialDoubleBookingResponse = await _client.PostAsJsonAsync("/api/reservations", reserveCommand);
        
        sequentialDoubleBookingResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var confirmResponse = await _client.PatchAsync($"/api/reservations/{resultDto!.ReservationId}/confirm", null);
        confirmResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    /*
     * This integration test verifies concurrency (Race Condition).
     * It sends two exact same booking requests at the exact same time using Task.WhenAll.
     * The database's Serializable isolation level should catch the collision,
     * returning 201 Created for the winner and 409 Conflict for the loser.
     */
    [Fact]
    public async Task ConcurrentReservationFlow_ShouldReturnConflictForRaceCondition()
    {
        var showtimesResponse = await _client.GetAsync("/api/showtimes");
        var showtimes = await showtimesResponse.Content.ReadFromJsonAsync<List<TestShowtimeDto>>();
        var showtimeId = showtimes!.First().Id;

        var reserveCommand = new ReserveSeatsCommand(
            showtimeId, 
            new[] { new ReserveSeatRequest(6, 6) });

        var task1 = _client.PostAsJsonAsync("/api/reservations", reserveCommand);
        var task2 = _client.PostAsJsonAsync("/api/reservations", reserveCommand);

        var responses = await Task.WhenAll(task1, task2);


        foreach (var response in responses)
        {
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var resultDto = await response.Content.ReadFromJsonAsync<ReservationResultDto>();
                _createdReservationIds.Add(resultDto!.ReservationId);
            }
        }
        var statusCodes = responses.Select(r => r.StatusCode).ToList();

        statusCodes.Should().Contain(HttpStatusCode.Created);
        
        statusCodes.Should().Contain(code => code == HttpStatusCode.Conflict);
        
    }
}