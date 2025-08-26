namespace demo_netcore_mvc.Helper
{
    public static class DbHelper
    {
        // Xử lý kiểu int: nếu = 0 thì trả về DBNull.Value
        public static object ToDbValue(int value)
        {
            return value == 0 ? DBNull.Value : (object)value;
        }

        // Xử lý kiểu nullable int
        public static object ToDbValue(int? value)
        {
            return value == null || value == 0 ? DBNull.Value : (object)value;
        }

        // Xử lý cho object/string: nếu null hoặc rỗng thì trả về DBNull.Value
        public static object ToDbValue(object? value)
        {
            return value == null || string.IsNullOrWhiteSpace(value.ToString())
                ? DBNull.Value
                : value;
        }
    }
}
