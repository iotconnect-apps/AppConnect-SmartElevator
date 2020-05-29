using AutoMapper;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;

namespace iot.solution.service.Mapper.Mapping
{
    public class ElevatorToUpdateDeviceModelMapping : ITypeConverter<Entity.Elevator, IOT.UpdateDeviceModel>
    {
        public IOT.UpdateDeviceModel Convert(Entity.Elevator source, IOT.UpdateDeviceModel destination, ResolutionContext context)
        {
            if (source == null)
            {
                return null;
            }

            if (destination == null)
            {
                destination = new IOT.UpdateDeviceModel();
            }

            destination.displayName = source.Name;
            destination.entityGuid = source.EntityGuid.ToString();
            destination.deviceTemplateGuid = source.TemplateGuid.ToString().ToUpper();
            destination.parentDeviceGuid = source.EntityGuid.ToString().ToUpper();
            destination.note = source.Note;
            destination.tag = source.Tag;
            destination.properties = new List<IOT.UpdateProperties>();
            //destination.primaryThumbprint = ;
            //destination.secondaryThumbprint = ;
            //destination.endorsementKey = ;
            return destination;
        }
    }
}
