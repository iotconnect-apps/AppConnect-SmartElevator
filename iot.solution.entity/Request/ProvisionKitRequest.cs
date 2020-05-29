using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace iot.solution.entity
{
    public class ProvisionKitRequest
    {
        public IFormFile ImageFile { get; set; }
        public Guid BuildingGuid { get; set; }
        public Guid WingGuid { get; set; }
        public string KitCode { get; set; }
        public string Name { get; set; }
        public string UniqueId { get; set; }
        public string Specification { get; set; }
        public string Description { get; set; }
    }
}
