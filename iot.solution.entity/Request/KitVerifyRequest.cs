﻿using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class HardwareKitDTO
    {
        public Guid Guid { get; set; }
        public Guid? KitTypeGuid { get; set; }
        public string KitCode { get; set; }
        public Guid? CompanyGuid { get; set; }
        public Guid? KitGuid { get; set; }
         public string UniqueId { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public string Tag { get; set; }
        public string AttributeName { get; set; }
        public bool? IsProvisioned { get; set; }

        //public Guid Guid { get; set; }
        //public Guid KitTypeGuid { get; set; }
        //public string KitCode { get; set; }
        //public Guid? CompanyGuid { get; set; }
        //public Guid? EntityGuid { get; set; }
        //public string UniqueId { get; set; }
        //public string Name { get; set; }
        //public string Note { get; set; }
        //public string Tag { get; set; }
        //public bool IsProvisioned { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    


    public class KitVerifyRequest 
    {
        public List<HardwareKitRequest> HardwareKits { get; set; }
        public string KitTypeGuid { get; set; }
        public string CompanyGuid { get; set; }
    }

    public class JsonSample
    {
        public List<KitVerifyRequest> HardwareKits { get; set; }
    }



}
