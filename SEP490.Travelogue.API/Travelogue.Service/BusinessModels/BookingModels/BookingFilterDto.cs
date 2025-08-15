using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.Commons.Helpers;

namespace Travelogue.Service.BusinessModels.BookingModels;

// [ModelBinder(BinderType = typeof(CamelCaseQueryModelBinder))]
public class BookingFilterDto
{
    public BookingStatus? Status { get; set; }
    public BookingType? BookingType { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
}