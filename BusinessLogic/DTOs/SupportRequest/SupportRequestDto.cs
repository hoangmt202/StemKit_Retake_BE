using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.SupportRequest
{
    public class SupportRequestDto
    {
        public int SupportId { get; set; }
        public string SupportDescription { get; set; }
        public string Username { get; set; }
        public bool? SupportStatus { get; set; }
        public int OrderId { get; set; }
    }
}
