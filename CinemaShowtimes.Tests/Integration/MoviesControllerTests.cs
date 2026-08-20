using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;

namespace CinemaShowtimes.Tests.Integration;

public class MoviesControllerTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    /*
     * This test starts the real application in memory and sends an HTTP request.
     * It checks if the "GET /api/movies" endpoint returns a 200 OK status.
     */
    [Fact]
    public async Task GetAllMovies_ShouldReturnOk()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/movies");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}