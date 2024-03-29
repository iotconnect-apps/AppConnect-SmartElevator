import 'rxjs/add/operator/map'

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { CookieService } from 'ngx-cookie-service'
import { ApiConfigService, NotificationService } from 'app/services';
import * as moment from 'moment'
@Injectable({
	providedIn: 'root'
})

export class ElevatorService {
	protected apiServer = ApiConfigService.settings.apiServer;
	cookieName = 'FM';
	constructor(private cookieService: CookieService,
		private httpClient: HttpClient,
		private _notificationService: NotificationService) {
		this._notificationService.apiBaseUrl = this.apiServer.baseUrl;
	}

	getUserlist(parameters) {
		const parameter = {
			params: {
				'pageNo': parameters.pageNumber + 1,
				'pageSize': parameters.pageSize,
				'searchText': parameters.searchText,
				'orderBy': parameters.sortBy
			},
			timestamp: Date.now()
		};

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/user/search', parameter).map(response => {
			return response;
		});
	}

	getUserDetails(userGuid) {
		let currentUser = JSON.parse(localStorage.getItem('currentUser'));
		let isAdmin = currentUser.userDetail.isAdmin;
		if (isAdmin) {
			return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/adminuser/' + userGuid).map(response => {
				return response;
			});
		} else {
			return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/user/' + userGuid).map(response => {
				return response;
			});
		}


	}

	deleteUser(userGuid) {

		return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/user/delete/' + userGuid, "").map(response => {
			return response;
		});
	}
	// login
	login(data) {

		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/account/login', data).map(response => {
			if (response.isSuccess === true && response.data.access_token) {
				// store user details and jwt token in the local storage to keep the user logged in between page refreshes
				localStorage.setItem('currentUser', JSON.stringify(response.data));
			}
			return response;
		});

	}

	// logout
	logout() {
		// remove user from the local storage
		localStorage.removeItem('currentUser');
	}

	getroleList() {

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/role/company').map(response => {
			return response;
		});
	}

	getTimezoneList() {


		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/Lookup/timezone').map(response => {
			return response;
		});
	}

	addUser(data) {
		let currentUser = JSON.parse(localStorage.getItem('currentUser'));
		let isAdmin = currentUser.userDetail.isAdmin;
		if (isAdmin) {
			return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/adminuser/manage', data).map(response => {
				return response;
			});
		} else {
			return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/user/manage', data).map(response => {
				return response;
			});
		}


	}

	changeStatus(id, isActive) {
		let status = isActive == true ? false : true;
		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/user/updatestatus/' + id + '/' + status, {}).map(response => {
			return response;
		});
	}

	/**
	 * Change password of a user
	*/
	changePassword(data) {
		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/account/changepassword', data).map(response => {
			return response;
		});
	}
	// adminlogin
	loginadmin(data) {

		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/account/adminlogin', data).map(response => {
			if (response.isSuccess === true && response.data.access_token) {
				// store user details and jwt token in the local storage to keep the user logged in between page refreshes
				localStorage.setItem('currentUser', JSON.stringify(response.data));
			}
			return response;
		});

	}
	addadminUser(data) {

		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/adminuser/manage', data).map(response => {
			return response;
		});
	}
	getadminUserDetails(userGuid) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/adminuser/' + userGuid, configHeader).map(response => {
			return response;
		});
	}
	deleteadminUser(userGuid) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/adminuser/delete/' + userGuid, configHeader).map(response => {
			return response;
		});
	}
	adminchangeStatus(id, isActive) {
		let status = isActive == true ? false : true;
		return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/adminuser/updatestatus/' + id + '/' + status, {}).map(response => {
			return response;
		});
	}
	getAdminUserlist(parameters) {
		let currentUser = JSON.parse(localStorage.getItem('currentUser'));
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
				'orderBy': parameters.sortBy
			},
			timestamp: Date.now()
		};
		var reqParameter = Object.assign(parameter, configHeader);

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/adminuser/list', reqParameter).map(response => {
			return response;
		});
	}


	/* my code naren */

	getelevatorlist(parameters) {
		let currentUser = JSON.parse(localStorage.getItem('currentUser'));
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
				'orderBy': parameters.sortBy
			},
			timestamp: Date.now()
		};
		var reqParameter = Object.assign(parameter, configHeader);

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/device/search', reqParameter).map(response => {
			return response;
		});
	}
	gettemplatelookup() {

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/alltemplate').map(response => {
			return response;
		});

	}

	getBuildinglookup(parentEntityId) {

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/entity/search/?parentEntityId=' + parentEntityId).map(response => {
			return response;
		});

	}

	addUpdateEveletor(data) {
		const formData = new FormData();
		for (const key of Object.keys(data)) {
			const value = data[key];
			if (data[key])
				formData.append(key, value);
		}

		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/device/manage', formData).map(response => {
			return response;
		});
	}

	getelevatorDetails(elevetorGuid) {

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/device/' + elevetorGuid).map(response => {
			return response;
		});
	}
	ElevatorchangeStatus(id, isActive) {
		let status = isActive == true ? false : true;
		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/device/updatestatus/' + id + '/' + status, {}).map(response => {
			return response;
		});
	}

	getelevatorTelemetryData(templateGuid) {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/attributes/' + templateGuid).map(response => {
			return response;
		});
	}
	//key : UIAlert || LiveData
	getStompConfig(key) {
		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/configuration/' + key, '').map(response => {
			return response;
		});
	}
	getelevatorcountDetails(elevetorGuid) {

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/dashboard/getdevicedetail/' + elevetorGuid, {
			params: {
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

	getTripGraph(elevetorGuid) {

    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/chart/gettripsbyelevator/' + elevetorGuid).map(response => {
			return response;
		});
	}
}
