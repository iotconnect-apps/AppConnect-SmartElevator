import * as moment from 'moment-timezone'
import { Component, OnInit } from '@angular/core'
import { NgxSpinnerService } from 'ngx-spinner'
import { DashboardService, UserService, NotificationService, Notification } from '../../services';

/*Dynamic Dashboard Code*/
import {ChangeDetectorRef , EventEmitter, ViewChild} from '@angular/core';
import { DynamicDashboardService, DeviceService, ElevatorService, ScheduledMaintenanceService } from 'app/services';
import {DisplayGrid, CompactType, GridsterConfig, GridsterItem, GridsterItemComponent, GridsterPush, GridType, GridsterComponentInterface, GridsterItemComponentInterface} from 'angular-gridster2';
import { HttpClient } from '@angular/common/http';
import { Observable, Subscription } from 'rxjs';
/*Dynamic Dashboard Code*/

@Component({
	selector: 'app-dashboard',
	templateUrl: './dashboard.component.html',
	styleUrls: ['./dashboard.component.css'],
})

export class DashboardComponent implements OnInit {
	pieChartObj = {
		chartType: 'PieChart',
		options: {
			title: 'Alerts',
			width: 110,
			height: 110,
			pieHole: 0.65,
			pieSliceText: 'value',
			legend: 'none',
			chartArea: { width: '90%', height: '90%' },
			tooltip: { text: 'value', trigger: 'selection' },
			backgroundColor: { fill: 'transparent' },
			slices: {
				0: { color: '#f25450' },
				1: { color: '#000000' },
				2: { color: '#039be5' }
			},
		},
		dataTable: [],
		total: 0
	}
	isShowLeftMenu = true;
	overView = {
		buildingCount: 0,
		elevatorsCount: {
			connected: 0,
			total: 0,
			alerts:0
		},
		energy: {
			totalConsumption: 0,
			elevators: []
		},
		maintenanceStats: {
			underMaintenanceCount: 0,
			requiredMaintenanceCount: 0
		},
		totalUserCount : 0,
		activeUserCount : 0,
		inactiveUserCount : 0,
	};
	maintenanceList = [];
	alerts = [];
	buildings = [];
	buildingsDetail = {}
	comboChart = {
		chartType: "ComboChart",
		dataTable: [],
		options: {
			title: "",
			vAxis: {
				title: "% Availablity",
				titleTextStyle: {
					bold: true
				},
				viewWindow: {
					min: 0
				}
			},
			hAxis: {
				//title: "pH Level",
				titleTextStyle: {
					bold: true
				},
			},
			legend: { position: "bottom", alignment: "start" },
			height: "400",
			seriesType: 'bars',
			series: { 1: { type: 'line' } }
		}
	};

	companyId: any;
	type: any;

	/*Dynamic Dashboard Code*/
	@ViewChild('gridster',{static:false}) gridster;
	isDynamicDashboard : boolean = true;
	options: GridsterConfig;
	dashboardWidgets: Array<any> = [];
	dashboardList = [];
	dashboardData = {
		id : '',
		index : 0,
		dashboardName : '',
		isDefault : false,
		widgets : []
	};
	resizeEvent: EventEmitter<any> = new EventEmitter<any>();
	alertLimitchangeEvent: EventEmitter<any> = new EventEmitter<any>();
	chartTypeChangeEvent: EventEmitter<any> = new EventEmitter<any>();
	zoomChangeEvent: EventEmitter<any> = new EventEmitter<any>();
	telemetryDeviceChangeEvent: EventEmitter<any> = new EventEmitter<any>();
	telemetryAttributeChangeEvent: EventEmitter<any> = new EventEmitter<any>();
	sideBarSubscription : Subscription;
	deviceData: any = [];
	/*Dynamic Dashboard Code*/

