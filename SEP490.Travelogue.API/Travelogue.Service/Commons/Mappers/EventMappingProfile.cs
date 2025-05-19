using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.EventModels;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.Commons.Mappers;
public class EventMappingProfile : Profile
{
    public EventMappingProfile()
    {
        CreateMap<EventCreateModel, Event>().ReverseMap();
        CreateMap<EventUpdateModel, Event>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<EventDataModel, Event>().ReverseMap();
        CreateMap<EventMedia, ImageModel>()
            .ForMember(dest => dest.Url, opt => opt.MapFrom<EventMediaUrlResolver>());

        CreateMap<EventMedia, MediaResponse>();

        //CreateMap<Event, EventDataModel>()
        //    .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.EventMedias));
        //CreateMap<Event, EventDataModel>()
        //    .ForMember(dest => dest.DistrictName, opt =>
        //        opt.MapFrom<EventDataNameByIdResolver>());
    }
}

//public class EventDataNameByIdResolver : IValueResolver<Event, EventDataModel, string>
//{
//    private readonly IUnitOfWork _unitOfWork;

//    public EventDataNameByIdResolver(IUnitOfWork unitOfWork)
//    {
//        _unitOfWork = unitOfWork;
//    }

//    public string Resolve(Event source, EventDataModel destination, string destMember, ResolutionContext context)
//    {
//        return _unitOfWork.GetEnumDisplayName(source.HeritageRank);
//    }
//}
