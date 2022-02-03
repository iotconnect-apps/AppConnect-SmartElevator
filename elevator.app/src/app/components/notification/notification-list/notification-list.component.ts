import { Component, OnInit, ViewChild } from '@angular/core'
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { MatDialog, MatTableDataSource, MatSort, MatPaginator } from '@angular/material'
import { DeviceService, NotificationService, RuleService } from 'app/services';
import { AppConstant, DeleteAlertDataModel } from "../../../app.constants";
import { DeleteDialogComponent } from '../../../components/common/delete-dialog/delete-dialog.component';
import { Notification } from 'app/services/notification/notification.service';

@Component({
  selector: 'app-notification-list',
  templateUrl: './notification-list.component.html',
  styleUrls: ['./notification-list.component.css']
})
export class NotificationListComponent implements OnInit {
  notificationList = [];
  searchText = '';
  deleteAlertDataModel: DeleteAlertDataModel;
  constructor(
    private spinner: NgxSpinnerService,
    public dialog: MatDialog,
    private _notificationService: NotificationService,
    private ruleService: RuleService,
    public _appConstant: AppConstant
  ) { }

  ngOnInit() {
    this.getRuleList();
  }

  /**
	* For search text call back
	**/
  searchTextCallback($event) {
    this.searchText = $event;
    this.getRuleList();

  }

  /**
	* For open delete alert confirmation model
	**/
  deleteModel(RuleModel) {
    this.deleteAlertDataModel = {
      title: "Delete Notification",
      message: this._appConstant.msgConfirm.replace('modulename', RuleModel['name']),
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
        this.deleteRule(RuleModel['guid']);
      }
    });
  }

  /**
	* For delete rule
	**/
  deleteRule(guid) {
    this.spinner.show();
    this.ruleService.deleteUserRule(guid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess) {
        this._notificationService.handleResponse({message:"Notification deleted successfully."},"success");
        this.getRuleList();
      } else {
        this._notificationService.handleResponse(response,"error");
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.handleResponse({message:'Notification not found'},"error");
    });
  }

  /**
	* For Manage status of alert
	**/
  activeInactiveRule(id: string, isActive: boolean, name: string) {
    var status = isActive == true ? this._appConstant.activeStatus : this._appConstant.inactiveStatus;
    var mapObj = {
      statusname: status,
      fieldname: name,
      modulename: "Notification"
    };
    this.deleteAlertDataModel = {
      title: "Change Notification Status",
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
        this.updateRuleStatus(id, isActive);

      }
    });

  }

  /**
	* For update status of alert
	**/
  updateRuleStatus(guid, status) {
    this.spinner.show();
    this.ruleService.updateUserRuleStatus(guid, status).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess) {
        this._notificationService.handleResponse({message:"Notification status updated successfully."},"success");
        this.getRuleList();
      } else {
        this._notificationService.handleResponse(response,"error");
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.handleResponse(error,"error");
    });
  }

  /**
	* For get rule list
	**/
  getRuleList() {
    this.spinner.show();
    this.ruleService.getUserRuleList(this.searchText).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess) {
        this.notificationList = response.data.items;
      } else {
        this.notificationList = [];
      }
    }, error => {
      this.spinner.hide();
      this.notificationList = [];
    });
  }

}
