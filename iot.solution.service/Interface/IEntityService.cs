using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;
using Model = iot.solution.model.Models;
namespace iot.solution.service.Interface
{
    public interface IEntityService
    {
        List<Entity.Entity> Get();
        Entity.Entity Get(Guid id);
        Entity.ActionStatus Manage(Entity.EntityModel request);
        Entity.ActionStatus Delete(Guid id);
        Entity.ActionStatus DeleteImage(Guid id);
        Entity.SearchResult<List<Entity.EntityDetail>> List(Entity.SearchRequest request);
        Entity.ActionStatus UpdateStatus(Guid id, bool status);
        Response.EntityDetailResponse GetEntityDetail(Guid entityId);
        Entity.BaseResponse<Entity.BuildingOverviewResponse> GetBuildingOverviewDetail(Guid buildingId, DateTime currentDate, string timeZone);
        Entity.BuildingOverviewResponse GetBuildingOverview(Guid buildingId, string frequency, DateTime currentDate, string timeZone);
    }
}
