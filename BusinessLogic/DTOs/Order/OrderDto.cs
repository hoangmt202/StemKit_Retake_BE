namespace BusinessLogic.DTOs.Order
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string CustomerUsername { get; set; }
        public DateOnly OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
