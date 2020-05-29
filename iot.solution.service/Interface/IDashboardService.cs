using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;

namespace iot.solution.service.Interface
{
    public interface IDashboardService
    {
        List<Entity.LookupItem> GetBuildingsLookup();
        Entity.BaseResponse<Entity.DashboardOverviewResponse> GetOverview();

        Entity.BaseResponse<Entity.BuildingOverviewResponse> GetBuildingOverview(Guid buildingId, string frequency);

    }
}
