namespace demo_netcore_mvc.ObjectData
{
    public class DeTaiData
    {
        public string MaDT { get; set; }
        public string TenDT { get; set; }
        public int KinhPhi { get; set; }
        public string NoiThucTap { get; set; }
        public int? ToiDa { get; set; }
        public int? SoLuong { get; set; }
        public int? MaGV { get; set; }
        public string? HoTenGV { get; set; }
        public string? TenKhoa { get; set; }
        public string? NamHoc { get; set; }
        public byte? HocKy { get; set; }
        public string? MaKhoa { get; set; }
        public int? MaSV { get; set; }
        public string? HoTenSV { get; set; }
        public bool? IsOpen { set; get; }
    }

    public class DeTaiDetailData
    {
        public string MaDT { get; set; }
        public string TenDT { get; set; }
        public int KinhPhi { get; set; }
        public string NoiThucTap { get; set; }
        public int? ToiDa { get; set; }
        public int? SoLuong { get; set; }
        public int? MaGV { get; set; }
        public string? HoTenGV { get; set; }
        public string? TenKhoa { get; set; }
        public string? NamHoc { get; set; }
        public byte? HocKy { get; set; }
        public string? MaKhoa { get; set; }
        public int? MaSV { get; set; }
        public string? HoTenSV { get; set; }
        public decimal? KetQua { get; set; }
        public bool? IsOpen { set; get; }

        public int? NguoiHuongDan { set; get; }
    }
}
