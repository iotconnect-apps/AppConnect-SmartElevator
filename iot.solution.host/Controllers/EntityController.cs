using component.helper;
using iot.solution.entity.Structs.Routes;
using iot.solution.host.Filter;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
namespace host.iot.solution.Controllers
{
    [Route(EntityRoute.Route.Global)]
    public class EntityController : BaseController
    {
        private readonly IEntityService _service;

        public EntityController(IEntityService entityService)
        {
            _service = entityService;
        }

        [HttpGet]
        [Route(EntityRoute.Route.GetList, Name = EntityRoute.Name.GetList)]
        public Entity.BaseResponse<List<Entity.Entity>> Get()
        {
            Entity.BaseResponse<List<Entity.Entity>> response = new Entity.BaseResponse<List<Entity.Entity>>(true);
            try
            {
                response.Data = _service.Get();
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.Entity>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(EntityRoute.Route.GetById, Name = EntityRoute.Name.GetById)]
        [EnsureGuidParameterAttribute("id", "Entity")]
        public Entity.BaseResponse<Entity.Entity> Get(string id)
        {
            Entity.BaseResponse<Entity.Entity> response = new Entity.BaseResponse<Entity.Entity>(true);
            try
            {
                response.Data = _service.Get(Guid.Parse(id));
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.Entity>(false, ex.Message);
            }
            return response;
        }
       
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        [HttpPost]
        [Route(EntityRoute.Route.Manage, Name = EntityRoute.Name.Add)]
        public Entity.BaseResponse<Entity.Entity> Manage([FromForm]Entity.EntityModel request)
        {
            Entity.BaseResponse<Entity.Entity> response = new Entity.BaseResponse<Entity.Entity>(true);
            try
            {
                var status = _service.Manage(request);
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Data;
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.Entity>(false, ex.Message);
            }
            return response;
        }
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [HttpPut]
        [Route(EntityRoute.Route.DeleteImage, Name = EntityRoute.Name.DeleteImage)]
        [EnsureGuidParameterAttribute("id", "Entity")]
        public Entity.BaseResponse<bool> DeleteImage(string id)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _service.DeleteImage(Guid.Parse(id));
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Success;
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [HttpPut]
        [Route(EntityRoute.Route.Delete, Name = EntityRoute.Name.Delete)]
        [EnsureGuidParameterAttribute("id", "Entity")]
        public Entity.BaseResponse<bool> Delete(string id)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _service.Delete(Guid.Parse(id));
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Success;
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(EntityRoute.Route.BySearch, Name = EntityRoute.Name.BySearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.EntityDetail>>> GetBySearch(string parentEntityId="",string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "")
        {
            Entity.BaseResponse<Entity.SearchResult<List<Entity.EntityDetail>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.EntityDetail>>>(true);
            try
            {
                response.Data = _service.List(new Entity.SearchRequest()
                {
                    EntityId = string.IsNullOrEmpty(parentEntityId)?Guid.Empty:new Guid(parentEntityId),
                    SearchText = searchText,
                    PageNumber = -1,//pageNo.Value,
                    PageSize = -1,//pageSize.Value,
                    OrderBy = orderBy
                });
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.EntityDetail>>>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(EntityRoute.Route.UpdateStatus, Name = EntityRoute.Name.UpdateStatus)]
        [EnsureGuidParameterAttribute("id", "Entity")]
        public Entity.BaseResponse<bool> UpdateStatus(string id, bool status)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                Entity.ActionStatus result = _service.UpdateStatus(Guid.Parse(id), status);
                response.IsSuccess = result.Success;
                response.Message = result.Message;
                response.Data = result.Success;
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }
    }
}