	constructor(
		private spinner: NgxSpinnerService,
		public dashboardService: DashboardService,
		public usersService: UserService,
		private _notificationService: NotificationService,
		public dynamicDashboardService: DynamicDashboardService,
		private changeDetector: ChangeDetectorRef,
		private deviceService: DeviceService,
		private scheduledMaintenanceService : ScheduledMaintenanceService,
		private elevatorService : ElevatorService,
		) {
    /*Dynamic Dashboard Code*/
    this.sideBarSubscription = this.dynamicDashboardService.isToggleSidebarObs.subscribe((toggle) => {
      console.log("Sidebar clicked");
      if (this.isDynamicDashboard && this.dashboardList.length > 0) {
        this.spinner.show();
        this.changedOptions();
        let cond = false;
        Observable.interval(700)
          .takeWhile(() => !cond)
          .subscribe(i => {
            console.log("Grid Responsive");
            cond = true;
            this.checkResponsiveness();
            this.spinner.hide();
          });
      }
    })
		/*Dynamic Dashboard Code*/
	}

	ngOnInit() {
		this.type = "d";
		let currentUser = JSON.parse(localStorage.getItem('currentUser'));
		this.companyId = currentUser.userDetail.companyId;
		this.getDashbourdCount();
		this.getElevatorList();
		/*Dynamic Dashboard Code*/
		this.options = {
			gridType: GridType.Fixed,
			displayGrid: DisplayGrid.Always,
			initCallback: this.gridInit.bind(this),
			itemResizeCallback: this.itemResize.bind(this),
			fixedColWidth: 20,
			fixedRowHeight: 20,
			keepFixedHeightInMobile: false,
			keepFixedWidthInMobile: false,
			mobileBreakpoint: 640,
			pushItems: false,
			draggable: {
				enabled: false
			},
			resizable: {
				enabled: false
			},
			enableEmptyCellClick: false,
			enableEmptyCellContextMenu: false,
			enableEmptyCellDrop: false,
			enableEmptyCellDrag: false,
			enableOccupiedCellDrop: false,
			emptyCellDragMaxCols: 50,
			emptyCellDragMaxRows: 50,

			minCols: 60,
			maxCols: 192,
			minRows: 59,
			maxRows: 375,
			setGridSize: true,
			swap: true,
			swapWhileDragging: false,
			compactType: CompactType.None,
			margin : 0,
			outerMargin : true,
			outerMarginTop : null,
			outerMarginRight : null,
			outerMarginBottom : null,
			outerMarginLeft : null,
		};
		/*Dynamic Dashboard Code*/
	}

	ngOnDestroy(): void {
		this.sideBarSubscription.unsubscribe();
	}

	/*Dynamic Dashboard Code*/
	getDashboards(){
		this.spinner.show();
		this.dashboardList = [];
		let isAnyDefault = false;
		let systemDefaultIndex = 0;
		this.dynamicDashboardService.getUserWidget().subscribe(response => {
			this.isDynamicDashboard = false;
			for (var i = 0; i <= (response.data.length - 1); i++) {
				response.data[i].id = response.data[i].guid;
				response.data[i].widgets = JSON.parse(response.data[i].widgets);
				this.dashboardList.push(response.data[i]);
				if(response.data[i].isDefault === true){
					isAnyDefault = true;
					this.dashboardData.index = i;
					this.isDynamicDashboard = true;
				}
				if(response.data[i].isSystemDefault === true){
					systemDefaultIndex = i;
				}
			}
			/*Display Default Dashboard if no data*/
			if(!isAnyDefault){
				this.dashboardData.index = systemDefaultIndex;
				this.isDynamicDashboard = true;
				this.dashboardList[systemDefaultIndex].isDefault = true;
			}
			/*Display Default Dashboard if no data*/
			this.spinner.hide();
			if(this.isDynamicDashboard){
				this.editDashboard('view','n');
			}
			else{
				this.getBuildingList();
				this.getMaintenanceList();
				this.getAlertList();
			}
		}, error => {
			this.spinner.hide();
			/*Load Old Dashboard*/
			this.isDynamicDashboard = false;
			this.getBuildingList();
			this.getMaintenanceList();
			this.getAlertList();
			/*Load Old Dashboard*/
			this._notificationService.handleResponse(error,"error");
		});
	}

