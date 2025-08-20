using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.TourGuideModels;
public class RejectionRequestFilter
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public RejectionRequestStatus? Status { get; set; }
    public Guid? TourGuideId { get; set; }
}
