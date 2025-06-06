using System.ComponentModel;

namespace QLCHNT.Const
{
    // static : có thể truy cập trực tiếp từ mọi nơi trong project
    public static class Enums
    // Kiểu dữ liệu chỉ được sử dụng để đọc, không gán giá trị được
    // Kiểu giá trị là 1 hằng số.
    {
        // Định nghĩa 1 kiểu dữ liệu là trạng thái của công việc
        // OOP : đóng gói dữ liệu  : private , public 
        public enum Status // => int
        {
            [Description("Đang chờ")]
            Pending = 1,
            [Description("Đã xác nhận")]
            Confirmed = 2,
            [Description("Đã giao")] 
            Shipped = 3,
            [Description("Đã giao")]
            Delivered = 4,
            [Description("Đã hủy")]
            Cancelled = 5
        }

        

        public enum Role
        {
            [Description("User")]
            User = 1,

            [Description("Manager")]
            Manager = 2,

            [Description("Admin")]
            Admin = 99 // int => lưu trong db là 99
        }

    }
}
