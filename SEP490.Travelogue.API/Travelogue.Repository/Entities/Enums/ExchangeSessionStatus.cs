namespace Travelogue.Repository.Entities.Enums;

public enum ExchangeSessionStatus
{
    Pending = 1,         // tour guide gửi lại, chờ user xem
    AcceptedByUser = 2,  // user đã đồng ý
    RejectedByUser = 3,  // user từ chối
    Cancelled = 4,       // tour guide hủy
    Confirmed = 5,       // booking đã được tạo từ yêu cầu
}