	editDashboard(type : string = 'view',is_cancel_btn : string = 'n'){
		this.spinner.show();
		this.dashboardWidgets = [];

		this.dashboardData.id = '';
		this.dashboardData.dashboardName = '';
		this.dashboardData.isDefault = false;
		for (var i = 0; i <= (this.dashboardList[this.dashboardData.index].widgets.length - 1); i++) {
			this.dashboardWidgets.push(this.dashboardList[this.dashboardData.index].widgets[i]);
		}

		if (this.options.api && this.options.api.optionsChanged) {
			this.options.api.optionsChanged();
		}
		this.spinner.hide();
	}

  gridInit(grid: GridsterComponentInterface) {
    if (this.options.api && this.options.api.optionsChanged) {
      this.options.api.optionsChanged();
    }
    let cond = false;
    Observable.interval(500)
      .takeWhile(() => !cond)
      .subscribe(i => {
        cond = true;
        this.checkResponsiveness();
      });
  }

  checkResponsiveness() {
    if (this.gridster) {
      let fixedColWidth = 20;
      let tempWidth = parseFloat((((this.gridster.curWidth * fixedColWidth) / (fixedColWidth * this.gridster.columns)).toFixed(2)).toString());
      tempWidth = (tempWidth - 0.01);
      if (this.gridster.curWidth >= 640) {
        this.options.fixedColWidth = tempWidth;
      }
      else {
        this.options.fixedColWidth = fixedColWidth;
      }
      for (var i = 0; i <= (this.dashboardWidgets.length - 1); i++) {
        if (this.gridster.curWidth < 640) {
          for (var g = 0; g <= (this.gridster.grid.length - 1); g++) {
            if (this.gridster.grid[g].item.id == this.dashboardWidgets[i].id) {
              this.dashboardWidgets[i].properties.w = this.gridster.grid[g].el.clientWidth;
            }
          }
        }
        else {
          this.dashboardWidgets[i].properties.w = (tempWidth * this.dashboardWidgets[i].cols);
        }
        this.resizeEvent.emit(this.dashboardWidgets[i]);
      }
      this.changedOptions();
      this.changeDetector.detectChanges();
    }
  }

	changedOptions() {
		if (this.options.api && this.options.api.optionsChanged) {
			this.options.api.optionsChanged();
		}
	}

	itemResize(item: any, itemComponent: GridsterItemComponentInterface) {
		this.resizeEvent.emit(item);
	}

	deviceSizeChange(size){
		this.checkResponsiveness();
	}

	getElevatorList(){
		this.spinner.show();
		this.deviceData = [];
		this.dynamicDashboardService.getElevatorLookupByCompany().subscribe(response => {
			if (response.isSuccess === true && response.data.items.length > 0){
				this.deviceData = response.data.items;
			}
			else
				this._notificationService.handleResponse(response,"error");
			this.changeDetector.detectChanges();
			this.getDashboards();
		}, error => {
			this.spinner.hide();
			this._notificationService.handleResponse(error,"error");
			this.changeDetector.detectChanges();
		});
	}
	/*Dynamic Dashboard Code*/

  /**
  * For get order list
  */
  getAlertList() {
  	let parameters = {
  		pageNumber: 0,
  		pageSize: 10,
  		searchText: '',
  		sortBy: 'eventDate desc',
  		deviceGuid: '',
  		entityGuid: '',
  	};
  	this.spinner.show();
  	this.dashboardService.getAlertsList(parameters).subscribe(response => {
  		this.spinner.hide();
  		if (response.isSuccess === true && response.data.items) {
  			this.alerts = response.data.items;
  		}
  		else {
  			this.alerts = [];
  			this._notificationService.handleResponse(response,"error");

  		}
  	}, error => {
  		this.alerts = [];
  		this._notificationService.handleResponse(error,"error");
  	});
  }

  /**
  * For convert UTC date to local time zone date
  */
  getLocalDate(lDate) {
  	var utcDate = moment.utc(lDate, 'YYYY-MM-DDTHH:mm:ss.SSS');
  	// Get the local version of that date
  	var localDate = moment(utcDate).local();
  	let res = moment(localDate).format('MMM DD, YYYY hh:mm:ss A');
  	return res;

  }

