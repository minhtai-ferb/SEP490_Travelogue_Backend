namespace Travelogue.Repository.Entities.Enums;

public enum TripPlanStatus
{
    Draft = 0,
    Sketch = 1, // chưa chọn tour guide
                // đang soạn, có thể là chưa xong phần lên lịch location, nghĩ là mới có thông tin cơ bản của
                //  trip plan như tên trip plan, ngày bắt đầu, ngày kết thúc, mô tả, hình ảnh, v.v.

    // Available = 1, // đã chọn tour guide rồi,  // chờ thanh toán (hủy thanh toán thì quay về Sketch)
    // có thể book được, PHẢI hoàn thành xong thêm đã có lịch trình (trip plan location)

    Booked = 2, // book tour guide rồi
    // t kh biết, nhưng nghĩ là đã hoàn thành xong trip plan, kh thể book được, 
    // và bussiness rules sẽ không cho phép book nữa, chỉ có thể xem thông tin
}