using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;
using Model = iot.solution.model.Models;

namespace iot.solution.service.Interface
{
    public interface IElevatorMaintenanceService
    {
        List<Entity.ElevatorMaintenance> Get();
        Entity.ElevatorMaintenance Get(Guid id);
        Entity.ActionStatus Manage(Entity.ElevatorMaintenance request);
        Entity.ActionStatus Delete(Guid id);
        Entity.SearchResult<List<Entity.ElevatorMaintenanceDetail>> List(Entity.SearchRequest request);
        Entity.ActionStatus UpdateStatus(Guid id, string status);
        List<Entity.ElevatorMaintenanceResponse> GetUpComingList(Entity.ElevatorMaintenanceRequest request);
    }
}