  /**
  * For Get dashboard overview and counts
  */
  getDashbourdCount() {
  	this.spinner.show();
  	this.dashboardService.getDashboardoverview().subscribe(response => {
  		if (response.isSuccess === true) {
  			this.overView = {
  				buildingCount: (response.data['totalBuilding']) ? response.data['totalBuilding'] : 0,
  				elevatorsCount: {
  					connected: (response.data['totalConnectedElevators']) ? response.data['totalConnectedElevators'] : 0,
  					total: (response.data['totalElevators']) ? response.data['totalElevators'] : 0,
  					alerts: (response.data['totalAlert']) ? response.data['totalAlert'] : 0
  				},
  				energy: {
  					totalConsumption: (response.data['totalEnergyCount']) ? response.data['totalEnergyCount'] : 0,
  					elevators: [
  					]
  				},
  				maintenanceStats: {
  					underMaintenanceCount: (response.data['totalUnderMaintenanceCount']) ? response.data['totalUnderMaintenanceCount'] : 0,
  					requiredMaintenanceCount: (response.data['totalScheduledCount']) ? response.data['totalScheduledCount'] : 0
  				},
  				totalUserCount : (response.data.totalUserCount) ? response.data.totalUserCount : 0,
				activeUserCount : (response.data.activeUserCount) ? response.data.activeUserCount : 0,
				inactiveUserCount : (response.data.inactiveUserCount) ? response.data.inactiveUserCount : 0
  			};
  			if (response.data['minElevatorName'] && response.data['minElevatorName']) {
  				this.overView['energy']['elevators'].push({ name: response.data['minElevatorName'], consumption: response.data['minElevatorEnergyCount'] });
  				this.overView['energy']['elevators'].push({ name: response.data['maxElevatorName'], consumption: response.data['maxElevatorEnergyCount'] });
  			}
  			if (response.data['alerts'] != '0' && response.data['alerts']['Critical'] != '0' && response.data['alerts']['Information'] != '0' && response.data['alerts']['Major'] != '0'
  				&& response.data['alerts']['Minor'] != '0' && response.data['alerts']['Warning'] != '0') {
  				this.pieChartObj.dataTable = [
  			['Alerts', 'Count'],
  			['Critical', parseInt(response.data['alerts']['Critical'])],
  			['Information', parseInt(response.data['alerts']['Information'])],
  			['Major', parseInt(response.data['alerts']['Major'])],
  			['Minor', parseInt(response.data['alerts']['Minor'])],
  			['Warning ', parseInt(response.data['alerts']['Warning'])]
  			];
  		}

  	}
  	else {
  		this.overView = {
  			buildingCount: 0,
  			elevatorsCount: {
  				connected: 0,
  				total: 0,
  				alerts:0
  			},
  			energy: {
  				totalConsumption: 0,
  				elevators: [
  				]
  			},
  			maintenanceStats: {
  				underMaintenanceCount: 0,
  				requiredMaintenanceCount: 0
  			},
  			totalUserCount : 0,
  			activeUserCount : 0,
  			inactiveUserCount : 0,
  		};
  		this.pieChartObj.dataTable = [];
  		this._notificationService.handleResponse(response,"error");

  	}
  	this.changeDetector.detectChanges();
  }, error => {
  	this.spinner.hide();
  	this._notificationService.handleResponse(error,"error");
  });
  }

