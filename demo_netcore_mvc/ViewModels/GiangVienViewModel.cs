using demo_netcore_mvc.Dto;

namespace demo_netcore_mvc.ViewModels
{
    public class GiangVienViewModel
    {
        public string? MaKhoa { get; set; }

        public List<GiangVienDto> GiangViens { get; set; }
    }
}
