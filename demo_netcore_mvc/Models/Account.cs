namespace demo_netcore_mvc.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public GiangVien? GiangVien { get; set; }
        public SinhVien? SinhVien { get; set; }
    }
}
