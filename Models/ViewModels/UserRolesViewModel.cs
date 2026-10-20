namespace RaceDay7.Models.ViewModels
{
    public class UserRolesControllerViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public System.Collections.Generic.IList<string> Roles { get; set; } = new System.Collections.Generic.List<string>();
    }
}