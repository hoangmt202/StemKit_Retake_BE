namespace BusinessLogic.DTOs.Order
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string CustomerUsername { get; set; }
        public DateOnly OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string DeliveryStatus { get; set; }
        public bool SupportStatus { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = new List<OrderDetailDto>();
    }
}
