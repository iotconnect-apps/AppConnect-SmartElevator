import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { DashboardService, NotificationService, Notification } from '../../../services';
import { ActivatedRoute } from '@angular/router';
import { AppConstant, DeleteAlertDataModel, MessageAlertDataModel } from '../../../app.constants';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { DeleteDialogComponent } from "../../../components/common/delete-dialog/delete-dialog.component";
import { MatDialog } from '@angular/material';
import { BuildingService } from '../../../services/building/building.service';
import { MessageDialogComponent } from '../..';
import * as moment from 'moment-timezone'
import { Location } from '@angular/common';
@Component({
  selector: 'app-building-details',
  templateUrl: './building-details.component.html',
  styleUrls: ['./building-details.component.css']
})
export class BuildingDetailsComponent implements OnInit {
  @ViewChild('myFile',{static:false}) myFile: ElementRef;
  validstatus = false;
  MesageAlertDataModel: MessageAlertDataModel;
  currentImage: any;
  elevatorList = [];

  buildingObj = {};
  buildingname:any;
  name: any;
  description: any;
  address1: any;
  address2: any;
  city: any;
  zipcode: any;
  status: any;
  image: any;
  buildingGuid: any;
  type: any;
  totalRecords = 0;
  deviceList = [];
  moduleName = "";
  wingModuleName = "";
  wingObject: any = {};
  handleImgInput = false;
  buttonname = 'SUBMIT';
  checkSubmitStatus = false;
  isEdit = false;
  deleteAlertDataModel: DeleteAlertDataModel;
  searchParameters = {
    parentEntityId: '',
    pageNumber: 0,
    pageSize: -1,
    searchText: '',
    sortBy: 'name asc'
  };
  energyUsage: any;
  humidity: any;
  moisture: any;
  temperature: any;
  totalDevices: any;
  waterUsage: any;
  fileName: any;
  fileToUpload: any;
  fileUrl: any;
  wingForm: FormGroup;
  wingGuid: any;
  wingList = [];
  mediaUrl: any;
  isOpenFilter: boolean = false;
  isOpenFilterGraph: boolean = false;

  lineChartData = {
    chartType: 'LineChart',
    options: {
      height: 400,
      interpolateNulls: true,
      hAxis: {
        title: 'hiii',
        gridlines: {
          count: 5
        },
      },
      vAxis: {
        title: 'Values',
        gridlines: {
          count: 5
        },
      }
    },
    legend: { position: "bottom", alignment: "start" },
    dataTable: [],

  }

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
  alerts = [];

  buidingDetailOverview = {
    elevatorsCounts: {
      connected: 0,
      total: 0
    },
    averageOperatingHours: 0,
    averageTrips: 0,
    energy: 0,
    totalMaintenanceCarriedOut: 0,
    alerts: 0
  }
  elevatorselected = [];
  public respondShow: boolean = false;



  constructor(
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    public service: BuildingService,
    public dialog: MatDialog,
    public _appConstant: AppConstant,
    public dashboardService: DashboardService,
    public buildingService: BuildingService,
    private _notificationService: NotificationService,
    public location: Location
  ) {
    this.createFormGroup();
    this.activatedRoute.params.subscribe(params => {
      if (params.buildingGuid) {
        this.buildingGuid = params.buildingGuid
      }

    })
  }

  ngOnInit() {
    let type = 'd';
    this.type = type
    this.getBuildingDetails(this.buildingGuid);
    this.getBuidingOverview(this.buildingGuid);
    this.getBuildingGraph(this.buildingGuid);
    this.getAlertList(this.buildingGuid);
    //this.getElevatorPeakHoursGraph(this.buildingGuid)
    this.getWingList(this.buildingGuid);
    this.getElevatorList(this.buildingGuid);
    this.mediaUrl = this._notificationService.apiBaseUrl;


  }

  /**
	* For manage wing side bar
	**/
  Respond() {
    this.fileName = '';
    this.fileUrl = null;
    this.wingForm.reset();
    this.fileToUpload = null;
    this.respondShow = true;
    this.isEdit = false;
    this.refresh();
  }

  /**
	* For manage wing side bar
	**/
  closerepond() {
    this.fileName = '';
    this.fileUrl = null;
    this.fileToUpload = null;
    this.checkSubmitStatus = false;
    this.respondShow = false;
    this.wingObject.image = '';
    this.wingForm.reset();
    this.currentImage = null;
  }

