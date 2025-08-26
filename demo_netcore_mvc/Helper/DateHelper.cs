namespace demo_netcore_mvc.Helper
{
    public static class DateHelper
    {
        /// <summary>
        /// Format ngày theo định dạng dd/MM/yyyy (hoặc định dạng bạn muốn).
        /// Nếu null => trả về string.Empty
        /// </summary>
        public static string FormatDate(DateTime? date, string format = "dd/MM/yyyy")
        {
            if (!date.HasValue)
                return string.Empty;

            return date.Value.ToString(format);
        }

        /// <summary>
        /// Trả về DateTime chỉ có phần ngày (giờ = 00:00:00).
        /// </summary>
        public static DateTime ToDateOnly(DateTime date)
        {
            return date.Date; // tự động set time = 00:00:00
        }

        /// <summary>
        /// Thử parse chuỗi ngày (dd/MM/yyyy) thành DateTime.
        /// </summary>
        public static DateTime? ParseDate(string dateStr, string format = "dd/MM/yyyy")
        {
            if (string.IsNullOrWhiteSpace(dateStr))
                return null;

            if (DateTime.TryParseExact(dateStr, format,
                                       System.Globalization.CultureInfo.InvariantCulture,
                                       System.Globalization.DateTimeStyles.None,
                                       out var result))
            {
                return result;
            }

            return null;
        }
    }
}
