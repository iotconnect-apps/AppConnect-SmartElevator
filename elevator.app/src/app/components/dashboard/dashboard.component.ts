import * as moment from 'moment-timezone'
import { Component, OnInit } from '@angular/core'
import { NgxSpinnerService } from 'ngx-spinner'
import { DashboardService, UsersService, NotificationService, Notification } from '../../services';

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
    }
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
  constructor(
    private spinner: NgxSpinnerService,
    public dashboardService: DashboardService,
    public usersService: UsersService,
    private _notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.type = "d";
    let currentUser = JSON.parse(localStorage.getItem('currentUser'));
    this.companyId = currentUser.userDetail.companyId;
    this.getDashbourdCount();
    this.getBuildingList();
    this.getMaintenanceList();
    this.getAlertList();

  }

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
        this._notificationService.add(new Notification('error', response.message));
        
      }
    }, error => {
      this.alerts = [];
      this._notificationService.add(new Notification('error', error));
    });
  }

  getLocalDate(lDate) {
    var utcDate = moment.utc(lDate, 'YYYY-MM-DDTHH:mm:ss.SSS');
    // Get the local version of that date
    var localDate = moment(utcDate).local();
    let res = moment(localDate).format('MMM DD, YYYY hh:mm:ss A');
    return res;
    
    }



  getDashbourdCount() {
    this.spinner.show();
    this.dashboardService.getDashboardoverview().subscribe(response => {
      this.spinner.hide();
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
          }
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
          }
        };
        this.pieChartObj.dataTable = [];
        this._notificationService.add(new Notification('error', response.message));

      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

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
        this._notificationService.add(new Notification('error', response.message));

      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

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
        this._notificationService.add(new Notification('error', response.message));

      }
    }, error => {
      this.buildingsDetail['totalElevator'] = 0;
      this.buildingsDetail['totalOperatingHours'] = 0;
      this.buildingsDetail['totalTrips'] = 0;
      this.buildingsDetail['totalEnergy'] = 0;
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

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
        this._notificationService.add(new Notification('error', response.message));

      }
    }, error => {
      this.comboChart.dataTable = [];
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }
  getMaintenanceList() {
    //this.spinner.show();
    this.dashboardService.getUpcomingMaintenance().subscribe(response => {
      //this.spinner.hide();
      if (response.isSuccess === true) {
        this.maintenanceList = response.data;
      }
      else {
        this._notificationService.add(new Notification('error', response.message));

      }
    }, error => {
      //this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }
 
  onTabChange(event) {
    this.getBuildingOverview(this.buildings[event.index]['value'],this.type);
    this.getBuildingGraph(this.buildings[event.index]['value'],'',this.type);
  }
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
