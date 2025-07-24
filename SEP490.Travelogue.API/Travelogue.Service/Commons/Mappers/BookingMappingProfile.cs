using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.OrderModels;

namespace Travelogue.Service.Commons.Mappers;

public class BookingMappingProfile : Profile
{
    public BookingMappingProfile()
    {
        // CreateMap<OrderUpdateModel, Order>()
        //     .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<BookingDataModel, Booking>().ReverseMap();
    }
}