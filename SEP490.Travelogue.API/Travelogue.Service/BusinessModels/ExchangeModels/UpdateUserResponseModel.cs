using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.ExchangeModels
{
    public class UpdateUserResponseModel
    {
        public Guid ExchangeId { get; set; }
        public UserExchangeResponseStatus Status { get; set; }
        public string UserResponseMessage { get; set; } = string.Empty;
    }

    public enum UserExchangeResponseStatus
    {
        AcceptedByUser = ExchangeSessionStatus.AcceptedByUser,
        RejectedByUser = ExchangeSessionStatus.RejectedByUser
    }
}