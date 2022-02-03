import { Component, OnInit, ViewChild } from '@angular/core'
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { MatDialog, MatTableDataSource, MatSort, MatPaginator } from '@angular/material'
import { DeleteDialogComponent } from '../../../../components/common/delete-dialog/delete-dialog.component';
import { DeviceService, NotificationService } from 'app/services';
import { Notification } from 'app/services/notification/notification.service';
import { AppConstant, DeleteAlertDataModel } from "../../../../app.constants";

@Component({ selector: 'app-hardware-kit-list', templateUrl: './hardware-kit-list.component.html', styleUrls: ['./hardware-kit-list.component.scss'] })

export class HardwareListComponent implements OnInit {
	changeStatusDeviceName: any;
	changeStatusDeviceStatus: any;
	white = true;
	assig = false;
	fieldshow = false;
	order = true;
	isSearch = false;
	pageSizeOptions: number[] = [5, 10, 25, 100];
	reverse = false;
	orderBy = 'guid';
	totalRecords = 0;
	searchParameters = {
		isAssigned: false,
		pageNo: 0,
		pageSize: 10,
		searchText: '',
		sortBy: 'guid asc'
	};
	displayedColumns: string[] = ['companyName', 'kitCode', 'kitTypeName', 'isActive', 'action'];
	dataSource = [];
	deleteAlertDataModel: DeleteAlertDataModel;

	constructor(
		private spinner: NgxSpinnerService,
		private router: Router,
		public dialog: MatDialog,
		private deviceService: DeviceService,
		private _notificationService: NotificationService,
		public _appConstant: AppConstant
	) { }

	ngOnInit() {
		this.getHardwarkitList();
	}

	/**
	* For Goto add HK section  
	**/
	clickAdd() {
		this.router.navigate(['admin/hardwarekits/add']);
	}

	/**
	* For Goto bulk upload HK section  
	**/
	clickBulk() {
		this.router.navigate(['admin/hardwarekits/bulkupload']);
	}

	/**
	* For Set order of HK list
	**/
	setOrder(sort: any) {
		if (!sort.active || sort.direction === '') {
			return;
		}
		this.searchParameters.sortBy = sort.active + ' ' + sort.direction;
		this.getHardwarkitList();
	}

	/**
	* For open HK delete confirmation model
	**/
	deleteModel(DeviceModel: any) {
		this.deleteAlertDataModel = {
			title: "Delete Device",
			message: this._appConstant.msgConfirm.replace('modulename', "device"),
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
				this.deleteDevice(DeviceModel.guid);
			}
		});
	}

	/**
	* For Manage paggination
	**/
	ChangePaginationAsPageChange(pagechangeresponse) {
		this.searchParameters.pageNo = pagechangeresponse.pageIndex;
		this.searchParameters.pageSize = pagechangeresponse.pageSize;
		this.isSearch = true;
		this.getHardwarkitList();
	}

	/**
	* For Search texbox call back
	**/
	searchTextCallback(filterText) {
		this.searchParameters.searchText = filterText;
		this.searchParameters.pageNo = 0;
		this.getHardwarkitList();
		this.isSearch = true;
	}

	/**
	* For Get HK list
	**/
	getHardwarkitList() {
		this.spinner.show();
		this.deviceService.getHardware(this.searchParameters).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this.totalRecords = response.data.count;
				this.dataSource = response.data.items;
			}
			else {
				this._notificationService.handleResponse(response,"error");
				this.dataSource = [];
			}
		}, error => {
			this.spinner.hide();
			this._notificationService.handleResponse(error,"error");
		});
	}

	/**
	* For Manage status of devices
	**/
	activeInactiveDevice(deviceId: string, isActive: boolean, name: string) {
		var status = isActive == false ? this._appConstant.activeStatus : this._appConstant.inactiveStatus;
		var mapObj = {
			statusname: status,
			fieldname: name,
			modulename: "device"
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
				this.changeDeviceStatus(deviceId, isActive);

			}
		});

	}

	/**
	* For Change devices status
	**/
	changeDeviceStatus(deviceId, isActive) {
		this.spinner.show();
		this.deviceService.changeStatus(deviceId, isActive).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this._notificationService.handleResponse({message:this._appConstant.msgStatusChange.replace("modulename", "Device")},"success");
				this.getHardwarkitList();

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
	* For Delete device
	**/
	deleteDevice(guid) {
		this.spinner.show();
		this.deviceService.deleteHW(guid).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this._notificationService.handleResponse({message:this._appConstant.msgDeleted.replace("modulename", "Hardware Kit")},"success");
				this.getHardwarkitList();
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
	* For manage tabs
	**/
	clickassigned(event) {
		if (event.tab.textLabel == 'Assigned') {
			this.fieldshow = true;
			this.assig = true;
			this.white = false;
			this.searchParameters.isAssigned = true;
		} else {
			this.fieldshow = false;
			this.assig = false;
			this.white = true;
			this.searchParameters.isAssigned = false;
		}

		//this.searchParameters.isAssigned = true;
		this.getHardwarkitList();
	}

	/**
	* For manage tabs
	**/
	clickwhitelist() {
		this.fieldshow = false;
		this.assig = false;
		this.white = true;
		this.searchParameters.isAssigned = false;
		this.getHardwarkitList();
	}

}