  /**
	* For Image remove from array
	**/
  imageRemove() {
    this.myFile.nativeElement.value = "";
    if (this.wingObject['image'] == this.currentImage) {
      this.wingForm.get('imageFile').setValue('');
      if (!this.handleImgInput) {
        this.handleImgInput = false;
        this.deleteImgModel();
      }
      else {
        this.handleImgInput = false;
      }
    }
    else {
      if (this.currentImage) {
        this.spinner.hide();
        this.wingObject['image'] = this.currentImage;
        this.fileToUpload = false;
        this.fileName = '';
        this.fileUrl = null;
      }
      else {
        this.spinner.hide();
        this.wingObject['image'] = null;
        this.wingForm.get('imageFile').setValue('');
        this.fileToUpload = false;
        this.fileName = '';
        this.fileUrl = null;
      }
    }
  }

  /**
	* For delete wing
	**/
  deletewingImg() {
    this.spinner.show();
    this.buildingService.removeBuildingImage(this.wingObject.guid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.currentImage = '';
        this.wingObject['image'] = null;
        this.wingForm.get('imageFile').setValue(null);
        this.fileToUpload = false;
        this.currentImage = '';
        this.fileName = '';
        this.getWingList(this.buildingGuid);
        this._notificationService.handleResponse({message:this._appConstant.msgDeleted.replace("modulename", "Wing Image")},"success");

      } else {
        this._notificationService.handleResponse(response,"error");
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.handleResponse(error,"error");
    });
  }


  /**
	* For open delete wing confirmation model
	**/
  deleteImgModel() {
    this.deleteAlertDataModel = {
      title: "Delete Image",
      message: this._appConstant.msgConfirm.replace('modulename', "Wing Image"),
      okButtonName: "Confirm",
      cancelButtonName: "Cancel",
    };
    const dialogRef = this.dialog.open(DeleteDialogComponent, {
      width: '400px',
      height: 'auto',
      data: this.deleteAlertDataModel,
      disableClose: false
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deletewingImg();
      }
    });
  }

  /**
	* For fire on elevator change
	**/
  changeElevator() {
    this.getElevatorPeakHoursGraph(this.buildingGuid, this.elevatorselected,this.type)
  }

  /**
	* For on change peak hours Graph in Filter
	**/
  changePeakHoursGraphFilter(event) {
    let type = 'd';
    if (event.value === 'Week') {
      type = 'w';
    } else if (event.value === 'Month') {
      type = 'm';
    }
     this.type = type
    this.getElevatorPeakHoursGraph(this.buildingGuid, this.elevatorselected, type)
  }

  /**
	* For Get Buiding Overview
	**/
  getBuidingOverview(buildingGuid) {
    this.spinner.show();
    this.service.getBuidingOverview(buildingGuid).subscribe(response => {
      this.spinner.hide();

      if (response.isSuccess === true) {
        this.buidingDetailOverview = response.data;
        this.buidingDetailOverview = {
          elevatorsCounts: {
            connected: (response.data['totalConnectedElevator']) ? response.data['totalConnectedElevator'] : 0,
            total: (response.data['totalElevator']) ? response.data['totalElevator'] : 0
          },
          averageOperatingHours: (response.data['totalOperatingHours']) ? response.data['totalOperatingHours'] : 0,
          averageTrips: (response.data['totalTrips']) ? response.data['totalTrips'] : 0,
          energy: (response.data['totalEnergy']) ? response.data['totalEnergy'] : 0,
          totalMaintenanceCarriedOut: (response.data['totalUnderMaintenanceCount']) ? response.data['totalUnderMaintenanceCount'] : 0,
          alerts: (response.data['totalAlerts']) ? response.data['totalAlerts'] : 0
        }
      }
      else {
        this.buidingDetailOverview = {
          elevatorsCounts: {
            connected: 0,
            total: 0
          },
          averageOperatingHours: 0,
          averageTrips: 0,
          energy: 0,
          totalMaintenanceCarriedOut: 0,
          alerts: 0
        }
        this._notificationService.handleResponse(response,"error");

      }
    }, error => {
      this.spinner.hide();
      this.buidingDetailOverview = {
        elevatorsCounts: {
          connected: 0,
          total: 0
        },
        averageOperatingHours: 0,
        averageTrips: 0,
        energy: 0,
        totalMaintenanceCarriedOut: 0,
        alerts: 0
      }
      this._notificationService.handleResponse(error,"error");
    });
  }

  /**
	* For Get Alert List
	**/
  getAlertList(buildingGuid) {
    let parameters = {
      pageNumber: 0,
      pageSize: 15,
      searchText: '',
      sortBy: 'eventDate desc',
      deviceGuid: '',
      entityGuid: buildingGuid,
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
        this.spinner.hide();
      this.alerts = [];
    });
  }

  /**
	* For Get Elevator List
	**/
  getElevatorList(buildingId) {
    this.spinner.show();
    this.service.getElevatorLookup(buildingId).subscribe(response => {
      this.spinner.hide();
      this.elevatorList = response.data;
      if(this.elevatorList.length > 0){
        this.elevatorselected.push(this.elevatorList[0].value);
        this.getElevatorPeakHoursGraph(this.buildingGuid, this.elevatorselected,this.type)
      }
    });
  }

  /**
	* For Get Building Graph
	**/
  getBuildingGraph(buildingId, elevatorId = '', type = 'd') {
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
        this._notificationService.handleResponse(response,"error");

      }
    }, error => {
      this.spinner.hide();
      this._notificationService.handleResponse(error,"error");
    });
  }

  /**
	* For Get Elevator PeakHours Graph
	**/
  getElevatorPeakHoursGraph(companyGuid, elevatorId = [], type) {
    this.spinner.show();
    let currentUser = JSON.parse(localStorage.getItem('currentUser'));

    if (elevatorId.length) {
      this.dashboardService.getdevicespeakhours(elevatorId, type).subscribe(response => {
        this.spinner.hide();
        this.lineChartData.dataTable = [];
        if (response.isSuccess === true) {
          let data = [];
          if (response.data.length) {
            data.push(["time"]);
            let i = 0;
            response.data.forEach(element => {
              let dataArr = [];
              dataArr.push(element.time);
              element.value.forEach(childElement => {
                if (i == 0) {
                  data[0].push(childElement.name);
                }
                dataArr.push(parseFloat(childElement.value));
              });
              data.push(dataArr);
              i++;
            });
          }
          this.lineChartData = {
            chartType: 'LineChart',
            options: {
              height: 400,
              interpolateNulls: true,
              hAxis: {
                title: '',
                gridlines: {
                  count: 5
                },
              },
              vAxis: {
                title: 'Values',
                gridlines: {
                  count: 5
                },
              }
            },
            legend: { position: "bottom", alignment: "start" },
            dataTable: data,
          };
        }
        else {
          this._notificationService.handleResponse(response,"error");

        }
      }, error => {
        this.spinner.hide();
        this._notificationService.handleResponse(error,"error");
      });
    } else {
      this.lineChartData.dataTable = [];
      this.spinner.hide();
    }
  }

  /**
	* For Create Form Group for wing
	**/
  createFormGroup() {
    this.wingForm = new FormGroup({
      parentEntityGuid: new FormControl(null),
      name: new FormControl('', [Validators.required]),
      description: new FormControl(''),
      isactive: new FormControl(''),
      imageFile: new FormControl(''),
    });
  }

  /**
	* For Building Details
	**/
  getBuildingDetails(buildingGuid) {
    this.spinner.show();
    this.service.getbuildingDetails(buildingGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.buildingObj = response.data;
        this.buildingname = response.data.name;
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
	* For Get building
	**/
  getbuilding(buildingGuid) {

    this.spinner.show();
    this.service.getbuildingDetails(buildingGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {

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
	* For Handle Image File Preview
	**/
  handleImageInput(event) {
    this.handleImgInput = true;
    let files = event.target.files;
    var that=this;
    if (files.length) {
      let fileType = files.item(0).name.split('.');
      let imagesTypes = ['jpeg', 'JPEG', 'jpg', 'JPG', 'png', 'PNG'];
      if (imagesTypes.indexOf(fileType[fileType.length - 1]) !== -1) {
        this.validstatus = true;
        this.fileName = files.item(0).name;
        this.fileToUpload = files.item(0);
        if (event.target.files && event.target.files[0]) {
          var reader = new FileReader();
          reader.readAsDataURL(event.target.files[0]);
          reader.onload = (innerEvent: any) => {
            this.fileUrl = innerEvent.target.result;
            that.wingObject.image= this.fileUrl  
          }
        }
      } else {
        this.imageRemove();
        this.MesageAlertDataModel = {
          title: "Wing Image",
          message: "Invalid Image Type.",
          message2: "Upload .jpg, .jpeg, .png Image Only.",
          okButtonName: "OK",
        };
        const dialogRef = this.dialog.open(MessageDialogComponent, {
          width: '400px',
          height: 'auto',
          data: this.MesageAlertDataModel,
          disableClose: false
        });
      }
    }
  }

  /**
	* For Manage Wing (Add / Edit)
	**/
  manageWing() {
    this.checkSubmitStatus = true;
    var data = {
      "parentEntityGuid": this.buildingGuid,
      "name": this.wingForm.value.name,
      "description": this.wingForm.value.description,
      "isactive": true,
      "countryGuid": this.buildingObj['countryGuid'],
      "stateGuid": this.buildingObj['stateGuid'],
      "city": this.buildingObj['city'],
      "zipcode": this.buildingObj['zipcode'],
      "address": this.buildingObj['address'],
      "latitude": this.buildingObj['latitude'],
      "longitude": this.buildingObj['longitude'],
    }
    if (this.isEdit) {
      if (this.wingGuid) {
        data["guid"] = this.wingGuid;
      }
      if (this.fileToUpload) {
        data["imageFile"] = this.fileToUpload;
      }
      data.isactive = this.wingObject['isactive']
    }
    else {
      data["imageFile"] = this.fileToUpload;
      this.wingForm.get('isactive').setValue(true);

    }
    if (this.wingForm.status === "VALID") {
      if (this.validstatus == true || !this.wingForm.value.imageFile) {
        this.spinner.show();
        this.service.addBuilding(data).subscribe(response => {
          this.spinner.hide();
          this.respondShow = false;
          this.getWingList(this.buildingGuid);

          if (response.isSuccess === true) {
            if (this.isEdit) {
              this._notificationService.handleResponse({message:"Wing updated successfully."},"success");
              this.closerepond();
            } else {
              this._notificationService.handleResponse({message:"Wing created successfully."},"success");
              this.closerepond();
            }
          } else {
            this._notificationService.handleResponse(response,"error");
          }
          this.checkSubmitStatus = false;
        })
      } else {
        this.MesageAlertDataModel = {
          title: "Wing Image",
          message: "Invalid Image Type.",
          message2: "Upload .jpg, .jpeg, .png Image Only.",
          okButtonName: "OK",
        };
        const dialogRef = this.dialog.open(MessageDialogComponent, {
          width: '400px',
          height: 'auto',
          data: this.MesageAlertDataModel,
          disableClose: false
        });
      }
    }
  }

  /**
	* For Get Wing List
	**/
  getWingList(buildingGuid) {
    this.spinner.show();
    this.searchParameters['parentEntityId'] = buildingGuid;
    this.service.getBuilding(this.searchParameters).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.wingList = response.data.items;
      }
      else {
        this._notificationService.handleResponse(response,"error");
        this.wingList = [];
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.handleResponse(error,"error");
    });
  }

  /**
	* For Reste Wing Form
	**/
  refresh() {
    this.createFormGroup();
    this.wingForm.reset(this.wingForm.value);
    this.wingModuleName = "Add Wing";
    this.wingGuid = null;
    this.buttonname = 'ADD';
    this.isEdit = false;
    this.currentImage = null;
  }

  /**
	* For open delete Wing confirmation model
	**/
  deleteModel(wingModel: any) {
    this.deleteAlertDataModel = {
      title: "Delete Wing",
      message: this._appConstant.msgConfirm.replace('modulename', "Wing"),
      okButtonName: "Confirm",
      cancelButtonName: "Cancel",
    };
    const dialogRef = this.dialog.open(DeleteDialogComponent, {
      width: '400px',
      height: 'auto',
      data: this.deleteAlertDataModel,
      disableClose: false
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteWing(wingModel.guid);
      }
    });
  }

  /**
	* For delete Wing 
	**/
  deleteWing(guid) {
    this.spinner.show();
    this.service.deleteBuilding(guid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this._notificationService.handleResponse({message:this._appConstant.msgDeleted.replace("modulename", "Wing")},"success");
        this.getWingList(this.buildingGuid);
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
	* For get Wing  details 
	**/
  getWingDetails(wingGuid) {
    this.closerepond();
    this.fileToUpload =false;
    this.wingObject.image='';
    this.wingModuleName = "Edit Wing";
    this.wingGuid = wingGuid;
    this.isEdit = true;
    this.buttonname = 'UPDATE';
    this.respondShow = true;
    this.spinner.show();
    this.service.getbuildingDetails(wingGuid).subscribe(response => {
      this.spinner.hide();
      this.buildingObj = response.data;
      if (response.isSuccess === true) {
        this.wingObject = response.data;
        if(this.wingObject.image){
          this.wingObject.image = this.mediaUrl + this.wingObject.image;
          this.currentImage = this.wingObject.image;
        }
      }
      else {
        this._notificationService.handleResponse(response,"error");
        this.wingList = [];
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.handleResponse(error,"error");
    });
  }

  /**
	* For convert UTC date to local time zone
	**/
  getLocalDate(lDate) {
    var utcDate = moment.utc(lDate, 'YYYY-MM-DDTHH:mm:ss.SSS');
    // Get the local version of that date
    var localDate = moment(utcDate).local();
    let res = moment(localDate).format('MMM DD, YYYY hh:mm:ss A');
    return res;
    
  }

  /**
	* For Change Graph Filter
	**/
  changeGraphFilter(event) {
    let type = 'd';
    if (event.value === 'Week') {
      type = 'w';
    } else if (event.value === 'Month') {
      type = 'm';
    }
    this.getBuildingGraph(this.buildingGuid, '', type);
  }
}
