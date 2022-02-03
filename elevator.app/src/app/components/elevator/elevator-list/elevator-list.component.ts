import { Component, OnInit, ViewChild } from '@angular/core'
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { MatDialog, MatPaginator, MatSort, MatTableDataSource } from '@angular/material'
import { DeleteDialogComponent } from '../../../components/common/delete-dialog/delete-dialog.component';
import { tap } from 'rxjs/operators';
import { AppConstant, DeleteAlertDataModel } from "../../../app.constants";
import { empty } from 'rxjs';
import { UserService, Notification, NotificationService, ElevatorService } from '../../../services';

@Component({
	selector: 'app-elevator-list',
	templateUrl: './elevator-list.component.html',
	styleUrls: ['./elevator-list.component.css']
})

export class ElevatorListComponent implements OnInit {
	changeStatusDeviceName: any;
	changeStatusDeviceStatus: any;
	changeDeviceStatus: any;
	deleteAlertDataModel: DeleteAlertDataModel;
	currentUser = JSON.parse(localStorage.getItem("currentUser"));
	elevatorList = [];
	totalRecords = 0;
	pageSizeOptions: number[] = [5, 10, 25, 100];
	moduleName = "Elevators";
  displayedColumns: string[] = ['uniqueId','name', 'building', 'wing', 'isProvisioned'];
	order = true;
	isSearch = false;
	reverse = false;
	orderBy = 'name';
	searchParameters = {
		pageNumber: 0,
		pageSize: 10,
		searchText: '',
		sortBy: 'name asc'
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
		private _notificationService: NotificationService,
		private elevatorService: ElevatorService,

	) { }

	ngOnInit() {
		this.getelevator();
	}

	/**
    * For Filter data 
	**/
	applyFilter(filterValue: string) {
		filterValue = filterValue.trim(); // Remove whitespace
		filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
		this.dataSource.filter = filterValue;
	}

	/**
    * For goto add elevator section
	**/
	clickAdd() {
		this.router.navigate(['elevators/add']);
	}

	/**
    * For set order of elevator list
	**/
	setOrder(sort: any) {
		console.log(sort);
		if (!sort.active || sort.direction === '') {
			return;
		}
		this.searchParameters.sortBy = sort.active + ' ' + sort.direction;
		this.getelevator();
	}

	/**
    * For Manage Paggination
	**/
	onPageSizeChangeCallback(pageSize) {
		this.searchParameters.pageSize = pageSize;
		this.searchParameters.pageNumber = 1;
		this.isSearch = true;
		this.getelevator();
	}

	/**
    * For Change paggination page
	**/
	ChangePaginationAsPageChange(pagechangeresponse) {
		this.searchParameters.pageNumber = pagechangeresponse.pageIndex;
		this.searchParameters.pageSize = pagechangeresponse.pageSize;
		this.isSearch = true;
		this.getelevator();
	}

	/**
	* For search text call back
	**/
	searchTextCallback(filterText) {
		this.searchParameters.searchText = filterText;
		this.searchParameters.pageNumber = 0;
		this.getelevator();
		this.isSearch = true;
	}

	/**
    * For Get elevator list
	**/
	getelevator() {
		this.spinner.show();
		this.elevatorService.getelevatorlist(this.searchParameters).subscribe(response => {
			this.spinner.hide();
			this.totalRecords = response.data.count;
			if (response.data.count) {
				this.elevatorList = response.data.items;
			} else {
				this.elevatorList = [];
			}



		}, error => {
			this.spinner.hide();
			this._notificationService.handleResponse(error,"error");
		});
	}

	onKey(filterValue: string) {
		this.applyFilter(filterValue);
	}

	/**
    * For open delete user confirmation model
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
    * For delete user
	**/
	deleteuser(guid) {
		this.spinner.show();
		this.userService.deleteadminUser(guid).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this._notificationService.handleResponse({message:this._appConstant.msgDeleted.replace("modulename", "User")},"success");
				this.getelevator();

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
    * For manage elevator status
	**/
	activeInactiveelevator(id: string, isActive: boolean, name: string) {
		var status = isActive == false ? this._appConstant.activeStatus : this._appConstant.inactiveStatus;
		var mapObj = {
			statusname: status,
			fieldname: name,
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
				this.changeElevatorStatus(id, isActive);

			}
		});

	}

	/**
    * For change elevator status
	**/
	changeElevatorStatus(id, isActive) {

		this.spinner.show();
		this.elevatorService.ElevatorchangeStatus(id, isActive).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this._notificationService.handleResponse({message:this._appConstant.msgStatusChange.replace("modulename", "Elevator")},"success");
				this.getelevator();

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
