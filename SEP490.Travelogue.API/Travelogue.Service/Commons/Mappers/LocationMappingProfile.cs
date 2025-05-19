using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.LocationModels;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.Services;

namespace Travelogue.Service.Commons.Mappers;
public class LocationMappingProfile : Profile
{
    public LocationMappingProfile()
    {
        CreateMap<LocationCreateModel, Location>().ReverseMap();
        CreateMap<LocationUpdateModel, Location>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<LocationUpdateWithMediaFileModel, Location>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<Location, LocationDataModel>()
            .ForMember(dest => dest.HeritageRankName, opt =>
                opt.MapFrom<HeritageRankDisplayNameResolver>());

        CreateMap<LocationMedia, MediaResponse>();
    }
}

public class HeritageRankDisplayNameResolver : IValueResolver<Location, LocationDataModel, string>
{
    private readonly IEnumService _enumService;

    public HeritageRankDisplayNameResolver(IEnumService enumService)
    {
        _enumService = enumService;
    }

    public string Resolve(Location source, LocationDataModel destination, string destMember, ResolutionContext context)
    {
        return _enumService.GetEnumDisplayName(source.HeritageRank);
    }
}
