using System.ComponentModel.DataAnnotations;

namespace ToDoApp.ViewModels
{
    public class ForgotPasswordVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
