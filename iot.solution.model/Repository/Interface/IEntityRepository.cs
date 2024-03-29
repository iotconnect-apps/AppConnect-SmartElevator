﻿using iot.solution.entity;
using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;

namespace iot.solution.model.Repository.Interface
{
    public interface IEntityRepository : IGenericRepository<Model.Entity>
    {
        Entity.SearchResult<List<Entity.EntityDetail>> List(Entity.SearchRequest request);
        List<Entity.LookupItem> GetLookup(Guid companyId);
        List<Entity.LookupItem> GetWingLookup(Guid companyId);
        ActionStatus Manage(Model.Entity request);
        Entity.BaseResponse<List<Entity.BuildingOverviewResponse>> GetBuildingOverview(Guid buildingId, string frequency, DateTime currentDate, string timeZone);


    }
}
