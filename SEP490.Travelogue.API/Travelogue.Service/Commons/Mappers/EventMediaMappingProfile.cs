using AutoMapper;

namespace Travelogue.Service.Commons.Mappers;
public class EventMediaMappingProfile : Profile
{
    public EventMediaMappingProfile()
    {
        //CreateMap<MediaCreateModel, EventMedia>().ReverseMap();
        //CreateMap<MediaUpdateModel, EventMedia>()
        //    .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        //CreateMap<MediaDataModel, EventMedia>().ReverseMap();
    }
}