using System.Globalization;

namespace demo_netcore_mvc.Helper
{
    public static class FormatHelper
    {
        /// <summary>
        /// Định dạng số thành chuỗi tiền tệ VNĐ, ví dụ: 1000000 -> "1.000.000đ"
        /// </summary>
        public static string FormatCurrency(decimal value)
        {
            return string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0}đ", value);
        }

        /// <summary>
        /// Định dạng số thành chuỗi rút gọn, ví dụ:
        /// 1000 -> "1 nghìn", 1500000 -> "1,5 triệu", 2000000000 -> "2 tỷ"
        /// </summary>
        public static string FormatShortNumber(decimal value)
        {
            if (value >= 1_000_000_000)
            {
                return (value / 1_000_000_000M).ToString("0.#", CultureInfo.InvariantCulture) + " tỷ";
            }
            else if (value >= 1_000_000)
            {
                return (value / 1_000_000M).ToString("0.#", CultureInfo.InvariantCulture) + " triệu";
            }
            else if (value >= 1_000)
            {
                return (value / 1_000M).ToString("0.#", CultureInfo.InvariantCulture) + " nghìn";
            }
            else
            {
                return value.ToString("0", CultureInfo.InvariantCulture);
            }
        }
    }
}
