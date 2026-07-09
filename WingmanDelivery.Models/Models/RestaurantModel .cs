namespace WingmanDelivery.Models;

public class RestaurantModel : BaseModel
{
    public required string f_name { get; set; }
    public required string f_address { get; set; }
    public string? f_contact { get; set; }
}