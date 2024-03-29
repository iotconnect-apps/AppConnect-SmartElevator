import { Component, OnInit } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner'
import { DashboardService, NotificationService, Notification } from '../../services';
import { ActivatedRoute } from '@angular/router';
import * as moment from 'moment-timezone'
@Component({
  selector: 'app-alerts',
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.css']
})
export class AlertsComponent implements OnInit {
  alerts = [];
  displayedColumns: string[] = ['message', 'buildingName', 'wingName', 'elevatorName', 'eventDate','severity'];
  order = true;
  isSearch = false;
  reverse = false;
  orderBy = 'name';
  pageSizeOptions: number[] = [5, 10, 25, 100];
  searchParameters = {
    pageNumber: 0,
    pageSize: 10,
    searchText: '',
    sortBy: 'eventDate desc',
    deviceGuid: '',
    entityGuid: '',
  };
  totalRecords = 0;
  constructor(
    private spinner: NgxSpinnerService,
    public dashboardService: DashboardService,
    private _notificationService: NotificationService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      if (params.elevatorGuid) {
        this.searchParameters.deviceGuid = params.elevatorGuid;
      }
      if (params.buildingGuid) {
        this.searchParameters.entityGuid = params.buildingGuid;
      }
    });
    this.getAlertList();
  }

  /**
	* For convert UTC date to local time zone
	**/
  getLocalDate(lDate) {
    var utcDate = moment.utc(lDate, 'YYYY-MM-DDTHH:mm:ss.SSS');
    // Get the local version of that date
    var localDate = moment(utcDate).local();
    let res = moment(localDate).format('MMM DD, YYYY hh:mm:ss A');
    return res;
    
    }

  /**
	* For Get Alert List
	**/
  getAlertList() {
    this.spinner.show();
    this.dashboardService.getAlertsList(this.searchParameters).subscribe(response => {
      this.spinner.hide();
      
      if (response.isSuccess === true && response.data.items) {
        this.alerts = response.data.items;
        this.totalRecords = response.data.count;
      }
      else {
        this.alerts = [];
        this._notificationService.handleResponse(response,"error");
        this.totalRecords = 0;
      }
    }, error => {
      this.alerts = [];
      this.totalRecords = 0;
      this._notificationService.handleResponse(error,"error");
    });
  }

  /**
	* For Set order of alerts
	**/
  setOrder(sort: any) {
    if (!sort.active || sort.direction === '') {
      return;
    }
    this.searchParameters.sortBy = sort.active + ' ' + sort.direction;
    this.getAlertList();
  }

  /**
	* For Manage Pagenation
	**/
  onPageSizeChangeCallback(pageSize) {
    this.searchParameters.pageSize = pageSize;
    this.searchParameters.pageNumber = 1;
    this.isSearch = true;
    this.getAlertList();
  }

  /**
	* For change page size for Pagenation
	**/
  ChangePaginationAsPageChange(pagechangeresponse) {
    this.searchParameters.pageNumber = pagechangeresponse.pageIndex;
    this.searchParameters.pageSize = pagechangeresponse.pageSize;
    this.isSearch = true;
    this.getAlertList();
  }

  /**
	* For call back of search text
	**/
  searchTextCallback(filterText) {
    this.searchParameters.searchText = filterText;
    this.searchParameters.pageNumber = 0;
    this.getAlertList();
    this.isSearch = true;
  }

}
