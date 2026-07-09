using WingmanDelivery.Models.Enums;

namespace WingmanDelivery.Models;

public class NotificationModel : BaseModel
{
    public Guid f_user_uid { get; set; }
    public required string f_message { get; set; }
    public NotificationType f_type { get; set; }
    public NotificationStatus f_status { get; set; }
}