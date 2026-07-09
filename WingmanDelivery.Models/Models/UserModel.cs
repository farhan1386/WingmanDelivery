namespace WingmanDelivery.Models;

public class UserModel : BaseModel
{
    public required string f_fname { get; set; }
    public required string f_lname { get; set; }
    public required string f_phone { get; set; }
    public required string f_email { get; set; }
    public required string f_password_hash { get; set; }
    public Guid f_role_uid { get; set; }
    public string? f_role_name { get; set; }
}
