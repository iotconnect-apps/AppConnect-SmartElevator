using component.helper;
using iot.solution.entity.Response;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using IOT = IoTConnect.Model;


namespace iot.solution.service.Implementation
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IotConnectClient _iotConnectClient;
        public ConfigurationService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
            _iotConnectClient = new IotConnectClient(SolutionConfiguration.BearerToken, SolutionConfiguration.Configuration.EnvironmentCode, SolutionConfiguration.Configuration.SolutionKey);
        }
        public StompReaderData GetConfguration(string key)
        {
            string configuarationtype = "stomp";
            if (key == "LiveData")
            {
                configuarationtype = "stomp";
            }
            else
            {
                configuarationtype = "ui-alert";
            }
            IOT.DataResponse<IOT.StompReaderData> deviceConnectionStatus = _iotConnectClient.Device.GetStompConfiguartionData(configuarationtype).Result;
            StompReaderData confgurationResponse = new StompReaderData();
            //var setting = component.helper.SolutionConfiguration.Configuration.IOTConnectSettings.Where(s => s.SettingType.Equals(key, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (deviceConnectionStatus != null && deviceConnectionStatus.status)
            {
                confgurationResponse = new StompReaderData()
                {
                    cpId = deviceConnectionStatus.data.cpId,
                    host = deviceConnectionStatus.data.host,
                    isSecure = deviceConnectionStatus.data.isSecure,
                    password = deviceConnectionStatus.data.password,
                    port = deviceConnectionStatus.data.port,
                    url = deviceConnectionStatus.data.url,
                    user = deviceConnectionStatus.data.user,
                    vhost = deviceConnectionStatus.data.vhost,
                };
            }
            return confgurationResponse;
        }
    }
}
