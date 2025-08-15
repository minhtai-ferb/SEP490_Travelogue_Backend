using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.Commons.Helpers;

public static class MediaHelpers
{
    public static MediaRequest ToMediaRequest(this MediaDto dto)
    {
        return new MediaRequest
        {
            MediaUrl = dto.MediaUrl,
            IsThumbnail = dto.IsThumbnail
        };
    }

    public static List<MediaRequest> ToMediaRequest(this List<MediaDto> dtoList)
    {
        return dtoList.Select(dto => dto.ToMediaRequest()).ToList();
    }

    public static MediaDto ToMediaDto(this MediaRequest request)
    {
        return new MediaDto
        {
            MediaUrl = request.MediaUrl,
            IsThumbnail = request.IsThumbnail
        };
    }

    public static List<MediaDto> ToMediaDto(this IEnumerable<MediaRequest> requestList)
    {
        return requestList.Select(r => r.ToMediaDto()).ToList();
    }
}
