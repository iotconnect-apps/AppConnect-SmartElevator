﻿using System;
using System.ComponentModel.DataAnnotations;

namespace iot.solution.entity
{
    public class Role
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        //public string Name { get; set; }
        //public string Description { get; set; }
        private string _name;
        
        [Required]
        public string Name
        {
            get { return _name; }
            set { _name = value.Trim(); }
        }
        private string _description;
       
        [Required]
        public string Description
        {
            get { return _description; }
            set { _description = value.Trim(); }
        }
        public bool IsAdminRole { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
