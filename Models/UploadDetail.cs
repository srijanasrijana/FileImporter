using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserManagementApp.Models
{
    public class UploadDetail
    {
        public int Id { get; set; } 
        public int TerminalId { get; set; }
        public int SerialNo { get; set; }
        public int MerchantId { get; set; }
        public string Description { get; set; } 
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string State { get; set; }
               
    }
}