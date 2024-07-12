using System.ComponentModel.DataAnnotations;

namespace asp_login.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "帳號是必填項。")]
    public required string Username { get; set; }
    [Required(ErrorMessage = "密碼是必填項。")]
    public required string Password { get; set; }
}