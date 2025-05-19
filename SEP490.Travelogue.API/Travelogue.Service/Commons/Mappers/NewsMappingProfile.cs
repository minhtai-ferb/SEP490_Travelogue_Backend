using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.NewsCategoryModels;
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
        CreateMap<NewsDataDetailModel, News>().ReverseMap();
        CreateMap<NewsDataModel, News>().ReverseMap();

        CreateMap<NewsCategoryDataModel, NewsCategory>().ReverseMap();
        CreateMap<NewsCategoryCreateModel, NewsCategory>().ReverseMap();
        CreateMap<NewsCategoryUpdateModel, NewsCategory>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}