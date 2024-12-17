using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Subcategory
{
    public class CreateSubcategoryDto
    {
       
        public string SubcategoryName { get; set; } = null!;
        public string? Description { get; set; }
    }
}
