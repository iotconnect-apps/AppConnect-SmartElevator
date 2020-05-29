﻿using component.logger;
using iot.solution.model.Models;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using LogHandler = component.services.loghandler;

namespace iot.solution.model.Repository.Implementation
{
    public class kitTypeCommandRepository : GenericRepository<KitTypeCommand>, IkitTypeCommandRepository
    {
        private readonly LogHandler.Logger logger;
        public kitTypeCommandRepository(IUnitOfWork unitOfWork, LogHandler.Logger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
        }
    }
}