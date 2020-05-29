using component.helper;
using component.logger;
using iot.solution.common;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;

namespace iot.solution.service.Implementation
{
    public class ElevatorMaintenanceService : IElevatorMaintenanceService
    {
        private readonly IElevatorMaintenanceRepository _elevatorMaintenanceRepository;
        private readonly IEntityRepository _entityRepository;


        private readonly IotConnectClient _iotConnectClient;
        private readonly ILogger _logger;

        public ElevatorMaintenanceService(IElevatorMaintenanceRepository entityMaintenanceRepository, IEntityRepository entityRepository,ILogger logger)
        {
            _logger = logger;
            _elevatorMaintenanceRepository = entityMaintenanceRepository;
            _entityRepository = entityRepository;
            _iotConnectClient = new IotConnectClient(SolutionConfiguration.BearerToken, SolutionConfiguration.Configuration.EnvironmentCode, SolutionConfiguration.Configuration.SolutionKey);
        }
      
        public List<Entity.ElevatorMaintenance> Get()
        {
            try
            {

                return _elevatorMaintenanceRepository.GetAll().Select(p => Mapper.Configuration.Mapper.Map<Entity.ElevatorMaintenance>(p)).ToList();
            }
            catch (Exception ex)
            {

                _logger.Error(Constants.ACTION_EXCEPTION, "ElevatorMaintenance.GetAll " + ex);
                return new List<Entity.ElevatorMaintenance>();
            }
        }
        public Entity.ElevatorMaintenance Get(Guid id)
        {
            Entity.ElevatorMaintenance maintenance = new Entity.ElevatorMaintenance();
            try
            {
                maintenance = _elevatorMaintenanceRepository.FindBy(t => t.Guid == id).Select(p => Mapper.Configuration.Mapper.Map<Entity.ElevatorMaintenance>(p)).FirstOrDefault();
                if (maintenance != null) {
                    maintenance.BuildingGuid = _entityRepository.FindBy(t => t.Guid == maintenance.EntityGuid).FirstOrDefault().ParentEntityGuid;
                   
                }                
                return maintenance;
                
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "ElevatorMaintenance.Get " + ex);
                return null;
            }
        }
        public Entity.ActionStatus Manage(Entity.ElevatorMaintenance request)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                if (request.Guid == null || request.Guid == Guid.Empty)
                {
                    var dbElevatorMaintenance = Mapper.Configuration.Mapper.Map<Entity.ElevatorMaintenance, Model.ElevatorMaintenance>(request);

                    dbElevatorMaintenance.Guid = request.Guid;
                    dbElevatorMaintenance.CompanyGuid = SolutionConfiguration.CompanyId;
                    DateTime dateValue;
                    if (DateTime.TryParse(request.ScheduledDate.ToString(), out dateValue))
                        dbElevatorMaintenance.ScheduledDate = dateValue;

                    actionStatus = _elevatorMaintenanceRepository.Manage(dbElevatorMaintenance);
                    if (actionStatus.Data != null)
                    {
                        //   actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.ElevatorMaintenance, Entity.ElevatorMaintenance>(actionStatus.Data);
                        actionStatus.Data = Get(actionStatus.Data);
                    }
                    if (!actionStatus.Success)
                    {
                        _logger.Error($"ElevatorMaintenance is not added, Error: {actionStatus.Message}");
                        actionStatus.Success = false;
                        actionStatus.Message = actionStatus.Message;
                    }
                }
                else
                {
                    var olddbElevatorMaintenance = _elevatorMaintenanceRepository.FindBy(x => x.Guid.Equals(request.Guid)).FirstOrDefault();
                    if (olddbElevatorMaintenance == null)
                    {
                        throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : ElevatorMaintenance");
                    }
                    var dbElevatorMaintenance = Mapper.Configuration.Mapper.Map(request, olddbElevatorMaintenance);
                    dbElevatorMaintenance.CompanyGuid = SolutionConfiguration.CompanyId;

                    actionStatus = _elevatorMaintenanceRepository.Manage(dbElevatorMaintenance);
                    if (actionStatus.Data != null)
                    {
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.ElevatorMaintenance, Entity.ElevatorMaintenance>(dbElevatorMaintenance);
                    }
                    if (!actionStatus.Success)
                    {
                        _logger.Error($"ElevatorMaintenance is not updated , Error: {actionStatus.Message}");
                        actionStatus.Success = false;
                        actionStatus.Message = actionStatus.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "ElevatorMaintenanceManager.Delete " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.ActionStatus Delete(Guid id)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbElevatorMaintenance = _elevatorMaintenanceRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbElevatorMaintenance == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : ElevatorMaintenance");
                }
                dbElevatorMaintenance.IsDeleted = true;             
                return _elevatorMaintenanceRepository.Update(dbElevatorMaintenance);     
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "ElevatorMaintenanceManager.Delete " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.SearchResult<List<Entity.ElevatorMaintenanceDetail>> List(Entity.SearchRequest request)
        {
            try
            {
                var result = _elevatorMaintenanceRepository.List(request);
                return new Entity.SearchResult<List<Entity.ElevatorMaintenanceDetail>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.ElevatorMaintenanceDetail>(p)).ToList(),
                    Count = result.Count
                };
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, $"ElevatorMaintenanceService.List, Error: {ex.Message}");
                return new Entity.SearchResult<List<Entity.ElevatorMaintenanceDetail>>();
            }
        }
        public Entity.ActionStatus UpdateStatus(Guid id, string status)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbElevatorMaintenance = _elevatorMaintenanceRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbElevatorMaintenance == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : ElevatorMaintenance");
                }               
                    dbElevatorMaintenance.Status = status.ToString();                    
                    return _elevatorMaintenanceRepository.Update(dbElevatorMaintenance);   
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "ElevatorMaintenance.UpdateStatus " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }

        public List<Entity.ElevatorMaintenanceResponse> GetUpComingList(Entity.ElevatorMaintenanceRequest request)
        {
            try
            {
                return _elevatorMaintenanceRepository.GetUpComingList(request);
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, $"ElevatorMaintenanceService.List, Error: {ex.Message}");
                return new List<Entity.ElevatorMaintenanceResponse>();
            }
        }
    }
}
