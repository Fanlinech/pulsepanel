using System.ComponentModel.DataAnnotations;
using PulsePanel.Core.DTOs.Servers;

namespace PulsePanel.Tests.DTOs.Servers;

public class CreateServerRequestValidationTests
{
    [Fact]
    public void Validate_ReturnsSuccess_WhenRequestIsValid()
    {
        var request = new CreateServerRequest
        {
            Name = "Production API",
            Host = "api.example.com",
            Description = "Main backend",
            CheckPort = 443
        };

        var errors = Validate(request);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenNameIsEmpty()
    {
        var request = new CreateServerRequest
        {
            Name = "",
            Host = "api.example.com"
        };

        var errors = Validate(request);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(CreateServerRequest.Name)));
    }

    [Fact]
    public void Validate_ReturnsError_WhenNameIsTooLong()
    {
        var request = new CreateServerRequest
        {
            Name = new string('a', 101),
            Host = "api.example.com"
        };

        var errors = Validate(request);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(CreateServerRequest.Name)));
    }

    [Fact]
    public void Validate_ReturnsError_WhenHostIsEmpty()
    {
        var request = new CreateServerRequest
        {
            Name = "Production API",
            Host = ""
        };

        var errors = Validate(request);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(CreateServerRequest.Host)));
    }

    [Fact]
    public void Validate_ReturnsError_WhenHostIsTooLong()
    {
        var request = new CreateServerRequest
        {
            Name = "Production API",
            Host = new string('h', 256)
        };

        var errors = Validate(request);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(CreateServerRequest.Host)));
    }

    [Fact]
    public void Validate_ReturnsError_WhenDescriptionIsTooLong()
    {
        var request = new CreateServerRequest
        {
            Name = "Production API",
            Host = "api.example.com",
            Description = new string('d', 501)
        };

        var errors = Validate(request);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(CreateServerRequest.Description)));
    }

    [Fact]
    public void Validate_ReturnsError_WhenCheckPortIsZero()
    {
        var request = new CreateServerRequest
        {
            Name = "Production API",
            Host = "api.example.com",
            CheckPort = 0
        };

        var errors = Validate(request);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(CreateServerRequest.CheckPort)));
    }

    private static List<ValidationResult> Validate(CreateServerRequest request)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(request);

        Validator.TryValidateObject(request, context, results, validateAllProperties: true);

        return results;
    }
}
