import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { NgxSpinnerService } from "ngx-spinner";
import { MatDialog } from "@angular/material";
import { DeleteDialogComponent } from "../../../components/common/delete-dialog/delete-dialog.component";
import { AppConstant, DeleteAlertDataModel } from "../../../app.constants";
import { BuildingService } from "../../../services/building/building.service";
import { Notification, NotificationService } from "../../../services";

@Component({
  selector: 'app-building-list',
  templateUrl: './building-list.component.html',
  styleUrls: ['./building-list.component.css']
})
export class BuildingListComponent implements OnInit {
  changeStatusDeviceName: any;
  changeStatusDeviceStatus: any;
  changeDeviceStatus: any;
  buildingList = [];
  moduleName = "Buildings";
  order = true;
  isSearch = false;
  totalRecords = 5;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  reverse = false;
  orderBy = "name";
  searchParameters = {
    parentEntityId: '',
    pageNumber: -1,
    pageSize: -1,
    searchText: "",
    sortBy: "name asc"
  };
  deleteAlertDataModel: DeleteAlertDataModel;
  displayedColumns: string[] = ["name", "address", "city", "zipcode", "description", "isActive", "action"];
  mediaUrl: any;

  constructor(
    private spinner: NgxSpinnerService,
    private router: Router,
    public dialog: MatDialog,
    public buildingService: BuildingService,
    public _appConstant: AppConstant,
    private _notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.mediaUrl = this._notificationService.apiBaseUrl;
    this.getbuildingList();
  }

  clickAdd() {
    this.router.navigate(["/building/add"]);
  }

  setOrder(sort: any) {
    console.log(sort);
    if (!sort.active || sort.direction === '') {
      return;
    }
    this.searchParameters.sortBy = sort.active + ' ' + sort.direction;
    this.getbuildingList();
  }

  deleteModel(userModel: any) {
    this.deleteAlertDataModel = {
      title: "Delete Building",
      message: this._appConstant.msgConfirm.replace('modulename', "Building"),
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
        this.deletebuilding(userModel.guid);
      }
    });
  }

  onPageSizeChangeCallback(pageSize) {
    this.searchParameters.pageSize = pageSize;
    this.searchParameters.pageNumber = 1;
    this.isSearch = true;
    this.getbuildingList();
  }

  activeInactivebuilding(id: string, isActive: boolean, name: string) {
    var status = isActive == false ? this._appConstant.activeStatus : this._appConstant.inactiveStatus;
    var mapObj = {
      statusname: status,
      fieldname: name,
      modulename: "Building"
    };
    this.deleteAlertDataModel = {
      title: "Status",
      message: this._appConstant.msgStatusConfirm.replace(/statusname|fieldname/gi, function (matched) {
        return mapObj[matched];
      }),
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
        this.changeBuildingStatus(id, isActive);

      }
    });

  }

  ChangePaginationAsPageChange(pagechangeresponse) {
    this.searchParameters.pageNumber = pagechangeresponse.pageIndex;
    this.searchParameters.pageSize = pagechangeresponse.pageSize;
    this.isSearch = true;
    this.getbuildingList();
  }

  searchTextCallback(filterText) {
    this.searchParameters.searchText = filterText;
    this.searchParameters.pageNumber = 0;
    this.getbuildingList();
    this.isSearch = true;
  }

  getbuildingList() {
    this.spinner.show();
    this.buildingService.getBuilding(this.searchParameters).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.totalRecords = response.data.count;
        // this.isSearch = false;

        if (response.data.count) {
          this.buildingList = response.data.items;
        } else {
          this.buildingList = [];
        }
      }
      else {
        this._notificationService.add(new Notification('error', response.message));
        this.buildingList = [];
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  deletebuilding(guid) {
    this.spinner.show();
    this.buildingService.deleteBuilding(guid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this._notificationService.add(new Notification('success', this._appConstant.msgDeleted.replace("modulename", "Building")));
        this.getbuildingList();

      }
      else {
        this._notificationService.add(new Notification('error', response.message));
      }

    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  changeBuildingStatus(id, isActive) {

    this.spinner.show();
    this.buildingService.changeStatus(id, isActive).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this._notificationService.add(new Notification('success', this._appConstant.msgStatusChange.replace("modulename", "Building")));
        this.getbuildingList();

      }
      else {
        this._notificationService.add(new Notification('error', response.message));
      }

    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }
}
