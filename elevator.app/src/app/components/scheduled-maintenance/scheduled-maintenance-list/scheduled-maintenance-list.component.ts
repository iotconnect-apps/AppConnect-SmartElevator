import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DeleteAlertDataModel, AppConstant } from '../../../app.constants';
import { DeleteDialogComponent } from '../..';
import { MatDialog } from '@angular/material';
import { NgxSpinnerService } from 'ngx-spinner';
import { ScheduledMaintenanceService, NotificationService, Notification } from '../../../services';
import * as moment from 'moment'
@Component({
  selector: 'app-scheduled-maintenance-list',
  templateUrl: './scheduled-maintenance-list.component.html',
  styleUrls: ['./scheduled-maintenance-list.component.css']
})
export class ScheduledMaintenanceListComponent implements OnInit {

  isSearch = false;
  moduleName = "Scheduled Maintenance";
  totalRecords = 0;
  order = true;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  reverse = false;
  orderBy = "name";
  searchParameters = {
    entityGuid: '',
    pageNumber: 0,
    pageSize: 10,
    searchText: '',
    sortBy: 'startDateTime desc'
  };
  deleteAlertDataModel: DeleteAlertDataModel;
  displayedColumns: string[] = ['building', 'wing', 'name', 'startDateTime', 'endDateTime', 'status', 'actions'];
  scheduledMaintenanceList = [];

  constructor(private router: Router,
    public _appConstant: AppConstant,
    public dialog: MatDialog,
    private spinner: NgxSpinnerService,
    private _service: ScheduledMaintenanceService,
    private _notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.getMaintenanceList();
  }

  getLocaleDate(date) {
    // var now = moment(date).format('YYYY-MM-DD HH:mm:ss');
    // return moment.utc(now).local().format('MM/DD/YYYY');

    var stillUtc = moment.utc(date).toDate();
    var local = moment(stillUtc).local().format('MMM DD, YYYY hh:mm:ss A');
    return local;
  }

  /**
   * Search text from list
   * @param filterText
   */
  searchTextCallback(filterText) {
    this.searchParameters.searchText = filterText;
    this.searchParameters.pageNumber = 0;
    this.getMaintenanceList();
    this.isSearch = true;
  }

  /**
   * Manage maintenance schedule
   * */
  scheduleMaintenance() {
    this.router.navigate(['/maintenance/add']);
  }

  /**
   * For open delete maintenance confirmation model
   * */
  deleteModel(model: any) {
    this.deleteAlertDataModel = {
      title: "Delete Scheduled Maintenance",
      message: this._appConstant.msgConfirm.replace('modulename', "Scheduled Maintenance"),
      okButtonName: "Yes",
      cancelButtonName: "No",
    };
    const dialogRef = this.dialog.open(DeleteDialogComponent, {
      width: '400px',
      height: 'auto',
      data: this.deleteAlertDataModel,
      disableClose: false
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteMaintenance(model.guid);
      }
    });
  }

  /**
   * For delete maintenance
   * */
  deleteMaintenance(guid) {
    this.spinner.show();
    this._service.deleteMaintenance(guid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this._notificationService.handleResponse({message:this._appConstant.msgDeleted.replace("modulename", "Scheduled Maintenance")},"success");
        this.getMaintenanceList();
      }
      else {
        this._notificationService.handleResponse(response,"error");
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.handleResponse(error,"error");
    });
  }

  /**
   * For handle paggination call back
   * */
  onPageSizeChangeCallback(pageSize) {
    this.searchParameters.pageSize = pageSize;
    this.searchParameters.pageNumber = 1;
    this.isSearch = true;
    this.getMaintenanceList();
  }

  /**
   * For get maintenance list
   * */
  getMaintenanceList() {
    this.spinner.show();
    this._service.getScheduledMaintenanceList(this.searchParameters).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.totalRecords = response.data.count;
        // this.isSearch = false;
        if (response.data.count) {
          this.scheduledMaintenanceList = response.data.items;
        } else {
          this.scheduledMaintenanceList = [];
        }
        //console.log(this.scheduledMaintenanceList);
      }
      else {
        this._notificationService.handleResponse(response,"error");
        this.scheduledMaintenanceList = [];
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.handleResponse(error,"error");
    });
  }

  /**
   * For set order of schedule list
   * */
  setOrder(sort: any) {
    console.log(sort);
    if (!sort.active || sort.direction === '') {
      return;
    }
    if (sort.active == 'name') {
      sort.active = 'elevatorName';
    }
    this.searchParameters.sortBy = sort.active + ' ' + sort.direction;

    this.getMaintenanceList();

  }

  /**
   * For manage paggination
   * */
  ChangePaginationAsPageChange(pagechangeresponse) {
    this.searchParameters.pageSize = pagechangeresponse.pageSize;
    this.searchParameters.pageNumber = pagechangeresponse.pageIndex;
    this.isSearch = true;
    this.getMaintenanceList();
  }
}
