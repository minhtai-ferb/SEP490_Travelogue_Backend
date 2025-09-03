using AutoMapper;

namespace Travelogue.Service.Commons.Mappers;

public class WorkshopMappingProfile : Profile
{
    public WorkshopMappingProfile()
    {
        // Workshop to WorkshopDataModel
        // CreateMap<Workshop, WorkshopResponseDto>()
        //     .ForMember(dest => dest.CraftVillageName, 
        //         opt => opt.MapFrom(src => src.CraftVillage != null ? src.CraftVillage.Username : string.Empty))
        //     .ForMember(dest => dest.Activities, 
        //         opt => opt.MapFrom(src => src.WorkshopActivities))
        //     .ForMember(dest => dest.Schedules, 
        //         opt => opt.MapFrom(src => src.WorkshopSchedules))
        //     .ForMember(dest => dest.ImageUrls, 
        //         opt => opt.MapFrom(src => src.WorkshopImages.Select(i => i.ImageUrl).ToList()));

        // // WorkshopActivity to WorkshopActivityDataModel
        // CreateMap<WorkshopActivity, WorkshopActivityDataModel>();

        // // WorkshopSchedule to WorkshopScheduleDataModel
        // CreateMap<WorkshopSchedule, WorkshopScheduleDataModel>();

        // // Appeal to AppealDataModel
        // CreateMap<Appeal, AppealDataModel>();
    }
}