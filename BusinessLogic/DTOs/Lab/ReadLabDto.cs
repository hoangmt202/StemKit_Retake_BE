using System.ComponentModel.DataAnnotations;
using BusinessLogic.DTOs.Product;

namespace BusinessLogic.DTOs.Lab
{
    public class ReadLabDto
    {
        public int LabId { get; set; }
        public string LabName { get; set; }
        public string? Description { get; set; }
        public string? LabFileUrl { get; set; }
        public string? VideoURL { get; set; }
        public IEnumerable<ReadProductDto>? Products { get; set; }
    }
}
