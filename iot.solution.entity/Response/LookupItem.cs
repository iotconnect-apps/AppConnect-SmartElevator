using System;

namespace iot.solution.entity
{
    public class LookupItem
    {
        private string _Value;
        public string Value { get { return _Value.ToLower(); } set { _Value = value; } }
        public string Text { get; set; }
    }
    public class LookupItemWithStatus : LookupItem
    {
        public bool? IsActive { get; set; }
    }
    public class ElevatorLookup
    {
        public Guid Value { get; set; }
        public string Text { get; set; }
    }
    public class BuildingElevatorLookup
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public class TagLookup
    {
        public string tag { get; set; }
        public bool templateTag { get; set; }
    }

    public class ElevatorLookupDetail
    {
        public Guid? Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid EntityGuid { get; set; }
        public Guid TemplateGuid { get; set; }
        public string UniqueId { get; set; }
        public string Name { get; set; }
        public string Building { get; set; }
        public string Wing { get; set; }
    }
}
