﻿using component.logger;
using iot.solution.common;
using iot.solution.entity;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
using LogHandler = component.services.loghandler;
using System.Reflection;

namespace iot.solution.service.Data
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly LogHandler.Logger _logger;

        public CompanyService(ICompanyRepository companyRepository, LogHandler.Logger logManager)
        {
            _companyRepository = companyRepository;
            _logger = logManager;
        }

        public List<Company> Get()
        {
            List<Entity.Company> result = new List<Entity.Company>();
            try
            {
                result = _companyRepository.GetAll().Where(e => !e.IsDeleted).Select(p => Mapper.Configuration.Mapper.Map<Entity.Company>(p)).ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, "CompanyManager.GetCompany" + ex);
            }
            return result;
        }
        public Company Get(Guid id)
        {
            return _companyRepository.FindBy(r => r.Guid == id).Select(p => Mapper.Configuration.Mapper.Map<Entity.Company>(p)).FirstOrDefault();
        }

        public ActionStatus Manage(Entity.AddCompanyRequest request)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                if (request.Guid == null || request.Guid == Guid.Empty)
                {
                    request.CreatedBy = component.helper.SolutionConfiguration.CurrentUserId;
                    request.CreatedDate = DateTime.Now;
                    actionStatus = _companyRepository.Manage(request);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Company, Entity.Company>(actionStatus.Data);
                    if (!actionStatus.Success)
                    _logger.ErrorLog(new Exception($"Company is not added in solution database, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                }
                else
                {
                    var olddbCompany = _companyRepository.GetByUniqueId(x => x.Guid == request.Guid);
                    if (olddbCompany == null)
                        throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Company");
                    request.CreatedDate = olddbCompany.CreatedDate;
                    request.CreatedBy = olddbCompany.CreatedBy;
                    request.UpdatedBy = component.helper.SolutionConfiguration.CurrentUserId;
                    request.UpdatedDate = DateTime.Now;
                    actionStatus = _companyRepository.Manage(request);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Company, Entity.Company>(actionStatus.Data);
                    if (!actionStatus.Success)
                    _logger.ErrorLog(new Exception($"Company is not added in solution database, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, "Company.InsertCompany " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public ActionStatus Delete(Guid id)
        {
            try
            {
                var dbCompany = _companyRepository.GetByUniqueId(x => x.Guid == id);
                if (dbCompany == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Company");
                }
                dbCompany.IsDeleted = true;
                dbCompany.UpdatedDate = DateTime.Now;
                dbCompany.UpdatedBy = component.helper.SolutionConfiguration.CurrentUserId;
                return _companyRepository.Update(dbCompany);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, "Company.Delete " + ex);
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
                var dbCompany = _companyRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbCompany == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Company");
                }

                //var updatedbStatusResult = _iotConnectClient.Device.UpdateStatus(dbDevice.Guid.ToString(), status).Result;
                //if (updatedbStatusResult != null && updatedbStatusResult.status == (int)HttpStatusCode.OK)
                //{
                dbCompany.IsActive = status;
                dbCompany.UpdatedDate = DateTime.Now;
                dbCompany.UpdatedBy = component.helper.SolutionConfiguration.CurrentUserId;
                return _companyRepository.Update(dbCompany);
                //}
                //else
                //{
                //    _logger.Error($"Company status is not updated in iotconnect, Error: {updatedbStatusResult.message}");
                //    actionStatus.Success = false;
                //    actionStatus.Message = "Something Went Wrong!";
                //}

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, "CompanyService.Delete " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
    }
}
