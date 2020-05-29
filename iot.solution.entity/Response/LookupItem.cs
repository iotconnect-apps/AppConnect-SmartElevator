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
}
