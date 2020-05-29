using Model = iot.solution.model.Models;
using Entity = iot.solution.entity;
using IOTUserProvider = IoTConnect.UserProvider;
using AutoMapper;
using System;
using IOT = IoTConnect.Model;
using iot.solution.service.Mapper.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace iot.solution.service.Mapper
{
    public class Configuration
    {
        public static IMapper Mapper { get; private set; }

        public static void Initialize()
        {
            var config = new MapperConfiguration(mc =>
            {
                mc.CreateMap<Model.User, Entity.User>().ReverseMap();
                mc.CreateMap<Model.User, Entity.AddUserRequest>()
                .ForMember(au => au.EntityGuid, o => o.MapFrom(u => u.Guid)).ReverseMap();
                mc.CreateMap<Model.Role, Entity.Role>().ReverseMap();
                mc.CreateMap<Model.Company, Entity.Company>().ReverseMap();
                mc.CreateMap<Model.Entity, Entity.Entity>().ReverseMap();
                mc.CreateMap<Model.Entity, Entity.EntityDetail>().ReverseMap();
                mc.CreateMap<Model.ElevatorMaintenance, Entity.ElevatorMaintenance>().ReverseMap();
                mc.CreateMap<Model.ElevatorMaintenance, Entity.ElevatorMaintenanceDetail>().ReverseMap();
                mc.CreateMap<Entity.EntityModel, Entity.Entity>().ReverseMap();
                mc.CreateMap<Model.Elevator, Entity.Elevator>().ReverseMap();
                mc.CreateMap<Model.Elevator, Entity.DeviceResponse>().ReverseMap();
                mc.CreateMap<Model.Elevator, Entity.DeviceDetailResponse>().ReverseMap();
                mc.CreateMap<Model.User, Entity.UserResponse>().ReverseMap();
                
                mc.CreateMap<Model.HardwareKit, Entity.HardwareKit>().ReverseMap();
                mc.CreateMap<Model.HardwareKit, Entity.HardwareKitDTO>().ReverseMap();

                mc.CreateMap<Model.KitType, Entity.KitType>().ReverseMap();
                mc.CreateMap<Model.KitTypeAttribute, Entity.KitTypeAttribute>().ReverseMap();
                mc.CreateMap<Model.KitTypeCommand, Entity.KitTypeCommand>().ReverseMap();
              

               
                mc.CreateMap<IOT.AllRuleResult, Entity.AllRuleResponse>().ReverseMap();
                mc.CreateMap<IOT.SingleRuleResult, Entity.SingleRuleResponse>().ConvertUsing(new SingleRuleResultToSingleRuleResponse());
                mc.CreateMap<IOT.EventSubscription, Entity.EventSubscription>().ConvertUsing(new EventSubscriptionToEventSubscriptionResponse());
                mc.CreateMap<IOT.DeliveryMethodData, Entity.DeliveryMethodData>().ReverseMap();
                mc.CreateMap<IOT.DataXml, Entity.DataXml>().ConvertUsing(new DataXmlToDataXmlResponse());
                mc.CreateMap<IOT.Roleguids, Entity.Roleguid>().ReverseMap();
                mc.CreateMap<IOT.Userguids, Entity.Userguid>().ReverseMap();
                mc.CreateMap<IOT.EventCommand, Entity.EventCommand>().ReverseMap();
                mc.CreateMap<Entity.Rule, IOT.AddRuleModel>().ReverseMap();
                mc.CreateMap<Entity.Rule, IOT.UpdateRuleModel>().ReverseMap();
                mc.CreateMap<Entity.NotificationAddRequest, IOT.AddRuleModel>().ReverseMap();
                mc.CreateMap<Entity.NotificationAddRequest, IOT.UpdateRuleModel>().ReverseMap();

                #region " IOT Connect Mapping"

                mc.CreateMap<Entity.Entity, IOT.AddEntityModel>().ConvertUsing(new EntityToAddEntityModelMapping());
                mc.CreateMap<Entity.Entity, IOT.UpdateEntityModel>().ConvertUsing(new EntityToUpdateEntityModelMapping());
                mc.CreateMap<Entity.Elevator, IOT.AddDeviceModel>().ConvertUsing(new ElevatorToAddDeviceModelMapping());
                mc.CreateMap<Entity.Elevator, IOT.UpdateDeviceModel>().ConvertUsing(new ElevatorToUpdateDeviceModelMapping());
                mc.CreateMap<Entity.AddUserRequest, IOT.AddUserModel>().ConvertUsing(new AddUserRequestToAddUserModelMapping());
                mc.CreateMap<Entity.AddUserRequest, IOT.UpdateUserModel>().ConvertUsing(new AddUserRequestToUpdateUserModelMapping());
                mc.CreateMap<Entity.ChangePasswordRequest, IOT.ChangePasswordModel>().ConvertUsing(new ChangePasswordRequestToChangePasswordModel());
                mc.CreateMap<Entity.DeviceCounterResult, IOT.DeviceCounterResult>().ReverseMap();
                mc.CreateMap<Entity.DeviceTelemetryDataResult, IOT.DeviceTelemetryData>().ReverseMap();
                mc.CreateMap<Entity.DeviceCounterByEntityResult, IOT.DeviceCounterByEntityResult>().ReverseMap();
                mc.CreateMap<Entity.Counters, IOT.Counters>().ReverseMap();
                mc.CreateMap<Entity.DeviceConnectionStatusResult, IOT.DeviceConnectionStatus>().ReverseMap();
                #endregion

                #region "AdminUser Mapping"

                mc.CreateMap<Model.AdminUser, Entity.AddAdminUserRequest>().ReverseMap();
                mc.CreateMap<Model.AdminUser, Entity.UserResponse>().ReverseMap();
                mc.CreateMap<Model.AdminUser, Entity.AdminUserResponse>().ReverseMap();

                #endregion

                #region "Admin Rule Mapping"
                mc.CreateMap<Model.AdminRule, Entity.AdminRule>().ReverseMap();
                #endregion
            });

            Mapper = config.CreateMapper();
        }

    }


}