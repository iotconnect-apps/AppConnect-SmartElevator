﻿using System;
using System.Collections.Generic;
using System.Text;
using Model = iot.solution.model.Models;
using Entity = iot.solution.entity;

namespace iot.solution.model.Repository.Interface
{
    public interface IDashboardRepository:IGenericRepository<Model.Entity>
    {
        Entity.BaseResponse<List<Entity.DashboardOverviewResponse>> GetStatistics(DateTime? currentDate = null, string timeZone = "");
    }
}
