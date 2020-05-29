import { Component, OnInit } from '@angular/core';
import { Notification, NotificationService, ElevatorService, DashboardService } from 'app/services';
import { ActivatedRoute, Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import 'chartjs-plugin-streaming';
import { StompRService } from '@stomp/ng2-stompjs'
import { Message } from '@stomp/stompjs'
import { Subscription } from 'rxjs'
import { Observable, forkJoin } from 'rxjs';
import { Location } from '@angular/common';
import * as moment from 'moment-timezone'
import * as _ from 'lodash'
@Component({
  selector: 'app-elevator-details',
  templateUrl: './elevator-details.component.html',
  styleUrls: ['./elevator-details.component.css'],
  providers: [StompRService]
})
export class ElevatorDetailsComponent implements OnInit {

  isConnected = false;
  cpId = '';
  subscribed;
  stompConfiguration = {
    url: '',
    headers: {
      login: '',
      passcode: '',
      host: ''
    },
    heartbeat_in: 0,
    heartbeat_out: 2000,
    reconnect_delay: 5000,
    debug: true
  }
  chartColors: any = {
    red: 'rgb(255, 99, 132)',
    orange: 'rgb(255, 159, 64)',
    yellow: 'rgb(255, 205, 86)',
    green: 'rgb(75, 192, 192)',
    blue: 'rgb(54, 162, 235)',
    purple: 'rgb(153, 102, 255)',
    grey: 'rgb(201, 203, 207)',
    cerise: 'rgb(255,0,255)',
    popati: 'rgb(0,255,0)',
    dark: 'rgb(5, 86, 98)',
    solid: 'rgb(98, 86, 98)'
  };
  datasets: any[] = [
    {
      label: 'Dataset 1 (linear interpolation)',
      backgroundColor: 'rgb(153, 102, 255)',
      borderColor: 'rgb(153, 102, 255)',
      fill: false,
      lineTension: 0,
      borderDash: [8, 4],
      data: []
    }
  ];

  options: any = {
    type: 'line',
    scales: {

      xAxes: [{
        type: 'realtime',
        time: {
          stepSize: 10
        },
        realtime: {
          duration: 90000,
          refresh: 7000,
          delay: 2000,
          //onRefresh: '',

          // delay: 2000

        }

      }],
      yAxes: [{
        scaleLabel: {
          display: true,
          labelString: 'value'
        }
      }]

    },
    tooltips: {
      mode: 'nearest',
      intersect: false
    },
    hover: {
      mode: 'nearest',
      intersect: false
    }

  };

  columnChart2 = {
    chartType: "ColumnChart",
    dataTable: [],
    options: {
      title: "",
      vAxis: {
        title: "",
        titleTextStyle: {
          bold: true
        },
        viewWindow: {
          min: 0
        }
      },
      hAxis: {
        title: "",
        titleTextStyle: {
          bold: true
        },
      },
      legend: { position: "bottom", alignment: "start" },
      height: "400",
      series: [
        { color: "#f4b400", visibleInLegend: true },
        { color: "#05b76b", visibleInLegend: true },
        { color: "#c0c0c0", visibleInLegend: true }
      ]
    }
  };
  dataobj: any = {}
  mediaUrl: string;
  nomediaUrl: any;
  elevatorGuid: any;
  sensdata: any = [];
  subscription: Subscription;
  messages: Observable<Message>;
  datadetail = {
    operatingHours: 0,
    tripsPerDay: 0,
    energyConsumption: 0,
    averageTemprature: 0,
    averageSpeed: 0,
    averageVibrationLevel: 0,
    maintenanceSchescheduled: 0
  }
  labelname: any;
  maintenanceList = [];
  alerts = [];

  constructor(
    private _notificationService: NotificationService,
    private elevatorService: ElevatorService,
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    private stompService: StompRService,
    public dashboardService: DashboardService,
    public location: Location
  ) {
    this.dataobj = { name: '', uniqueId: '' }
    this.activatedRoute.params.subscribe(params => {
      if (params.elevatorGuid != null) {
        this.elevatorGuid = params.elevatorGuid;
        this.getelevatorDetails(params.elevatorGuid);
        this.getelevatorCountDetails(params.elevatorGuid);
        this.getTripGraph(params.elevatorGuid);
        this.getMaintenanceList(params.elevatorGuid);
        this.getAlertList(params.elevatorGuid);
        this.elevatorGuid = params.elevatorGuid
      }
    });

  }

  ngOnInit() {
    //this.onTabChange()
  }

  getLocalDate(lDate) {
    var utcDate = moment.utc(lDate, 'YYYY-MM-DDTHH:mm:ss.SSS');
    // Get the local version of that date
    var localDate = moment(utcDate).local();
    let res = moment(localDate).format('MMM DD, YYYY hh:mm:ss A');
    return res;

  }

  getMaintenanceList(elevatorGuid) {
    //this.spinner.show();
    this.dashboardService.getUpcomingMaintenance('', elevatorGuid).subscribe(response => {
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
  getAlertList(elevatorGuid) {
    let parameters = {
      pageNumber: 0,
      pageSize: 10,
      searchText: '',
      sortBy: 'eventDate desc',
      deviceGuid: elevatorGuid,
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

  getelevatorDetails(elevetorGuid) {
    this.spinner.show();
    this.elevatorService.getelevatorDetails(elevetorGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.mediaUrl = this._notificationService.apiBaseUrl + response.data.image
        this.nomediaUrl = response.data.image
        this.dataobj = response.data
        this.getgenraterTelemetryData(response.data.templateGuid);
        //this.userObject = response.data;
        //this.fileUrl = this.deviceObject['image'];
      }
    });
  }
  getelevatorCountDetails(elevetorGuid) {
    this.spinner.show();
    this.elevatorService.getelevatorcountDetails(elevetorGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.datadetail = response.data;
        let msVal = (response.data['day']) ? response.data['day'] : 0;
        msVal += ' d ';
        msVal += (response.data['hour']) ? response.data['hour'] : 0;
        msVal += ' hrs ';
        msVal += (response.data['minute']) ? response.data['minute'] : 0;
        msVal += ' m';
        this.datadetail = {
          operatingHours: (response.data['operatingHourCount']) ? response.data['operatingHourCount'] : 0,
          tripsPerDay: (response.data['averageTrip']) ? response.data['averageTrip'] : 0,
          energyConsumption: (response.data['energyCount']) ? response.data['energyCount'] : 0,
          averageTemprature: (response.data['averageTemperature']) ? response.data['averageTemperature'] : 0,
          averageSpeed: (response.data['averageSpeed']) ? response.data['averageSpeed'] : 0,
          averageVibrationLevel: (response.data['averageVibration']) ? response.data['averageVibration'] : 0,
          maintenanceSchescheduled: msVal
        }
      }
    });

  }

  getTripGraph(elevatorGuid) {
    this.elevatorService.getTripGraph(elevatorGuid).subscribe(response => {
      this.spinner.hide();
      let data = [];
      if (response.isSuccess === true) {

        if (response.data.length) {
          data.push(["Time", ""])
          response.data.forEach(element => {
            data.push([element.time, parseFloat(element.value)])
          });
        }
        this.columnChart2 = {
          chartType: "ColumnChart",
          dataTable: data,
          options: {
            title: "",
            vAxis: {
              title: "",
              titleTextStyle: {
                bold: true
              },
              viewWindow: {
                min: 0
              }
            },
            hAxis: {
              title: "",
              titleTextStyle: {
                bold: true
              },
            },
            legend: { position: "bottom", alignment: "start" },
            height: "400",
            series: [
              { color: "#f4b400", visibleInLegend: true },
              { color: "#05b76b", visibleInLegend: true },
              { color: "#c0c0c0", visibleInLegend: true }
            ]
          }
        };

      }
    });

  }


  // For get TelemetryData
  getgenraterTelemetryData(templateGuid) {
    this.spinner.show();
    this.elevatorService.getelevatorTelemetryData(templateGuid).subscribe(response => {
      if (response.isSuccess === true) {
        this.spinner.hide();
        this.sensdata = response.data
        //this.onTabChange(response.data[0].text)
        let temp = [];
        response.data.forEach((element, i) => {
          var colorNames = Object.keys(this.chartColors);
          var colorName = colorNames[i % colorNames.length];
          var newColor = this.chartColors[colorName];
          var graphLabel = {
            label: element.text,
            backgroundColor: 'rgb(153, 102, 255)',
            borderColor: newColor,
            fill: false,
            cubicInterpolationMode: 'monotone',
            data: []
          }
          temp.push(graphLabel);
        });
        this.datasets = temp;
        this.getStompConfig();
        /*	let temp = [];
          response.data.forEach((element, i) => {
            var colorNames = Object.keys(this.chartColors);
            var colorName = colorNames[i % colorNames.length];
            var newColor = this.chartColors[colorName];
            var graphLabel = {
              label: element.text,
              backgroundColor: 'rgb(153, 102, 255)',
              borderColor: newColor,
              fill: false,
              cubicInterpolationMode: 'monotone',
              data: []
            }
            temp.push(graphLabel);
          });
          // response.data.forEach(element, i) => {
   
          // });
          this.datasets = temp;
          this.getStompConfig();*/
      } else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }
  onTabChange(tab) {
    if (tab != undefined && tab != '') {
      this.labelname = tab.tab.textLabel;
      let datalabel = this.labelname
      this.options = {
        type: 'line',
        scales: {

          xAxes: [{
            type: 'realtime',
            time: {
              stepSize: 10
            },
            realtime: {
              duration: 90000,
              refresh: 1000,
              delay: 2000,
              onRefresh: function (chart: any) {
                //if (obj.data.msgType !== 'device') {
                chart.data.datasets.forEach(function (dataset: any) {
                  if (dataset.label == datalabel) {
                    dataset.hidden = false
                  } else {
                    dataset.hidden = true
                  }
                });
                // }




              },

              // delay: 2000

            }

          }],
          yAxes: [{
            scaleLabel: {
              display: true,
              //labelString: 'value'
            }
          }]

        },
        tooltips: {
          mode: 'nearest',
          intersect: false
        },
        hover: {
          mode: 'nearest',
          intersect: false
        }

      }
      //this.getStompConfig();
      //console.log("tabsss",tab.tab.textLabel)
      /*let temp = [];
      var colorNames = Object.keys(this.chartColors);
      var colorName = colorNames[tab.index % colorNames.length];
      var newColor = this.chartColors[colorName];
      var graphLabel = {
        label: tab.tab.textLabel,
        backgroundColor: 'rgb(153, 102, 255)',
        borderColor: newColor,
        fill: false,
        cubicInterpolationMode: 'monotone',
        data: []
      }
      temp.push(graphLabel);
      this.datasets = temp;*/
    }


  }

  getStompConfig() {

    this.elevatorService.getStompConfig('LiveData').subscribe(response => {
      if (response.isSuccess) {
        this.stompConfiguration.url = response.data.url;
        this.stompConfiguration.headers.login = response.data.user;
        this.stompConfiguration.headers.passcode = response.data.password;
        this.stompConfiguration.headers.host = response.data.vhost;
        this.cpId = response.data.cpId;
        this.initStomp();
      }
    });
  }
  initStomp() {
    let config = this.stompConfiguration;
    this.stompService.config = config;
    this.stompService.initAndConnect();
    this.stompSubscribe();
  }
  public stompSubscribe() {
    if (this.subscribed) {
      return;
    }

    this.messages = this.stompService.subscribe('/topic/' + this.cpId + '-' + this.dataobj.uniqueId);
    this.subscription = this.messages.subscribe(this.on_next);
    this.subscribed = true;
  }
  public on_next = (message: Message) => {
    let obj: any = JSON.parse(message.body);
    let reporting_data = obj.data.data.reporting
    this.isConnected = true;
    let dates = obj.data.data.time;
    //var datalabel = this.labelname;
    let now = moment();
    if (obj.data.data.status == undefined && obj.data.msgType == 'telemetry' && obj.data.msgType != 'device' && obj.data.msgType != 'simulator') {
      this.options = {
        type: 'line',
        scales: {

          xAxes: [{
            type: 'realtime',
            time: {
              stepSize: 10
            },
            realtime: {
              duration: 90000,
              refresh: 7000,
              delay: 2000,
              onRefresh: function (chart: any) {
                if (chart.height) {
                  if (obj.data.msgType !== 'device') {
                    chart.data.datasets.forEach(function (dataset: any) {
                      /*if (dataset.label === datalabel) {
                        dataset.hidden = false
                      } else{
                        dataset.hidden = true
                      }*/
                      dataset.data.push({

                        x: now,

                        y: reporting_data[dataset.label]

                      });
                    });
                  }
                } else {

                }



              },

              // delay: 2000

            }

          }],
          yAxes: [{
            scaleLabel: {
              display: true,
              labelString: 'value'
            }
          }]

        },
        tooltips: {
          mode: 'nearest',
          intersect: false
        },
        hover: {
          mode: 'nearest',
          intersect: false
        }

      }
    }

    obj.data.data.time = now;
    /*var colorNames = Object.keys(this.chartColors);
    var colorName = colorNames[this.datasets.length % colorNames.length];
    var newColor = this.chartColors[colorName];
    var test = {
      label: 'Dataset 3 (cubic interpolation)',
      backgroundColor: 'rgb(153, 102, 255)',
      borderColor: newColor,
      fill: false,
      cubicInterpolationMode: 'monotone',
      data: []
    }*/


  }
}
