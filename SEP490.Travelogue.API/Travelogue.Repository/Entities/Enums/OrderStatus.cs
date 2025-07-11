namespace Travelogue.Repository.Entities.Enums;

public enum BookingStatus
{
    Pending = 0,          // Đang chờ xác nhận
    Confirmed = 1,        // Đã xác nhận bởi tour guide
    Rejected = 2,         // Bị từ chối bởi tour guide
    Cancelled = 3,        // Bị hủy bởi khách hoặc hệ thống
    InProgress = 4,       // Tour đang diễn ra
    Completed = 5,        // Tour đã hoàn thành
    NoShow = 6,           // Khách không đến
    Expired = 7           // Đơn đặt tour hết hạn mà không được xử lý
}

