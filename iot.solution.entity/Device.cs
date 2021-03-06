﻿using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class Device
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid EntityGuid { get; set; }
        public Guid TemplateGuid { get; set; }
        public Guid? ParentDeviceGuid { get; set; }
        public byte? Type { get; set; }
        public string UniqueId { get; set; }
        public string Name { get; set; }
       
        public string Note { get; set; }
        public string Tag { get; set; }
        public string Image { get; set; }
        public bool IsProvisioned { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public int DeviceCount { get; set; }
    }
}
