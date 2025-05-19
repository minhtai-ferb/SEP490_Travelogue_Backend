using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.EventModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Commons.Mappers;
public class EventMediaUrlResolver : IValueResolver<EventMedia, ImageModel, string>
{
    private readonly IMediaCloudService _mediaCloudService;

    public EventMediaUrlResolver(IMediaCloudService mediaCloudService)
    {
        _mediaCloudService = mediaCloudService;
    }

    public string Resolve(EventMedia source, ImageModel destination, string destMember, ResolutionContext context)
    {
        return _mediaCloudService.GetFileUrl(source.MediaUrl);
    }
}

