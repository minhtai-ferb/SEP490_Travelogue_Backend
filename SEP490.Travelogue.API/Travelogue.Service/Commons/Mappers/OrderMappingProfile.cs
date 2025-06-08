using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.OrderModels;

namespace Travelogue.Service.Commons.Mappers;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<TourGuideBookingRequest, Order>().ReverseMap();
        // CreateMap<OrderUpdateModel, Order>()
        //     .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<OrderDataModel, Order>().ReverseMap();
    }
}