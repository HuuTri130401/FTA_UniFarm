using Capstone.UniFarm.Domain.Enum;

namespace Capstone.UniFarm.Services.ViewModels.ModelResponses;

public record StationNotificationResponse
{
    public Guid StationId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Code { get; set; }
    public DateTime UpdatedAt { get; set; }
    public EnumConstants.NotificationType NotificationType { get; set; }
}