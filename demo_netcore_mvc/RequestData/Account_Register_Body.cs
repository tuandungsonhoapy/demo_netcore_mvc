namespace demo_netcore_mvc.RequestData
{
    public class Account_Register_Body
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }
    }
}
