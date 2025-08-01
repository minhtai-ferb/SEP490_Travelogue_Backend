namespace Travelogue.Repository.Entities.Enums;

public enum BookingStatus
{
    Pending = 0,          // Đang chờ xác nhận
    Confirmed = 1,        // Đã xác nhận bởi tour guide
    Rejected = 2,         // Bị từ chối bởi tour guide
    Cancelled = 3,        // Bị hủy bởi khách hoặc hệ thống
    Completed = 4,        // Tour đã hoàn thành
    Expired = 5           // Đơn đặt tour hết hạn mà không được xử lý
}

