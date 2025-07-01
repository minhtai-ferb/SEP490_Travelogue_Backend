using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.CraftVillageModels;
using Travelogue.Service.BusinessModels.CuisineModels;
using Travelogue.Service.BusinessModels.HistoricalLocationModels;
using Travelogue.Service.BusinessModels.HotelModels;
using Travelogue.Service.BusinessModels.LocationModels;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.Services;

namespace Travelogue.Service.Commons.Mappers;

public class LocationMappingProfile : Profile
{
    public LocationMappingProfile()
    {
        CreateMap<LocationCreateModel, Location>().ReverseMap()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<LocationUpdateModel, Location>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<LocationUpdateWithMediaFileModel, Location>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<HistoricalLocation, LocationDataModel>()
            .ForMember(dest => dest.HeritageRankName, opt =>
                opt.MapFrom<HeritageRankDisplayNameResolver>());

        CreateMap<Location, LocationDataModel>().ReverseMap();
        CreateMap<Location, HotelCreateModel>()
            .ReverseMap()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Location, LocationDataDetailModel>().ReverseMap();

        CreateMap<LocationMedia, MediaResponse>();

        CreateMap<HistoricalLocationDataModel, HistoricalLocation>().ReverseMap();
        CreateMap<HistoricalLocationCreateModel, HistoricalLocation>();
        CreateMap<CraftVillageCreateModel, CraftVillage>();
        CreateMap<HotelCreateModel, Hotel>();
        CreateMap<CuisineCreateModel, Cuisine>();


    }
}

public class HeritageRankDisplayNameResolver : IValueResolver<HistoricalLocation, LocationDataModel, string>
{
    private readonly IEnumService _enumService;

    public HeritageRankDisplayNameResolver(IEnumService enumService)
    {
        _enumService = enumService;
    }

    public string Resolve(HistoricalLocation source, LocationDataModel destination, string destMember, ResolutionContext context)
    {
        return _enumService.GetEnumDisplayName(source.HeritageRank);
    }
}
