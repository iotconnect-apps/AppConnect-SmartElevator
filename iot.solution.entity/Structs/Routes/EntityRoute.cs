using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public class EntityRoute
    {
        public struct Name
        {
            public const string Add = "entity.add";
            public const string GetList = "entity.list";
            public const string GetById = "entity.getentitybyid";
            public const string Delete = "entity.deleteentity";
            public const string DeleteImage = "entity.deleteentityimage";
            public const string BySearch = "entity.search";
            public const string UpdateStatus = "entity.updatestatus";
        }

        public struct Route
        {
            public const string Global = "api/entity";
            public const string Manage = "manage";
            public const string GetList = "";
            public const string GetById = "{id}";
            public const string Delete = "delete/{id}";
            public const string DeleteImage = "deleteimage/{id}";
            public const string UpdateStatus = "updatestatus/{id}/{status}";
            public const string BySearch = "search";
        }
    }
}
