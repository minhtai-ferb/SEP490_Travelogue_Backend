using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.NewsModels;

namespace Travelogue.Service.Commons.Mappers;

public class NewsMappingProfile : Profile
{
    public NewsMappingProfile()
    {
        CreateMap<NewsCreateModel, News>().ReverseMap();
        CreateMap<NewsUpdateModel, News>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<NewsUpdateWithMediaFileModel, News>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<NewsDataModel, News>().ReverseMap();
    }
}
