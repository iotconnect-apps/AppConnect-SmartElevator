using System;
using System.Collections.Generic;
using System.Text;
using iot.solution.entity.Response;

namespace iot.solution.service.Interface
{
    public interface IConfigurationService
    {
        public ConfgurationResponse GetConfguration(string key);
    }
}
