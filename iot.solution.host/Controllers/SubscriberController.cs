﻿using component.helper;
using host.iot.solution.Controllers;
using iot.solution.entity;
using iot.solution.entity.Response;
using iot.solution.entity.Structs.Routes;
using iot.solution.host.Filter;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;

namespace iot.solution.host.Controllers
{
    [Route(SubscriberRoute.Route.Global)]
    [ApiController]
    public class SubscriberController : BaseController
    {
        private readonly ISubscriberService _service;
        public SubscriberController(ISubscriberService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(SubscriberRoute.Route.GetStripeKey, Name = SubscriberRoute.Name.GetStripeKey)]
        public Entity.BaseResponse<string> GetStripeKey()
        {
            Entity.BaseResponse<string> response = new Entity.BaseResponse<string>(true);
            try
            {
                response.Data = SolutionConfiguration.Configuration.SubscriptionAPI.StripeAPIKey;
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<string>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(SubscriberRoute.Route.GetCountryLookup, Name = SubscriberRoute.Name.GetCountryLookup)]
        public Entity.BaseResponse<Response.CountryResponse> GetCountryLookUp()
        {
            Entity.BaseResponse<Response.CountryResponse> response = new Entity.BaseResponse<Response.CountryResponse>(true);
            try
            {
                response.Data = _service.GetCountryLookUp();
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Response.CountryResponse>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(SubscriberRoute.Route.GetStateLookup, Name = SubscriberRoute.Name.GetStateLookup)]
        [EnsureGuidParameterAttribute("countryId", "Subscriber")]
        public Entity.BaseResponse<Response.StateResponse> GetStateLookUp(string countryId)
        {
            BaseResponse<Response.StateResponse> response = new BaseResponse<Response.StateResponse>(true);
            try
            {
                response.Data = _service.GetStateLookUp(countryId);
            }
            catch (Exception ex)
            {
                return new BaseResponse<Response.StateResponse>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(SubscriberRoute.Route.GetTimeZoneLookup, Name = SubscriberRoute.Name.GetTimeZoneLookup)]
        public Entity.BaseResponse<Entity.Response.TimezoneResponse> GetTimezoneLookUp()
        {
            BaseResponse<Response.TimezoneResponse> response = new BaseResponse<Response.TimezoneResponse>(true);
            try
            {
                response.Data = _service.GetTimezoneLookUp();
            }
            catch (Exception ex)
            {
                return new BaseResponse<Response.TimezoneResponse>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(SubscriberRoute.Route.GetSubscriptionPlan, Name = SubscriberRoute.Name.GetSubscriptionPlan)]
        public BaseResponse<SubscriptionPlanResponse> GetSubscriptionPlans()
        {
            BaseResponse<Response.SubscriptionPlanResponse> response = new BaseResponse<Response.SubscriptionPlanResponse>(true);
            try
            {
                response.Data = _service.GetSubscriptionPlans(SolutionConfiguration.Configuration.SubscriptionAPI.SolutionCode);
            }
            catch (Exception ex)
            {
                return new BaseResponse<Response.SubscriptionPlanResponse>(false, ex.Message);
            }
            return response;
        }
        [HttpPost]
        [AllowAnonymous]
        [Route(SubscriberRoute.Route.ValidateCompany, Name = SubscriberRoute.Name.ValidateCompany)]
        public BaseResponse<bool> ValidateCompany(ValidateCompanyRequest requestData)
        {
            var response = new BaseResponse<bool>(true);
            try
            {
                var apiResponse = _service.ValidateCompany(requestData);
                    response.IsSuccess = apiResponse.Success;
                    response.Message = apiResponse.Message;                
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new BaseResponse<bool>(false, ex.Message);
            }

            return response;
        }
        [HttpPost]
        [AllowAnonymous]
        [Route(SubscriberRoute.Route.SaveCompany, Name = SubscriberRoute.Name.SaveCompany)]
        public BaseResponse<Entity.SaveCompanyResponse> SaveCompany(SaveCompanyRequest requestData)
        {
            var response = new BaseResponse<Entity.SaveCompanyResponse>(true);
            try
            {
                var apiResponse = _service.SaveCompany(requestData);
                response.Data = apiResponse.Data;
                if (!apiResponse.Success)
                {
                    response.IsSuccess = false;
                    response.Message = apiResponse.Message;
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse<Entity.SaveCompanyResponse>(false, ex.Message);
            }

            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(SubscriberRoute.Route.BySearch, Name = SubscriberRoute.Name.BySearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.SubscriberData>>> GetBySearch(string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "")
        {
            Entity.BaseResponse<Entity.SearchResult<List<Entity.SubscriberData>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.SubscriberData>>>(true);
            try
            {
                response.Data = _service.SubscriberList(SolutionConfiguration.Configuration.SubscriptionAPI.SolutionCode
                    , new Entity.SearchRequest()
                    {
                        SearchText = searchText,
                        PageNumber = pageNo.Value,
                        PageSize = pageSize.Value,
                        OrderBy = orderBy,

                    });
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.SubscriberData>>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(SubscriberRoute.Route.GetSubscriberDetails, Name = SubscriberRoute.Name.GetSubscriberDetails)]
        public BaseResponse<Entity.SubsciberCompanyDetails> GetSubscriberDetails(string userEmail)
        {
            BaseResponse<Entity.SubsciberCompanyDetails> response = new BaseResponse<Entity.SubsciberCompanyDetails>(true);
            try
            {
                response.Data = _service.GetSubscriberDetails(SolutionConfiguration.Configuration.SubscriptionAPI.SolutionCode, userEmail);
            }
            catch (Exception ex)
            {
                return new BaseResponse<Entity.SubsciberCompanyDetails>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(SubscriberRoute.Route.GetSubscriberKitDetails, Name = SubscriberRoute.Name.GetSubscriberKitDetails)]
        public BaseResponse<Entity.SearchResult<List<Entity.HardwareKitResponse>>> GetSubscriberKitDetails(string companyId, string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "")
        {
            BaseResponse<Entity.SearchResult<List<Entity.HardwareKitResponse>>> response = new BaseResponse<Entity.SearchResult<List<Entity.HardwareKitResponse>>>(true);
            try
            {
                response.Data = _service.GetSubscriberKitDetails(companyId, new Entity.SearchRequest()
                {
                    SearchText = searchText,
                    PageNumber = pageNo.Value,
                    PageSize = pageSize.Value,
                    OrderBy = orderBy,

                }, true);
            }
            catch (Exception ex)
            {
                return new BaseResponse<Entity.SearchResult<List<Entity.HardwareKitResponse>>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(SubscriberRoute.Route.GetLastSyncDetails, Name = SubscriberRoute.Name.GetLastSyncDetails)]
        public BaseResponse<Entity.LastSyncResponse> GetLastSyncDetails()
        {
            BaseResponse<Entity.LastSyncResponse> response = new BaseResponse<Entity.LastSyncResponse>(true);
            try
            {
                response.Data = _service.GetLastSyncDetails();
            }
            catch (Exception ex)
            {
                return new BaseResponse<Entity.LastSyncResponse>(false, ex.Message);
            }
            return response;
        }

    }
}
