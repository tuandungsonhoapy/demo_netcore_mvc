using System.ComponentModel.DataAnnotations;

namespace demo_netcore_mvc.ObjectData
{
    public class AccountGiangVienData
    {
        [Display(Name = "Tài khoản")]
        public string Username { get; set; }

        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Họ tên giảng viên")]
        public string HoTenGV { get; set; }

        [Display(Name = "Lương")]
        public decimal Luong { get; set; }

        [Display(Name = "Khoa")]
        public string MaKhoa { get; set; }
    }
}
