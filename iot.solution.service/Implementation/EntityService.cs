using component.helper;
using component.logger;
using iot.solution.common;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;
using Model = iot.solution.model.Models;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Implementation
{
    public class EntityService : IEntityService
    {
        private readonly IEntityRepository _entityRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IDeviceService _deviceService;


        private readonly IotConnectClient _iotConnectClient;
        private readonly ILogger _logger;

        public EntityService(IEntityRepository entityRepository, ILogger logger, IDeviceRepository deviceRepository, IDeviceService deviceService)
        {
            _logger = logger;
            _entityRepository = entityRepository;
            _deviceService = deviceService;

            _deviceRepository = deviceRepository;
            _iotConnectClient = new IotConnectClient(SolutionConfiguration.BearerToken, SolutionConfiguration.Configuration.EnvironmentCode, SolutionConfiguration.Configuration.SolutionKey);
        }
        public List<Entity.Entity> Get()
        {
            try
            {
                return _entityRepository.GetAll().Where(e => !e.IsDeleted && !e.ParentEntityGuid.HasValue && e.CompanyGuid.Equals(SolutionConfiguration.CompanyId)).Select(p => Mapper.Configuration.Mapper.Map<Entity.Entity>(p)).ToList();
            }
            catch (Exception ex)
            {

                _logger.Error(Constants.ACTION_EXCEPTION, "Entity.GetAll " + ex);
                return new List<Entity.Entity>();
            }
        }
        public Entity.Entity Get(Guid id)
        {
            try
            {
                return _entityRepository.FindBy(r => r.Guid == id).Select(p => Mapper.Configuration.Mapper.Map<Entity.Entity>(p)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "Entity.Get " + ex);
                return null;
            }
        }
        public Entity.ActionStatus Manage(Entity.EntityModel request)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {


                if (request.Guid == null || request.Guid == Guid.Empty)
                {
                    Entity.Entity ghEntity = Mapper.Configuration.Mapper.Map<Entity.EntityModel, Entity.Entity>(request);
                    var addEntityResult = AsyncHelpers.RunSync<IOT.DataResponse<IOT.AddEntityResult>>(() =>
                       _iotConnectClient.Entity.Add(Mapper.Configuration.Mapper.Map<IOT.AddEntityModel>(ghEntity)));

                    if (addEntityResult != null && addEntityResult.status && addEntityResult.data != null)
                    {
                        request.Guid = Guid.Parse(addEntityResult.data.EntityGuid.ToUpper());
                        var dbEntity = Mapper.Configuration.Mapper.Map<Entity.Entity, Model.Entity>(ghEntity);
                        if (request.ImageFile != null)
                        {
                            // upload image                                     
                            dbEntity.Image = SaveEntityImage(request.Guid, request.ImageFile);
                        }
                        dbEntity.Guid = request.Guid;
                        dbEntity.CompanyGuid = SolutionConfiguration.CompanyId;
                        dbEntity.CreatedDate = DateTime.Now;
                        dbEntity.CreatedBy = SolutionConfiguration.CurrentUserId;
                        if (request.ParentEntityGuid == SolutionConfiguration.EntityGuid)
                        {
                            dbEntity.ParentEntityGuid = null;
                        }
                        actionStatus = _entityRepository.Manage(dbEntity);
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Entity, Entity.Entity>(actionStatus.Data);
                        if (!actionStatus.Success)
                        {
                            _logger.Error($"Entity is not added in solution database, Error: {actionStatus.Message}");
                            var deleteEntityResult = _iotConnectClient.Entity.Delete(request.Guid.ToString()).Result;
                            if (deleteEntityResult != null && deleteEntityResult.status)
                            {
                                _logger.Error($"Entity is not deleted from iotconnect, Error: {deleteEntityResult.message}");
                                actionStatus.Success = false;
                                actionStatus.Message = new UtilityHelper().IOTResultMessage(deleteEntityResult.errorMessages);
                            }
                        }
                    }
                    else
                    {
                        _logger.Error($"Entity is not added in iotconnect, Error: {addEntityResult.message}");
                        actionStatus.Success = false;
                        actionStatus.Message = new UtilityHelper().IOTResultMessage(addEntityResult.errorMessages);
                    }
                }
                else
                {
                    Entity.Entity ghEntity = Mapper.Configuration.Mapper.Map<Entity.EntityModel, Entity.Entity>(request);
                    var olddbEntity = _entityRepository.FindBy(x => x.Guid.Equals(request.Guid)).FirstOrDefault();
                    if (olddbEntity == null)
                    {
                        throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Entity");
                    }

                    var updateEntityResult = _iotConnectClient.Entity.Update(request.Guid.ToString(), Mapper.Configuration.Mapper.Map<IOT.UpdateEntityModel>(ghEntity)).Result;
                    if (updateEntityResult != null && updateEntityResult.status && updateEntityResult.data != null)
                    {
                        string existingImage = olddbEntity.Image;
                        var dbEntity = Mapper.Configuration.Mapper.Map(request, olddbEntity);
                        if (request.ImageFile != null)
                        {
                            if (File.Exists(SolutionConfiguration.UploadBasePath + dbEntity.Image) && request.ImageFile.Length > 0)
                            {
                                //if already exists image then delete  old image from server
                                File.Delete(SolutionConfiguration.UploadBasePath + dbEntity.Image);
                            }
                            if (request.ImageFile.Length > 0)
                            {
                                // upload new image                                     
                                dbEntity.Image = SaveEntityImage(request.Guid, request.ImageFile);
                            }
                        }
                        else
                        {
                            dbEntity.Image = existingImage;
                        }

                        dbEntity.UpdatedDate = DateTime.Now;
                        dbEntity.UpdatedBy = SolutionConfiguration.CurrentUserId;
                        dbEntity.CompanyGuid = SolutionConfiguration.CompanyId;
                        if (request.ParentEntityGuid == SolutionConfiguration.EntityGuid)
                        {
                            dbEntity.ParentEntityGuid = null;
                        }
                        actionStatus = _entityRepository.Manage(dbEntity);
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Entity, Entity.Entity>(dbEntity);
                        if (!actionStatus.Success)
                        {
                            _logger.Error($"Entity is not updated in solution database, Error: {actionStatus.Message}");
                            actionStatus.Success = false;
                            actionStatus.Message = actionStatus.Message;
                        }
                    }
                    else
                    {
                        _logger.Error($"Entity is not added in iotconnect, Error: {updateEntityResult.message}");
                        actionStatus.Success = false;
                        actionStatus.Message = new UtilityHelper().IOTResultMessage(updateEntityResult.errorMessages);
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "EntityManager.Delete " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        // Saving Image on Server   
        private string SaveEntityImage(Guid guid, IFormFile image)
        {
            var fileBasePath = SolutionConfiguration.UploadBasePath + SolutionConfiguration.BuildingImageBasePath;
            bool exists = System.IO.Directory.Exists(fileBasePath);
            if (!exists)
                System.IO.Directory.CreateDirectory(fileBasePath);
            string extension = Path.GetExtension(image.FileName);
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string fileName = guid.ToString() + "_" + unixTimestamp;
            var filePath = Path.Combine(fileBasePath, fileName + extension);
            if (image != null && image.Length > 0)
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }
                return Path.Combine(SolutionConfiguration.BuildingImageBasePath, fileName + extension);
            }
            return null;
        }
        public Entity.ActionStatus Delete(Guid id)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbEntity = _entityRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbEntity == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Entity");
                }

                //var dbSubEntities = _entityRepository.FindBy(t => t.ParentEntityGuid.Equals(id) && t.CompanyGuid.Equals(SolutionConfiguration.CompanyId) && !t.IsDeleted);
                //var dbDevice = _deviceRepository.FindBy(x => dbSubEntities.Any(t => t.Guid.Equals(x.EntityGuid)) && x.CompanyGuid.Equals(SolutionConfiguration.CompanyId)).FirstOrDefault();

                var dbChildEntity = _entityRepository.FindBy(x => x.ParentEntityGuid.Equals(id) && !x.IsDeleted).FirstOrDefault();
                var dbDevice = _deviceRepository.FindBy(x => !x.IsDeleted && (x.EntityGuid.Equals(id) || (dbChildEntity != null && x.EntityGuid.Equals(dbChildEntity.Guid)))).FirstOrDefault();
                if (dbDevice == null && dbChildEntity == null)
                {                   
                    var res = _iotConnectClient.Device.GetDeviceCounterByEntity(id.ToString()).Result;                    
                    if(res.data[0].counters.total !=0)
                    {
                        _logger.Error($"Location is not deleted in solution database.Machine exists, Error: {actionStatus.Message}");
                        actionStatus.Success = false;
                        actionStatus.Message = "Building or Wing is already associated with Elevator in IoTConnect so it can not be deleted.";
                    }  
                    else
                    {
                        var deleteEntityResult = _iotConnectClient.Entity.Delete(id.ToString()).Result;
                        if (deleteEntityResult != null && deleteEntityResult.status)
                        {
                            dbEntity.IsDeleted = true;
                            dbEntity.UpdatedDate = DateTime.Now;
                            dbEntity.UpdatedBy = SolutionConfiguration.CurrentUserId;
                            actionStatus = _entityRepository.Update(dbEntity);
                            //if (!string.IsNullOrEmpty(dbEntity.Image))
                            //{
                            //    DeleteEntityImage(dbEntity.Guid, dbEntity.Image);
                            //}
                            return actionStatus;
                        }
                        else
                        {
                            _logger.Error($"Entity is not deleted from iotconnect, Error: {deleteEntityResult.message}");
                            actionStatus.Success = false;
                            actionStatus.Message = new UtilityHelper().IOTResultMessage(deleteEntityResult.errorMessages);
                        }
                    }
                    
                }
                else if (dbChildEntity != null)
                {
                    _logger.Error($"Wing is already associated with Building so it can not be deleted, Error: {actionStatus.Message}");
                    actionStatus.Success = false;
                    actionStatus.Message = "Building is already associated with Wing so it can not be deleted.";
                }
                else
                {
                    _logger.Error($"Entity cannot be deleted. Device allocated!, Error: {actionStatus.Message}");
                    actionStatus.Success = false;
                    actionStatus.Message = "Elevator is already associated with Building or Wing so it can not be deleted.";
                }

            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "EntityManager.Delete " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        // Delete Image on Server   
        private bool DeleteEntityImage(Guid guid, string imageName)
        {
            var fileBasePath = SolutionConfiguration.UploadBasePath + SolutionConfiguration.BuildingImageBasePath;
            var filePath = Path.Combine(fileBasePath, imageName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return true;
        }
        public Entity.ActionStatus DeleteImage(Guid id)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(false);
            try
            {
                var dbEntity = _entityRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbEntity == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Entity");
                }

               bool deleteStatus = DeleteEntityImage(id, dbEntity.Image);
                if (deleteStatus)
                {
                    dbEntity.Image = "";
                    dbEntity.UpdatedDate = DateTime.Now;
                    dbEntity.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    dbEntity.CompanyGuid = SolutionConfiguration.CompanyId;

                    actionStatus = _entityRepository.Manage(dbEntity);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Entity, Entity.Entity>(dbEntity);
                    actionStatus.Success = true;
                    actionStatus.Message = "Image deleted successfully!";
                    if (!actionStatus.Success)
                    {
                        _logger.Error($"Entity is not updated in database, Error: {actionStatus.Message}");
                        actionStatus.Success = false;
                        actionStatus.Message = actionStatus.Message;
                    }
                }
                else {
                    actionStatus.Success = false;
                    actionStatus.Message = "Image not deleted!";
                }
                return actionStatus;
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "EntityManager.DeleteImage " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.SearchResult<List<Entity.EntityDetail>> List(Entity.SearchRequest request)
        {
            try
            {
                var result = _entityRepository.List(request);
                return new Entity.SearchResult<List<Entity.EntityDetail>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.EntityDetail>(p)).ToList(),
                    Count = result.Count
                };
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, $"EntityService.List, Error: {ex.Message}");
                return new Entity.SearchResult<List<Entity.EntityDetail>>();
            }
        }
        public Entity.ActionStatus UpdateStatus(Guid id, bool status)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbEntity = _entityRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbEntity == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Entity");
                }

                //var dbSubEntities = _entityRepository.FindBy(t => t.ParentEntityGuid.Equals(id) && t.CompanyGuid.Equals(SolutionConfiguration.CompanyId) && !t.IsDeleted);
                //var dbDevice = _deviceRepository.FindBy(x => dbSubEntities.Any(t => t.Guid.Equals(x.EntityGuid)) && x.CompanyGuid.Equals(SolutionConfiguration.CompanyId)).FirstOrDefault();
                var dbChildEntity = _entityRepository.FindBy(x => x.ParentEntityGuid.Equals(id) && !x.IsDeleted).FirstOrDefault();
                var dbDevice = _deviceRepository.FindBy(x => !x.IsDeleted && (x.EntityGuid.Equals(id) || (dbChildEntity != null && x.EntityGuid.Equals(dbChildEntity.Guid)))).FirstOrDefault();
                if (dbDevice == null && dbChildEntity == null)
                {

                   
                    dbEntity.IsActive = status;
                    dbEntity.UpdatedDate = DateTime.Now;
                    dbEntity.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    return _entityRepository.Update(dbEntity);
                }
                else if(dbChildEntity!=null)
                {
                    _logger.Error($"Entity cannot be deleted. Device allocated!, Error: {actionStatus.Message}");
                    actionStatus.Success = false;
                    actionStatus.Message = "Building status cannot be changed because wing allocated!";
                }
                else
                {
                    _logger.Error($"Entity cannot be deleted. Device allocated!, Error: {actionStatus.Message}");
                    actionStatus.Success = false;
                    actionStatus.Message = "Building status cannot be changed because elevator allocated!";
                }


            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "UserManager.Delete " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Response.EntityDetailResponse GetEntityDetail(Guid entityId)
        {
            return new Response.EntityDetailResponse()
            {
                EnergyUsage = 2700,
                Temperature = 73,
                Moisture = 15,
                Humidity = 62,
                WaterUsage = 3800,
                TotalDevices = 15
            };
        }
        public Entity.BaseResponse<Entity.BuildingOverviewResponse> GetBuildingOverviewDetail(Guid buildingId, DateTime currentDate, string timeZone)
        {
            Entity.BaseResponse<List<Entity.BuildingOverviewResponse>> listResult = new Entity.BaseResponse<List<Entity.BuildingOverviewResponse>>();
            Entity.BaseResponse<Entity.BuildingOverviewResponse> result = new Entity.BaseResponse<Entity.BuildingOverviewResponse>();
            try
            {
                listResult = _entityRepository.GetBuildingOverview(buildingId, "",currentDate,timeZone);

                var deviceResult = _deviceService.GetDeviceCountersByEntity(buildingId);
                if (listResult.Data.Count > 0)
                {
                    result.IsSuccess = true;
                    result.Data = listResult.Data[0];
                    result.LastSyncDate = listResult.LastSyncDate;
                    if (deviceResult.IsSuccess && deviceResult.Data != null)
                    {
                        result.Data.TotalConnectedElevator = deviceResult.Data.counters.connected.ToString();
                        result.Data.TotalElevator = deviceResult.Data.counters.total.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "EntityService.GetBuildingOverviewDetail " + ex);
            }
            return result;
        }
        public Entity.BuildingOverviewResponse GetBuildingOverview(Guid buildingId, string frequency, DateTime currentDate, string timeZone)
        {
            Entity.BaseResponse<List<Entity.BuildingOverviewResponse>> listResult = new Entity.BaseResponse<List<Entity.BuildingOverviewResponse>>();
            Entity.BuildingOverviewResponse result = new Entity.BuildingOverviewResponse();
            try
            {
                listResult = _entityRepository.GetBuildingOverview(buildingId, frequency,currentDate,timeZone);
                if (listResult.Data.Count > 0)
                {
                    result = listResult.Data[0];
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "EntityService.GetBuildingOverview " + ex);
            }
            return result;
        }
    }
}
