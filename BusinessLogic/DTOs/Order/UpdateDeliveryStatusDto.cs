using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.DTOs.Order
{
    public class UpdateDeliveryStatusDto
    {
        [Required]
        public string DeliveryStatus { get; set; } = string.Empty;
    }
}
