using AutoMapper;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;

namespace iot.solution.service.Mapper.Mapping
{
    public class ElevatorToAddDeviceModelMapping : ITypeConverter<Entity.Elevator, IOT.AddDeviceModel>
    {
        public IOT.AddDeviceModel Convert(Entity.Elevator source, IOT.AddDeviceModel destination, ResolutionContext context)
        {
            if (source == null)
            {
                return null;
            }

            if (destination == null)
            {
                destination = new IOT.AddDeviceModel();
            }

            destination.DisplayName = source.Name;
            destination.uniqueId = source.UniqueId;
            destination.entityGuid = source.EntityGuid.ToString();
            destination.deviceTemplateGuid = source.TemplateGuid.ToString().ToUpper();
            //destination.parentDeviceGuid = source.EntityGuid.ToString().ToUpper();
            destination.note = source.Note;
            destination.tag = source.Tag;
            //destination.properties = new List<IOT.AddProperties>();
            //destination.primaryThumbprint = ;
            //destination.secondaryThumbprint = ;
            //destination.endorsementKey = ;
            return destination;
        }
    }
}
