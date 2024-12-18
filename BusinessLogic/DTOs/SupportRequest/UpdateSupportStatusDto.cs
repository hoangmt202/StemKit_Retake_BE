using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.SupportRequest
{
    public class UpdateSupportStatusDto
    {
        [Required]
        public bool SupportStatus { get; set; }
    }
}
