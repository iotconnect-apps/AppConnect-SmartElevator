import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { CookieService } from 'ngx-cookie-service'
import * as moment from 'moment'
import { ApiConfigService, NotificationService } from '..';
@Injectable()

export class DashboardService {
  protected apiServer = ApiConfigService.settings.apiServer;
  cookieName = 'FM';
  constructor(
    private http: HttpClient,
    private cookieService: CookieService,
    private _notificationService: NotificationService
  ) {
    this._notificationService.apiBaseUrl = this.apiServer.baseUrl;
  }

  getDashboardoverview() {
    let currentUser = JSON.parse(localStorage.getItem('currentUser'));
    return this.http.get<any>(this.apiServer.baseUrl + 'api/dashboard/overview', {
      params: {
        currentDate: moment(new Date()).format('YYYY-MM-DDTHH:mm:ss'),
        timeZone: moment().utcOffset().toString()
      }
      }).map(response => {
      return response;
    });
  }

  getBuilding() {
    return this.http.get<any>(this.apiServer.baseUrl + 'api/dashboard/buildings/').map(response => {
      return response;
    });
  }


  getBuildingOverview(buildingId, frequency = 'd') {
    return this.http.get<any>(this.apiServer.baseUrl + 'api/dashboard/buildingoverview/' + buildingId, {
      params: {
        frequency: frequency,
        currentDate: moment(new Date()).format('YYYY-MM-DDTHH:mm:ss'),
        timeZone: moment().utcOffset().toString()
      }
    }).map(response => {
      return response;
    });
  }

  getTimeZone() {
    return /\((.*)\)/.exec(new Date().toString())[1];
  }
  getBuildingGraph(buildingId = '', frequency = 'd') {
    return this.http.post<any>(this.apiServer.baseUrl + 'api/chart/getoperationhours', {
      buildingId: buildingId,
      frequency: frequency,
    }).map(response => {
      return response;
    });
  }

  getdevicespeakhours(elevatorId = [], frequency = 'd') {
    return this.http.post<any>(this.apiServer.baseUrl + 'api/chart/getpeakhoursbyelevator', {
      //buildingId: buildingId,
      elevatorList: elevatorId,
      frequency: frequency,
    }).map(response => {
      return response;
    });
  }



  getUpcomingMaintenance(buildingId = '', elevatorId = '') {
    return this.http.get<any>(this.apiServer.baseUrl + 'api/elevatormaintenance/upcoming', {
      params: {
        buildingId: buildingId,
        elevatorId: elevatorId,
        currentDate: moment(new Date()).format('YYYY-MM-DDTHH:mm:ss'),
        timeZone: moment().utcOffset().toString()
      }
    }).map(response => {
      return response;
    });
  }

  getAlerts(buildingId = '', elevatorId = '') {
    return this.http.get<any>(this.apiServer.baseUrl + 'api/alert', {
      params: {
        buildingId: buildingId,
        elevatorId: elevatorId,
      }
    }).map(response => {
      return response;
    });
  }


  getAlertsList(parameters) {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
      }
    };

    const parameter = {
      params: {
        'pageNo': parameters.pageNumber + 1,
        'pageSize': parameters.pageSize,
        'searchText': parameters.searchText,
        'orderBy': parameters.sortBy,
        'deviceGuid': parameters.deviceGuid,
        'entityGuid': parameters.entityGuid,
      },
      timestamp: Date.now()
    };
    var reqParameter = Object.assign(parameter, configHeader);

    return this.http.get<any>(this.apiServer.baseUrl + 'api/alert', reqParameter).map(response => {
      return response;
    });
  }


  getBuildingDetail(buildingGuid) {
    return this.http.get<any>(this.apiServer.baseUrl + 'api/dashboard/getbuildingdetail/' + buildingGuid).map(response => {
      return response;
    });
  }
  getSoilnutrition(buildingGuid) {
    return this.http.get<any>(this.apiServer.baseUrl + 'api/building/getsoilnutrition/' + buildingGuid).map(response => {
      return response;
    });
  }


  getNotificationList() {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.get(this.apiServer.baseUrl + 'api/dashboard/notification', configHeader).map(response => {
      return response;
    });
  }

  getTruckUsage() {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.get(this.apiServer.baseUrl + 'api/dashboard/getTruckUsage', configHeader).map(response => {
      return response;
    });
  }

  getTruckActivity() {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.get(this.apiServer.baseUrl + 'api/dashboard/getTruckActivity', configHeader).map(response => {
      return response;
    });
  }

  getTruckGraph() {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.get(this.apiServer.baseUrl + 'api/dashboard/getTruckGraph', configHeader).map(response => {
      return response;
    });
  }

  getStompCon() {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.get(this.apiServer.baseUrl + 'api/dashboard/getStompConfiguration', configHeader).map(response => {
      return response;
    });
  }

  getDeviceAttributeHistoricalData(data) {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.post(this.apiServer.baseUrl + 'api/dashboard/getDeviceAttributeHistoricalData', data, configHeader).map(response => {
      return response;
    });
  }

  getSensors(data) {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.post(this.apiServer.baseUrl + 'api/dashboard/getDeviceAttributes', data, configHeader).map(response => {
      return response;
    });
  }

  tripStatus(id, data) {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.put<any>(this.apiServer.baseUrl + 'api/trip/' + id + '/status', data, configHeader).map(response => {
      return response;
    });
  }

  startSimulator(id, isSalesTemplate = true) {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.get<any>(this.apiServer.baseUrl + 'api/trip/startSimulator/' + id + '/' + isSalesTemplate, configHeader).map(response => {
      return response;
    });
  }

  getTruckLookup() {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.get(this.apiServer.baseUrl + 'api/truck/lookup', configHeader).map(response => {
      return response;
    });
  }

  // Get building look up by companyId
  getBuildingLookup(companyId) {

    return this.http.get<any>(this.apiServer.baseUrl + 'api/lookup/buildinglookup/' + companyId).map(response => {
      return response;
    });
  }
}
