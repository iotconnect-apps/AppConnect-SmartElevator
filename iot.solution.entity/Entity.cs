﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class Entity
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid? ParentEntityGuid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public Guid? StateGuid { get; set; }
        public Guid? CountryGuid { get; set; }
        public string Image { get; set; }
        public bool Isactive { get; set; }
        public bool Isdeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
       
    }
    public class EntityModel : Entity
    {
        public IFormFile ImageFile { get; set; }

    }
    public class EntityDetail : Entity
    {
        public int ElevatorCount { get; set; }
        public int WingCount { get; set; }

    }
}
