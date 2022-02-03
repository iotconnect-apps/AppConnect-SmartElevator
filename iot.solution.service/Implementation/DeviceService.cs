using component.helper;
using component.logger;
using iot.solution.common;
using iot.solution.entity;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;
using Model = iot.solution.model.Models;
using Response = iot.solution.entity.Response;
using LogHandler = component.services.loghandler;

namespace iot.solution.service.Implementation
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IHardwareKitRepository _hardwareKitRepository;
        private readonly ILookupService _lookupService;
        private readonly IotConnectClient _iotConnectClient;
        private readonly LogHandler.Logger _logger;
        private readonly IEntityRepository _entityRepository;
        public DeviceService(IDeviceRepository deviceRepository, ILookupService lookupService, IHardwareKitRepository hardwareKitRepository, LogHandler.Logger logger, IEntityRepository entityRepository)
        {
            _logger = logger;
            _deviceRepository = deviceRepository;
            _hardwareKitRepository = hardwareKitRepository;
            _lookupService = lookupService;
            _entityRepository = entityRepository;
            _iotConnectClient = new IotConnectClient(SolutionConfiguration.BearerToken, SolutionConfiguration.Configuration.EnvironmentCode, SolutionConfiguration.Configuration.SolutionKey);
        }

        public List<Entity.Elevator> Get()
        {
            try
            {
                return _deviceRepository.GetAll().Where(e => !e.IsDeleted).Select(p => Mapper.Configuration.Mapper.Map<Entity.Elevator>(p)).ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }
        public Entity.Elevator Get(Guid id)
        {
            try
            {
                Entity.Elevator elevator = new Elevator();
                elevator = _deviceRepository.FindBy(r => r.Guid == id).Select(p => Mapper.Configuration.Mapper.Map<Entity.Elevator>(p)).FirstOrDefault();
                var wing = _entityRepository.FindBy(r => r.Guid == elevator.EntityGuid).FirstOrDefault();
                var building = _entityRepository.FindBy(r => r.Guid == wing.ParentEntityGuid).FirstOrDefault();
                elevator.Wing = wing.Name;
                elevator.Building = building.Name;
                var template = _lookupService.GetIotTemplateGuidByCode();
                if (template != null)
                    elevator.TemplateGuid = new Guid(template);
                return elevator;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, "DeviceManager.Get " + ex);
                return null;
            }
        }
        // Saving Image on Server   
        private string SaveElevatorImage(Guid guid, IFormFile image)
        {
            var fileBasePath = SolutionConfiguration.UploadBasePath + SolutionConfiguration.ElevatorImageBasePath;
            bool exists = System.IO.Directory.Exists(fileBasePath);
            if (!exists)
                System.IO.Directory.CreateDirectory(fileBasePath);
            string extension = Path.GetExtension(image.FileName);
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string fileName = guid.ToString() + "_" + unixTimestamp;
            var filePath = Path.Combine(fileBasePath, fileName + extension);
            if (image != null && image.Length > 0 && SolutionConfiguration.AllowedImages.Contains(extension.ToLower()))
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }
                return Path.Combine(SolutionConfiguration.ElevatorImageBasePath, fileName + extension);
            }
            return null;
        }
        public Entity.ActionStatus Manage(Entity.ElevatorModel request)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbDevice = Mapper.Configuration.Mapper.Map<Entity.Elevator, Model.Elevator>(request);
                if (request.Guid == null || request.Guid == Guid.Empty)
                {
                        string templateGuid = _lookupService.GetIotTemplateGuidByCode();
                        if (!string.IsNullOrEmpty(templateGuid))
                        {
                            request.TemplateGuid = new Guid(templateGuid);
                            var addDeviceResult = _iotConnectClient.Device.Add(Mapper.Configuration.Mapper.Map<IOT.AddDeviceModel>(request)).Result;
                            if (addDeviceResult != null && addDeviceResult.status && addDeviceResult.data != null)
                            {
                                request.Guid = Guid.Parse(addDeviceResult.data.newid.ToUpper());
                                IOT.DataResponse<IOT.AcquireDeviceResult> acquireResult = _iotConnectClient.Device.AcquireDevice(request.UniqueId, new IOT.AcquireDeviceModel()).Result;
                                if (request.ImageFile != null)
                                {
                                    dbDevice.Image = SaveElevatorImage(request.Guid.Value, request.ImageFile);
                                }
                                dbDevice.Guid = request.Guid.Value;
                                dbDevice.IsProvisioned = true;
                                dbDevice.IsActive = true;
                                dbDevice.CompanyGuid = SolutionConfiguration.CompanyId;
                                dbDevice.CreatedDate = DateTime.Now;
                                dbDevice.CreatedBy = SolutionConfiguration.CurrentUserId;
                                actionStatus = _deviceRepository.Manage(dbDevice);
                                actionStatus.Data = (Guid)(actionStatus.Data); //Mapper.Configuration.Mapper.Map<Model.Elevator, Entity.Elevator>(actionStatus.Data);
                                if (!actionStatus.Success)
                                {
                                    _logger.ErrorLog(new Exception($"Elevator is not added in solution database, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                                    var deleteEntityResult = _iotConnectClient.Device.Delete(request.Guid.ToString()).Result;
                                    if (deleteEntityResult != null && deleteEntityResult.status)
                                    {
                                        _logger.ErrorLog(new Exception($"Elevator is not deleted from iotconnect"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                                        actionStatus.Success = false;
                                        actionStatus.Message = "Something Went Wrong!";
                                    }
                                }
                            }
                            else
                            {
                                _logger.ErrorLog(new Exception($"Elevator is not added in iotconnect, Error: {addDeviceResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                                actionStatus.Success = false;
                                actionStatus.Data = Guid.Empty;
                                actionStatus.Message = new UtilityHelper().IOTResultMessage(addDeviceResult.errorMessages);
                            }
                        }
                        else
                        {
                            actionStatus.Success = false;
                            actionStatus.Data = Guid.Empty;
                            actionStatus.Message = "Template not found in IoTConnect";
                        }
                    }
                    else
                    {
                        var olddbDevice = _deviceRepository.GetByUniqueId(x => x.Guid.Equals(request.Guid));
                        if (olddbDevice == null)
                        {
                            throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Device");
                        }
                        var updateEntityResult = _iotConnectClient.Device.Update(request.Guid.ToString(), Mapper.Configuration.Mapper.Map<IOT.UpdateDeviceModel>(request)).Result;
                        if (updateEntityResult != null && updateEntityResult.status)
                        {
                            dbDevice.CreatedDate = olddbDevice.CreatedDate;
                            dbDevice.CreatedBy = olddbDevice.CreatedBy;
                            dbDevice.UpdatedDate = DateTime.Now;
                            dbDevice.UpdatedBy = SolutionConfiguration.CurrentUserId;
                            dbDevice.CompanyGuid = SolutionConfiguration.CompanyId;
                            dbDevice.TemplateGuid = olddbDevice.TemplateGuid;
                            actionStatus = _deviceRepository.Manage(dbDevice, true);
                            actionStatus.Data = (Guid)(actionStatus.Data);
                            //actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Elevator, Entity.Elevator>(actionStatus.Data);
                            if (!actionStatus.Success)
                            {
                                _logger.ErrorLog(new Exception($"Elevator is not updated in solution database, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);

                                actionStatus.Success = false;
                                actionStatus.Data = Guid.Empty;
                                actionStatus.Message = "Something Went Wrong!";
                            }
                        }
                        else
                        {
                            _logger.ErrorLog(new Exception($"Elevator is not added in iotconnect, Error: {updateEntityResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                            actionStatus.Success = false;
                            actionStatus.Data = Guid.Empty;
                            actionStatus.Message = new UtilityHelper().IOTResultMessage(updateEntityResult.errorMessages);
                        }
                    }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, "Elevator.Manage " + ex);
                return new Entity.ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            return actionStatus;
        }
        public Entity.ActionStatus Delete(Guid id)
        {

            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbDevice = _deviceRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbDevice == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Device");
                }

                var deleteEntityResult = _iotConnectClient.Device.Delete(id.ToString()).Result;
                if (deleteEntityResult != null && deleteEntityResult.status)
                {
                    dbDevice.IsDeleted = true;
                    dbDevice.UpdatedDate = DateTime.Now;
                    dbDevice.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    return _deviceRepository.Update(dbDevice);
                }
                else
                {
                    _logger.ErrorLog(new Exception($"Device is not deleted from iotconnect, Error: {deleteEntityResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    actionStatus.Success = false;
                    actionStatus.Message = new UtilityHelper().IOTResultMessage(deleteEntityResult.errorMessages);
                }

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, "DeviceManager.Delete " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.ActionStatus UpdateStatus(Guid id, bool status)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbDevice = _deviceRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbDevice == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Device");
                }

                var updatedbStatusResult = _iotConnectClient.Device.UpdateDeviceStatus(dbDevice.Guid.ToString(), new IOT.UpdateDeviceStatusModel() { IsActive = status }).Result;
                if (updatedbStatusResult != null && updatedbStatusResult.status)
                {
                    dbDevice.IsActive = status;
                    dbDevice.UpdatedDate = DateTime.Now;
                    dbDevice.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    return _deviceRepository.Update(dbDevice);
                }
                else
                {
                    _logger.ErrorLog(new Exception($"Device status is not updated in iotconnect, Error: {updatedbStatusResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    actionStatus.Success = false;
                    actionStatus.Message = new UtilityHelper().IOTResultMessage(updatedbStatusResult.errorMessages);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, "DeviceService.Delete " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.ActionStatus AcquireDevice(string uniqueid)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                IOT.DataResponse<IOT.AcquireDeviceResult> acquireResult = _iotConnectClient.Device.AcquireDevice(uniqueid, null).Result;
                if (acquireResult != null && acquireResult.status)
                {
                    var dbDevice = _deviceRepository.FindBy(x => x.Guid.Equals(uniqueid)).FirstOrDefault();
                    dbDevice.IsProvisioned = true;
                    dbDevice.UpdatedDate = DateTime.Now;
                    dbDevice.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    return _deviceRepository.Update(dbDevice);
                }
                else
                {
                    return new ActionStatus(false, acquireResult.message);
                }

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, "DeviceService.AcquireDevice " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.SearchResult<List<Entity.Elevator>> List(Entity.SearchRequest request)
        {
            try
            {
                Entity.SearchResult<List<Entity.Elevator>> result = _deviceRepository.List(request);
                return new Entity.SearchResult<List<Entity.Elevator>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.Elevator>(p)).ToList(),
                    Count = result.Count
                };
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, $"DeviceService.List, Error: {ex.Message}");
                return new Entity.SearchResult<List<Entity.Elevator>>();
            }
        }
        public List<Entity.Elevator> GetEntityDeviceList(Guid entityId)
        {
            try
            {
                return _deviceRepository.FindBy(e => e.Guid == entityId && !e.IsDeleted && e.IsActive.HasValue && e.IsActive.Value).Select(c => Mapper.Configuration.Mapper.Map<Entity.Elevator>(c)).ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, $"DeviceService.GetEntityDeviceList, Error: {ex.Message}");
                return new List<Entity.Elevator>();
            }
        }
        public Entity.SearchResult<List<Entity.DeviceDetailResponse>> GetEntityDeviceDetailList(Entity.SearchRequest request)
        {
            try
            {
                Entity.SearchResult<List<Entity.DeviceDetailResponse>> result = _deviceRepository.DetailList(request);
                return new Entity.SearchResult<List<Entity.DeviceDetailResponse>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.DeviceDetailResponse>(p)).ToList(),
                    Count = result.Count
                };
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, $"DeviceService.List, Error: {ex.Message}");
                return new Entity.SearchResult<List<Entity.DeviceDetailResponse>>();
            }
        }
        public Entity.SearchResult<List<Entity.DeviceSearchResponse>> GatewayList(Entity.SearchRequest request)
        {
            try
            {
                var result = _deviceRepository.GatewayList(request);
                return new Entity.SearchResult<List<Entity.DeviceSearchResponse>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.DeviceSearchResponse>(p)).ToList(),
                    Count = result.Count
                };
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, $"DeviceService.List, Error: {ex.Message}");
                return new Entity.SearchResult<List<Entity.DeviceSearchResponse>>();
            }
        }
        public SearchResult<List<Elevator>> ChildDeviceList(SearchRequest request)
        {
            try
            {
                var result = _deviceRepository.GetChildDevice(request);
                return new Entity.SearchResult<List<Entity.Elevator>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.Elevator>(p)).ToList(),
                    Count = result.Count
                };
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, $"DeviceService.List, Error: {ex.Message}");
                return new Entity.SearchResult<List<Entity.Elevator>>();
            }
        }
        public Entity.BaseResponse<Response.DeviceDetailsResponse> GetDeviceDetail(Guid deviceId, DateTime? currentDate = null, string timeZone = "")
        {
            try
            {
                var result = _deviceRepository.GetDeviceDetail(deviceId,currentDate,timeZone);
                return result;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, $"DeviceService.GetDeviceDetail, Error: {ex.Message}");
                return new Entity.BaseResponse<Response.DeviceDetailsResponse>();
            }

        }
        public List<Response.EntityDevicesResponse> GetEntityDevices(Guid entityId)
        {
            try
            {
                return _deviceRepository.GetEntityDevices(entityId, null);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, $"GetEntityDevices.List, Error: {ex.Message}");
                return null;
            }
        }
        public List<Response.EntityDevicesResponse> GetEntityChildDevices(Guid deviceId)
        {
            try
            {
                return _deviceRepository.GetEntityDevices(null, deviceId);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, $"GetEntityChildDevices.List, Error: {ex.Message}");
                return null;
            }
        }
        public Entity.BaseResponse<int> ValidateKit(string kitCode)
        {
            Entity.BaseResponse<int> result = new Entity.BaseResponse<int>(true);
            try
            {
                return _deviceRepository.ValidateKit(kitCode);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, $"Device.ValidateKit, Error: {ex.Message}");
                return null;
            }

        }
        public Entity.BaseResponse<bool> ProvisionKit(Entity.ProvisionKitRequest request)
        {
            Entity.BaseResponse<bool> result = new Entity.BaseResponse<bool>(true);
            try
            {
                var repoResult = _deviceRepository.ProvisionKit(request);

                if (repoResult != null && repoResult.Data != null && repoResult.Data.Any())
                {
                    Entity.HardwareKit hardwareKit = repoResult.Data.FirstOrDefault();
                    string templateGuid = _lookupService.GetIotTemplateGuidByCode();

                    IOT.AddDeviceModel iotDeviceDetail = new IOT.AddDeviceModel()
                    {
                        DisplayName = hardwareKit.Name,
                        entityGuid = request.WingGuid.ToString(),
                        uniqueId = hardwareKit.UniqueId,
                        deviceTemplateGuid = templateGuid,
                        note = hardwareKit.Note,
                        properties = new List<IOT.AddProperties>()
                    };

                    var addDeviceResult = _iotConnectClient.Device.Add(iotDeviceDetail).Result;
                    if (addDeviceResult != null && addDeviceResult.status && addDeviceResult.data != null)
                    {
                        IOT.DataResponse<IOT.AcquireDeviceResult> acquireResult = _iotConnectClient.Device.AcquireDevice(hardwareKit.UniqueId, new IOT.AcquireDeviceModel()).Result;

                        Model.Elevator dbDevice = new Model.Elevator()
                        {
                            Guid = Guid.Parse(addDeviceResult.data.newid.ToUpper()),
                            CompanyGuid = SolutionConfiguration.CompanyId,
                            EntityGuid = request.WingGuid,
                            TemplateGuid = Guid.Parse(templateGuid),
                            UniqueId = hardwareKit.UniqueId,
                            Name = hardwareKit.Name,
                            Note = hardwareKit.Note,
                            Description = request.Description,
                            Specification = request.Specification,
                            IsProvisioned = acquireResult.status,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = SolutionConfiguration.CurrentUserId
                        };

                        if (request.ImageFile != null)
                        {
                            dbDevice.Image = SaveElevatorImage(dbDevice.Guid, request.ImageFile);
                        }

                        Entity.ActionStatus actionStatus = _deviceRepository.Manage(dbDevice);

                        if (!actionStatus.Success)
                        {
                            _logger.ErrorLog(new Exception($"Device is not added in solution database, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                            var deleteEntityResult = _iotConnectClient.Device.Delete(dbDevice.Guid.ToString()).Result;
                            if (deleteEntityResult != null && deleteEntityResult.status)
                            {
                                _logger.ErrorLog(new Exception($"Device is not deleted from iotconnect, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                                actionStatus.Success = false;
                                actionStatus.Message = "Something Went Wrong!";
                            }
                        }

                        var dbhardwareKit = _hardwareKitRepository.GetByUniqueId(t => t.KitCode == request.KitCode);
                        if (dbhardwareKit != null)
                        {
                            dbhardwareKit.CompanyGuid = SolutionConfiguration.CompanyId;
                            _hardwareKitRepository.Update(dbhardwareKit);
                        }
                    }
                    else
                    {
                        _logger.ErrorLog(new Exception($"Device is not deleted from iotconnect, Error: {addDeviceResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        result.IsSuccess = false;
                        result.Message = "Something Went Wrong!";
                    }
                }
                else
                {
                    return new Entity.BaseResponse<bool>(false, repoResult.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, $"Device.GetDeviceStatus, Error: {ex.Message}");
                return null;
            }
            return result;
        }
        public Entity.BaseResponse<Entity.DeviceCounterResult> GetDeviceCounters()
        {
            Entity.BaseResponse<Entity.DeviceCounterResult> result = new Entity.BaseResponse<Entity.DeviceCounterResult>(true);
            try
            {
                IOT.DataResponse<List<IOT.DeviceCounterResult>> deviceCounterResult = _iotConnectClient.Device.GetDeviceCounters("").Result;
                if (deviceCounterResult != null && deviceCounterResult.status)
                {
                    result.Data = Mapper.Configuration.Mapper.Map<Entity.DeviceCounterResult>(deviceCounterResult.data.FirstOrDefault());

                    var device = _iotConnectClient.Device.AllDevice(new IoTConnect.Model.AllDeviceModel { }).Result;
                    if (device != null && device.Data != null && device.Data.Any())
                    {
                        var resultIoT = (from r in device.Data
                                         join l in _deviceRepository.GetAll().Where(t => t.CompanyGuid.Equals(SolutionConfiguration.CompanyId) && !t.IsDeleted).ToList()
                   on r.Guid.ToUpper() equals l.Guid.ToString().ToUpper()
                                         select new
                                         {
                                             r.IsActive,
                                             r.IsConnected,
                                             r.Guid
                                         }).ToList();
                        result.Data.connected = resultIoT.Where(t => t.IsConnected).Count();
                        result.Data.disConnected = resultIoT.Where(t => !t.IsConnected).Count();
                        result.Data.active = resultIoT.Where(t => t.IsActive).Count();
                        result.Data.inActive = resultIoT.Where(t => !t.IsActive).Count();
                        result.Data.total = resultIoT.Count();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.BaseResponse<Entity.DeviceCounterResult>(false, ex.Message);
            }
            return result;
        }
        public Entity.BaseResponse<List<Entity.DeviceTelemetryDataResult>> GetTelemetryData(Guid deviceId)
        {
            Entity.BaseResponse<List<Entity.DeviceTelemetryDataResult>> result = new Entity.BaseResponse<List<Entity.DeviceTelemetryDataResult>>(true);
            try
            {
                IOT.DataResponse<List<IOT.DeviceTelemetryData>> deviceCounterResult = _iotConnectClient.Device.GetTelemetryData(deviceId.ToString()).Result;
                if (deviceCounterResult != null && deviceCounterResult.status)
                {
                    result.Data = deviceCounterResult.data.Select(d => Mapper.Configuration.Mapper.Map<Entity.DeviceTelemetryDataResult>(d)).ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.BaseResponse<List<Entity.DeviceTelemetryDataResult>>(false, ex.Message);
            }
            return result;
        }

        public Entity.BaseResponse<Entity.DeviceCounterByEntityResult> GetDeviceCountersByEntity(Guid entityGuid)
        {
            Entity.BaseResponse<Entity.DeviceCounterByEntityResult> result = new Entity.BaseResponse<Entity.DeviceCounterByEntityResult>(true);
            try
            {
                IOT.DataResponse<List<IOT.DeviceCounterByEntityResult>> deviceCounterResult = _iotConnectClient.Device.GetDeviceCounterByEntity(entityGuid.ToString()).Result;
                if (deviceCounterResult != null && deviceCounterResult.status)
                {
                    result.Data = Mapper.Configuration.Mapper.Map<Entity.DeviceCounterByEntityResult>(deviceCounterResult.data.FirstOrDefault());
                    var device = _iotConnectClient.Device.AllDevice(new IoTConnect.Model.AllDeviceModel { entityGuid = entityGuid.ToString()}).Result;
                    if (device != null && device.Data != null && device.Data.Any())
                    {
                        var resultIoT = (from r in device.Data
                                         join l in _deviceRepository.GetAll().Where(t => t.CompanyGuid.Equals(SolutionConfiguration.CompanyId) && !t.IsDeleted).ToList()
                   on r.Guid.ToUpper() equals l.Guid.ToString().ToUpper()
                                         select new
                                         {
                                             r.IsActive,
                                             r.IsConnected,
                                             r.Guid
                                         }).ToList();
                        result.Data.counters.connected = resultIoT.Where(t => t.IsConnected).Count();
                        result.Data.counters.disConnected = resultIoT.Where(t => !t.IsConnected).Count();
                        result.Data.counters.active = resultIoT.Where(t => t.IsActive).Count();
                        result.Data.counters.inActive = resultIoT.Where(t => !t.IsActive).Count();
                        result.Data.counters.total = resultIoT.Count();
                    }
                }
                else
                {
                    result.Data = null;
                    result.IsSuccess = false;
                    result.Message = new UtilityHelper().IOTResultMessage(deviceCounterResult.errorMessages);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.BaseResponse<Entity.DeviceCounterByEntityResult>(false, ex.Message);
            }
            return result;
        }

        public Entity.BaseResponse<Entity.DeviceConnectionStatusResult> GetConnectionStatus(string uniqueId)
        {
            Entity.BaseResponse<Entity.DeviceConnectionStatusResult> result = new Entity.BaseResponse<Entity.DeviceConnectionStatusResult>(true);
            try
            {
                IOT.DataResponse<List<IOT.DeviceConnectionStatus>> deviceConnectionStatus = _iotConnectClient.Device.GetConnectionStatus(uniqueId).Result;
                if (deviceConnectionStatus != null && deviceConnectionStatus.status && deviceConnectionStatus.data != null)
                {
                    result.Data = Mapper.Configuration.Mapper.Map<Entity.DeviceConnectionStatusResult>(deviceConnectionStatus.data.FirstOrDefault());
                }
                else
                {
                    result.Data = null;
                    result.IsSuccess = false;
                    result.Message = new UtilityHelper().IOTResultMessage(deviceConnectionStatus.errorMessages);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.BaseResponse<Entity.DeviceConnectionStatusResult>(false, ex.Message);
            }
            return result;
        }
    }
}