  /**
  * For Get building List
  */
  getBuildingList() {
  	this.spinner.show();
  	this.dashboardService.getBuildingLookup(this.companyId).subscribe(response => {
  		this.spinner.hide();
  		if (response.isSuccess === true) {
  			this.buildings = response.data;
  			if (this.buildings.length) {
  				this.getBuildingOverview(this.buildings[0]['value'],this.type);
  				this.getBuildingGraph(this.buildings[0]['value'],'',this.type);
  			}
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
  * For Get building overview
  */
  getBuildingOverview(buildingId, type) {
  	this.spinner.show();
  	this.dashboardService.getBuildingOverview(buildingId, type).subscribe(response => {
  		this.spinner.hide();
  		if (response.isSuccess === true) {
  			this.buildingsDetail['totalElevator'] = (response.data['totalElevator']) ? response.data['totalElevator'] : 0;
  			this.buildingsDetail['totalOperatingHours'] = (response.data['totalOperatingHours']) ? response.data['totalOperatingHours'] : 0;
  			this.buildingsDetail['totalTrips'] = (response.data['totalTrips']) ? response.data['totalTrips'] : 0;
  			this.buildingsDetail['totalEnergy'] = (response.data['totalEnergy']) ? response.data['totalEnergy'] : 0;
  		}
  		else {
  			this.buildingsDetail['totalElevator'] = 0;
  			this.buildingsDetail['totalOperatingHours'] = 0;
  			this.buildingsDetail['totalTrips'] = 0;
  			this.buildingsDetail['totalEnergy'] = 0;
  			this._notificationService.handleResponse(response,"error");

  		}
  	}, error => {
  		this.buildingsDetail['totalElevator'] = 0;
  		this.buildingsDetail['totalOperatingHours'] = 0;
  		this.buildingsDetail['totalTrips'] = 0;
  		this.buildingsDetail['totalEnergy'] = 0;
  		this.spinner.hide();
  		this._notificationService.handleResponse(error,"error");
  	});
  }

  /**
  * For Get building overview
  */
  getBuildingGraph(buildingId, elevatorId = '', type) {
  	this.spinner.show();
  	this.dashboardService.getBuildingGraph(buildingId, type).subscribe(response => {
  		this.spinner.hide();
  		if (response.isSuccess === true) {
  			let data = [];
  			if (response.data.length) {
  				data.push(["WeekDays", "Operating Hours", "Energy Consumption"])

  				response.data.forEach(element => {
  					data.push([element.name, parseFloat(element.operatingHours), parseFloat(element.energyConsumption)])
  				});
  			}
  			this.comboChart = {
  				chartType: "ComboChart",
  				dataTable: data,
  				options: {
  					title: "",
  					vAxis: {
  						title: "Hours && KW",
  						titleTextStyle: {
  							bold: true
  						},
  						viewWindow: {
  							min: 0
  						}
  					},
  					hAxis: {
  						titleTextStyle: {
  							bold: true
  						},
  					},
  					legend: { position: "bottom", alignment: "start" },
  					height: "400",
  					seriesType: 'bars',
  					series: { 1: { type: 'line' } }
  				}
  			};
  		}
  		else {
  			this.comboChart.dataTable = [];
  			this._notificationService.handleResponse(response,"error");

  		}
  	}, error => {
  		this.comboChart.dataTable = [];
  		this.spinner.hide();
  		this._notificationService.handleResponse(error,"error");
  	});
  }

  /**
  * For Get upcoming maintenance list
  */
  getMaintenanceList() {
  	//this.spinner.show();
  	this.dashboardService.getUpcomingMaintenance().subscribe(response => {
  		//this.spinner.hide();
  		if (response.isSuccess === true) {
  			this.maintenanceList = response.data;
  		}
  		else {
  			this._notificationService.handleResponse(response,"error");

  		}
  	}, error => {
  		//this.spinner.hide();
  		this._notificationService.handleResponse(error,"error");
  	});
  }

  /**
  * For Fire event on tab change
  */
  onTabChange(event) {
  	this.getBuildingOverview(this.buildings[event.index]['value'],this.type);
  	this.getBuildingGraph(this.buildings[event.index]['value'],'',this.type);
  }

  /**
  * For change graph filter
  */
  changeGraphFilter(buiding, event) {
  	let type = 'd';
  	if (event.value === 'Week') {
  		type = 'w';
  	} else if (event.value === 'Month') {
  		type = 'm';
  	}
  	this.type = type;
  	this.getBuildingGraph(buiding['value'], '', type);
  	this.getBuildingOverview(buiding['value'], type);

  }
}
