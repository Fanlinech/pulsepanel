using System.ComponentModel.DataAnnotations;
using PulsePanel.Core.DTOs.Servers;

namespace PulsePanel.Tests.DTOs.Servers;

public class UpdateServerRequestValidationTests
{
    [Fact]
    public void Validate_ReturnsSuccess_WhenRequestIsValid()
    {
        var request = new UpdateServerRequest
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
        var request = new UpdateServerRequest
        {
            Name = "",
            Host = "api.example.com"
        };

        var errors = Validate(request);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(UpdateServerRequest.Name)));
    }

    [Fact]
    public void Validate_ReturnsError_WhenNameIsTooLong()
    {
        var request = new UpdateServerRequest
        {
            Name = new string('a', 101),
            Host = "api.example.com"
        };

        var errors = Validate(request);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(UpdateServerRequest.Name)));
    }

    [Fact]
    public void Validate_ReturnsError_WhenHostIsEmpty()
    {
        var request = new UpdateServerRequest
        {
            Name = "Production API",
            Host = ""
        };

        var errors = Validate(request);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(UpdateServerRequest.Host)));
    }

    [Fact]
    public void Validate_ReturnsError_WhenHostIsTooLong()
    {
        var request = new UpdateServerRequest
        {
            Name = "Production API",
            Host = new string('h', 256)
        };

        var errors = Validate(request);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(UpdateServerRequest.Host)));
    }

    [Fact]
    public void Validate_ReturnsError_WhenDescriptionIsTooLong()
    {
        var request = new UpdateServerRequest
        {
            Name = "Production API",
            Host = "api.example.com",
            Description = new string('d', 501)
        };

        var errors = Validate(request);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(UpdateServerRequest.Description)));
    }

    [Fact]
    public void Validate_ReturnsError_WhenCheckPortIsZero()
    {
        var request = new UpdateServerRequest
        {
            Name = "Production API",
            Host = "api.example.com",
            CheckPort = 0
        };

        var errors = Validate(request);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(UpdateServerRequest.CheckPort)));
    }

    private static List<ValidationResult> Validate(UpdateServerRequest request)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(request);

        Validator.TryValidateObject(request, context, results, validateAllProperties: true);

        return results;
    }
}
