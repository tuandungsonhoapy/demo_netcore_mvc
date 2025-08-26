namespace demo_netcore_mvc.ObjectData
{
    public class GiangVienData
    {
        public int MaGV { get; set; }
        public required string HoTenGV { get; set; }
        public decimal Luong { get; set; }
        public required string MaKhoa { get; set; }

        public required string TenKhoa { get; set; }
        public string? DienThoai { get; set; }
    }
}
