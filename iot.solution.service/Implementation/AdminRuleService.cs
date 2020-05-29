﻿using component.helper;
using component.logger;
using iot.solution.common;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
using LogHandler = component.services.loghandler;

namespace iot.solution.service.Implementation
{
    public class AdminRuleService : IAdminRuleService
    {
        private readonly IAdminRuleRepository _adminRuleRepository;
        private readonly LogHandler.Logger _logger;
        public AdminRuleService(IAdminRuleRepository adminRuleRepository, LogHandler.Logger logManager)
        {
            _adminRuleRepository = adminRuleRepository;
            _logger = logManager;
        }
        public Entity.SearchResult<List<Entity.AdminRule>> List(Entity.SearchRequest request)
        {
            try
            {
                return _adminRuleRepository.List(request);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, $"AdminRuleService.List, Error: {ex.Message}");
                return new Entity.SearchResult<List<Entity.AdminRule>>();
            }
        }
        public Entity.AdminRule Get(Guid id)
        {
            try
            {
                return _adminRuleRepository.FindBy(r => r.Guid == id).Select(p => Mapper.Configuration.Mapper.Map<Entity.AdminRule>(p)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, "AdminRuleManager.Get " + ex);
                return null;
            }
        }
        public Entity.ActionStatus Manage(Entity.AdminRule request)
        {
            try
            {
                Entity.ActionStatus actionStatus = null;
                if (request.Guid == null || request.Guid == Guid.Empty)
                {
                    var dbAdminRule = Mapper.Configuration.Mapper.Map<Entity.AdminRule, Model.AdminRule>(request);
                    dbAdminRule.Guid = Guid.NewGuid();
                    dbAdminRule.CreatedDate = DateTime.Now;
                    dbAdminRule.CreatedBy = SolutionConfiguration.CurrentUserId;
                    dbAdminRule.IsActive = true;
                    actionStatus = _adminRuleRepository.Insert(dbAdminRule);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.AdminRule, Entity.AdminRule>(actionStatus.Data);
                }
                else
                {
                    var dbAdminRule = _adminRuleRepository.GetByUniqueId(x => x.Guid == request.Guid);
                    if (dbAdminRule == null)
                        throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : AdminRule");

                    request.CreatedBy = dbAdminRule.CreatedBy;
                    request.CreatedDate = dbAdminRule.CreatedDate;
                    request.IsActive = dbAdminRule.IsActive;
                    dbAdminRule = Mapper.Configuration.Mapper.Map<Entity.AdminRule, Model.AdminRule>(request, dbAdminRule);
                    dbAdminRule.UpdatedDate = DateTime.Now;
                    dbAdminRule.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    actionStatus = _adminRuleRepository.Update(dbAdminRule);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.AdminRule, Entity.AdminRule>(actionStatus.Data);
                }
                return actionStatus;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, "AdminRule.Manage " + ex);
                return new Entity.ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public Entity.ActionStatus Delete(Guid id)
        {
            try
            {
                var dbUser = _adminRuleRepository.GetByUniqueId(x => x.Guid == id);
                if (dbUser == null)
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : AdminRule");
                dbUser.IsDeleted = true;
                dbUser.UpdatedDate = DateTime.Now;
                dbUser.UpdatedBy = component.helper.SolutionConfiguration.CurrentUserId;
                return _adminRuleRepository.Update(dbUser);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, "AdminRule.Delete " + ex);
                return new Entity.ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public Entity.ActionStatus UpdateStatus(Guid id, bool status)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbAdminRule = _adminRuleRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbAdminRule == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : AdminRule");
                }
                dbAdminRule.IsActive = status;
                dbAdminRule.UpdatedDate = DateTime.Now;
                dbAdminRule.UpdatedBy = SolutionConfiguration.CurrentUserId;
                return _adminRuleRepository.Update(dbAdminRule);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, "AdminRule.UpdateStatus " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
    }
}
