using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;

namespace iot.solution.model.Repository.Interface
{
    public interface IElevatorMaintenanceRepository : IGenericRepository<Model.ElevatorMaintenance>
    {
        Entity.SearchResult<List<Entity.ElevatorMaintenanceDetail>> List(Entity.SearchRequest request);
        Entity.ActionStatus Manage(Model.ElevatorMaintenance request);
        List<Entity.ElevatorMaintenanceResponse> GetUpComingList(Entity.ElevatorMaintenanceRequest request);
        Entity.ElevatorMaintenance Get(Guid id, DateTime currentDate, string timeZone);
    }
}
