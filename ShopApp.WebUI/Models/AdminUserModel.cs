using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApp.WebUI.Models
{
    [NotMapped]
    public class AdminUserModel
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Username { get; set; }
                   
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public bool IsAdmin { get; set; }
        public bool  EmailConfirmed { get; set; }
    }
}
