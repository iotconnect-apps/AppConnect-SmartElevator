import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { MatDialog } from '@angular/material'
import { DeleteDialogComponent } from '../../../components/common/delete-dialog/delete-dialog.component';
import { RolesService } from './../../../services/index'
import { Notification, NotificationService } from 'app/services';
import { AppConstant, DeleteAlertDataModel } from "../../../app.constants";


@Component({
  selector: 'app-roles-list',
  templateUrl: './roles-list.component.html',
  styleUrls: ['./roles-list.component.css']
})
export class RolesListComponent implements OnInit {

  currentUser = JSON.parse(localStorage.getItem("currentUser"));
  changeStatusRoleName: any;
  changeStatusRoleStatus: any;
  moduleName = "Roles";
  order = true;
  isSearch = false;
  totalRecords = 5;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  reverse = false;
  searchParameters = {
    pageNumber: 0,
    pageSize: 10,
    searchText: '',
    sortBy: 'name asc'
  };
  displayedColumns: string[] = ['name', 'description', 'isActive', 'action'];
  rolesList = [];
  deleteAlertDataModel: DeleteAlertDataModel;


  constructor(
    private spinner: NgxSpinnerService,
    private router: Router,
    public dialog: MatDialog,
    public rolesService: RolesService,
    private _notificationService: NotificationService,
    public _appConstant: AppConstant
  ) { }

  ngOnInit() {
    this.getRolesList();
  }

  /**
	* For Goto add role section
	**/
  clickAdd() {
    this.router.navigate(['/roles/add']);
  }

  /**
	* For set order role list
	**/
  setOrder(sort: any) {
    console.log(sort);
    if (!sort.active || sort.direction === '') {
      return;
    }
    this.searchParameters.sortBy = sort.active + ' ' + sort.direction;
    this.getRolesList();
  }

  /**
	* For open delete role confirmation model
	**/
  deleteModel(userModel: any) {
    this.deleteAlertDataModel = {
      title: "Delete Role",
      message: this._appConstant.msgConfirm.replace('modulename', "Role"),
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
        this.deleteRole(userModel.guid);
      }
    });
  }

  /**
	* For Manage role status
	**/
  activeInactiveRole(roleId: string, isActive: boolean, name: string) {
    var status = isActive == false ? this._appConstant.activeStatus : this._appConstant.inactiveStatus;
    var mapObj = {
      statusname: status,
      fieldname: name,
      modulename: "role"
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
        this.changeRoleStatus(roleId, isActive);

      }
    });

  }

  /**
	* For change role status
	**/
  changeRoleStatus(roleId, isActive) {
	
		this.spinner.show();
		this.rolesService.changeStatus(roleId, isActive).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
        this._notificationService.handleResponse({message:this._appConstant.msgStatusChange.replace("modulename", "Role")},"success");
				this.getRolesList();

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
	* For manage paggination
	**/
  ChangePaginationAsPageChange(pagechangeresponse) {
    this.searchParameters.pageSize = pagechangeresponse.pageSize;
    this.searchParameters.pageNumber = pagechangeresponse.pageIndex;
    this.isSearch = true;
    this.getRolesList();
  }

  /**
	* For search text call back
  **/
  searchTextCallback(filterText) {
    this.searchParameters.searchText = filterText;
    this.searchParameters.pageNumber = 0;
    this.isSearch = true;
    this.getRolesList();
  }

  /**
	* For Get Role List
	**/
  getRolesList() {
    this.spinner.show();
    this.rolesService.getRoles(this.searchParameters).subscribe(response => {
      this.spinner.hide();

      if (response.isSuccess === true) {
        this.totalRecords = response.data.count;
        // this.isSearch = false;
        this.rolesList = response.data.items;
      }
      else {
        this._notificationService.handleResponse(response,"error");
        this.rolesList = [];
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.handleResponse(error,"error");
    });
  }

  /**
	* For update role status
	**/
  updateRoleStatus(roleId, isActive) {

    this.spinner.show();
    this.rolesService.changeStatus(roleId, isActive).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this._notificationService.handleResponse({message:this._appConstant.msgStatusChange.replace("modulename", "Role")},"success");
        this.getRolesList();

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
	* For open delete role 
	**/
  deleteRole(roleId) {
    this.spinner.show();
    this.rolesService.deleteRole(roleId).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this._notificationService.handleResponse({message:this._appConstant.msgDeleted.replace("modulename", "Role")},"success");
        this.getRolesList();

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
