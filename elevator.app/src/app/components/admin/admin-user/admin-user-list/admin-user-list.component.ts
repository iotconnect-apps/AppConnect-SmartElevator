import { Component, OnInit, ViewChild } from '@angular/core'
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { MatDialog, MatPaginator, MatSort, MatTableDataSource } from '@angular/material'
import { DeleteDialogComponent } from '../../../../components/common/delete-dialog/delete-dialog.component';
import { tap } from 'rxjs/operators';
import { AppConstant, DeleteAlertDataModel } from "../../../../app.constants";
import { Notification, NotificationService, UserService } from 'app/services';
import { empty } from 'rxjs';

@Component({
  selector: 'app-admin-user-list',
  templateUrl: './admin-user-list.component.html',
  styleUrls: ['./admin-user-list.component.css']
})
export class AdminUserListComponent implements OnInit {
  changeStatusDeviceName: any;
  changeStatusDeviceStatus: any;
  changeDeviceStatus: any;
  deleteAlertDataModel: DeleteAlertDataModel;
  currentUser = JSON.parse(localStorage.getItem("currentUser"));
  userList = [];
  totalRecords = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  moduleName = "Users";
  displayedColumns: string[] = ['firstName', 'lastName', 'email', 'contactNo', 'isActive', 'action'];
  order = true;
  isSearch = false;
  reverse = false;
  orderBy = 'name';
  searchParameters = {
    pageNumber: 0,
    pageSize: 10,
    searchText: '',
    sortBy: 'firstName asc'
  };
  dataSource: MatTableDataSource<any>;
  @ViewChild('paginator', { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;

  constructor(
    public dialog: MatDialog,
    private spinner: NgxSpinnerService,
    private router: Router,
    private userService: UserService,
    public _appConstant: AppConstant,
    private _notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.getUserList();
  }

  /**
  * For manage user data with filter 
  **/
  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  /**
  * For Goto add user section 
  **/
  clickAdd() {
    this.router.navigate(['admin/users/add']);
  }

  /**
  * For Set user list order 
  **/
  setOrder(sort: any) {
    if (!sort.active || sort.direction === '') {
      return;
    }
    this.searchParameters.sortBy = sort.active + ' ' + sort.direction;
    this.getUserList();
  }

  /**
  * For Manage Paggination  
  **/
  onPageSizeChangeCallback(pageSize) {
    this.searchParameters.pageSize = pageSize;
    this.searchParameters.pageNumber = 1;
    this.isSearch = true;
    this.getUserList();
  }

  /**
  * For Change Paggination value  
  **/
  ChangePaginationAsPageChange(pagechangeresponse) {
    this.searchParameters.pageNumber = pagechangeresponse.pageIndex;
    this.searchParameters.pageSize = pagechangeresponse.pageSize;
    this.isSearch = true;
    this.getUserList();
  }

  /**
  * For Search Call Back  
  **/
  searchTextCallback(filterText) {
    this.searchParameters.searchText = filterText;
    this.searchParameters.pageNumber = 0;
    this.getUserList();
    this.isSearch = true;
  }


  /**
  * For Get User List  
  **/
  getUserList() {
    this.spinner.show();
    this.userService.getAdminUserlist(this.searchParameters).subscribe(response => {
      this.spinner.hide();
      this.totalRecords = response.data.count;
      this.userList = response.data.items;

    }, error => {
      this.spinner.hide();
      this._notificationService.handleResponse(error,"error");
    });
  }

  /**
  * For Open delete confirmation model  
  **/
  deleteModel(userModel: any) {
    this.deleteAlertDataModel = {
      title: "Delete User",
      message: this._appConstant.msgConfirm.replace('modulename', "User"),
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
        this.deleteuser(userModel.guid);
      }
    });
  }

  /**
  * For delete user by id  
  **/
  deleteuser(guid) {
    this.spinner.show();
    this.userService.deleteadminUser(guid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this._notificationService.handleResponse({message:this._appConstant.msgDeleted.replace("modulename", "User")},"success");
        this.getUserList();

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
  * For Manage Status (Active / Inactive)
  **/
  activeInactiveuser(id: string, isActive: boolean, fname: string, lname: string) {
    var status = isActive == false ? this._appConstant.activeStatus : this._appConstant.inactiveStatus;
    var mapObj = {
      statusname: status,
      fieldname: fname + " " + lname,
      modulename: ""
    };
    this.deleteAlertDataModel = {
      title: "Status",
      message: this._appConstant.msgStatusConfirm.replace(/statusname|fieldname|modulename/gi, function (matched) {
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
        this.changeUserStatus(id, isActive);

      }
    });

  }

  /**
  * For Manage usercStatus
  **/
  changeUserStatus(id, isActive) {
    this.spinner.show();
    this.userService.adminchangeStatus(id, isActive).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this._notificationService.handleResponse({message:this._appConstant.msgStatusChange.replace("modulename", "User")},"success");
        this.getUserList();
      }
      else {
        this._notificationService.handleResponse(response,"error");
      }

    }, error => {
      this.spinner.hide();
      this._notificationService.handleResponse(error,"error");
    });
  }
}
