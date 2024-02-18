namespace Capstone.UniFarm.API.Helpers
{
    public sealed record ErrorResponse(int StatusCode, string StatusPhrase, dynamic Errors, DateTime Timestamp);
}
