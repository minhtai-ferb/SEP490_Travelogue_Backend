using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.TourGuideModels;

namespace Travelogue.Service.Commons.Mappers;

public class TourGuideMappingProfile : Profile
{
    public TourGuideMappingProfile()
    {
        // CreateMap<TourGuideCreateModel, TourGuide>().ReverseMap();
        CreateMap<TourGuideUpdateModel, TourGuide>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        // CreateMap<TourGuideUpdateWithMediaFileModel, TourGuide>()
        //     .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<TourGuide, TourGuideDataModel>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.User.Sex))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Introduction, opt => opt.MapFrom(src => src.Introduction))
            .ReverseMap();
        // CreateMap<TourGuideDetailDataModel, TourGuide>().ReverseMap();
    }
}