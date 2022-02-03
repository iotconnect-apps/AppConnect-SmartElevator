import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { NgxSpinnerService } from 'ngx-spinner';
import { ScheduledMaintenanceService, NotificationService, Notification } from '../../../services';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageAlertDataModel, AppConstant } from '../../../app.constants';
import { MatDialog } from '@angular/material';
import { MessageDialogComponent } from '../..';
import * as moment from 'moment'


@Component({
  selector: 'app-scheduled-maintenance-add',
  templateUrl: './scheduled-maintenance-add.component.html',
  styleUrls: ['./scheduled-maintenance-add.component.css']
})
export class ScheduledMaintenanceAddComponent implements OnInit {

  moduleName = "Schedule Maintenance";
  maintenanceForm: FormGroup;
  maintenanceObject: any = {};
  buildingList = [];
  wingList = [];
  elevatorList = [];
  statusList = [{ "value": "Scheduled", 'disabled': false }, { "value": "Under Maintenance", 'disabled': false }, { "value": "Completed", 'disabled': false }];
  buttonname = "Submit";
  checkSubmitStatus = false;
  isEdit = false;
  companyId: any;
  messageAlertDataModel: MessageAlertDataModel;
  guid: any;
  maintenanceGuid: any;
  isCompleted = false;
  today: any;
  public endDateValidate: any;

  constructor(private formBuilder: FormBuilder,
    private spinner: NgxSpinnerService,
    private _service: ScheduledMaintenanceService,
    private _notificationService: NotificationService,
    private router: Router,
    public _appConstant: AppConstant,
    public dialog: MatDialog,
    private activatedRoute: ActivatedRoute) {
    this.activatedRoute.params.subscribe(params => {
      if (params.guid) {
        this.maintenanceGuid = params.guid;
        this.getScheduledMaintenanceDetails(params.guid);
        this.guid = params.buildingGuid;
        this.moduleName = "Edit Scheduled Maintenance";
        this.isEdit = true;
        this.buttonname = 'Update'
      } else {
        this.maintenanceObject = {}
      }
    });
    this.createFormGroup();
  }
  minDate: Date;
  maxDate: Date;
  ngOnInit() {
    const current = new Date().getFullYear();


    let currentUser = JSON.parse(localStorage.getItem('currentUser'));
    this.companyId = currentUser.userDetail.companyId;
    this.getBuildingList(this.companyId);
    this.today = new Date();
    let dd = this.today.getDate();
    let mm = this.today.getMonth() + 1; //January is 0!
    let yyyy = this.today.getFullYear();
    this.minDate = new Date(yyyy, mm - 1, dd);
    // this.maxDate = new Date(yyyy + 1, 11, 31);

    if (dd < 10) {
      dd = '0' + dd
    }
    if (mm < 10) {
      mm = '0' + mm
    }

    this.today = yyyy + '-' + mm + '-' + dd;
    this.endDateValidate = yyyy + '-' + mm + '-' + dd;
  }

  /**
   * Create form on page load
   * */
  createFormGroup() {
    this.maintenanceForm = this.formBuilder.group({
      entityGuid: new FormControl({ value: '', disabled: this.isEdit }, [Validators.required]),
      buildingGuid: new FormControl({ value: '', disabled: this.isEdit }, [Validators.required]),
      elevatorGuid: new FormControl({ value: '', disabled: this.isEdit }, [Validators.required]),
      startDateTime: ['', Validators.required],
      endDateTime: ['', Validators.required],
      description: ['']
    });
  }

  /**
   * for get current time zone
   * */
  getTimeZone() {
    return /\((.*)\)/.exec(new Date().toString())[1];
  }

  /**
   * Schedule Maintenance
   * */
  scheduleMaintenance() {
    this.checkSubmitStatus = true;

    if (this.isEdit) {
      this.maintenanceForm.registerControl('guid', new FormControl(this.maintenanceGuid))
    }
    if (this.maintenanceForm.status === "VALID") {

      this.spinner.show();
      let currentUser = JSON.parse(localStorage.getItem('currentUser'));
      let data = this.maintenanceForm.value;

      if (this.isEdit) {
        data.entityGuid = this.maintenanceForm.get('entityGuid').value;
        data.buildingGuid = this.maintenanceForm.get('buildingGuid').value;
        data.elevatorGuid = this.maintenanceForm.get('elevatorGuid').value;
        data.guid = this.maintenanceGuid;
      }

      data.startDateTime = moment(data.startDateTime).format('YYYY-MM-DDTHH:mm:ss');
      data.endDateTime = moment(data.endDateTime).format('YYYY-MM-DDTHH:mm:ss');
      data.timeZone = moment().utcOffset();

      this._service.scheduleMaintenance(data).subscribe(response => {
        if (response.isSuccess === true) {
          this.spinner.hide();
          if (this.isEdit) {
            this._notificationService.handleResponse({message:"Maintenance updated successfully."},"success");
          } else {
            this._notificationService.handleResponse({message:"Maintenance created successfully."},"success");
          }
          this.router.navigate(['/maintenance']);
        } else {
          this.spinner.hide();
          this._notificationService.handleResponse(response,"error");
        }
      });
    }
  }

  /**
   * Get building lookup by companyId
   * @param companyId
   */
  getBuildingList(companyId) {
    this.spinner.show();
    this._service.getBuildingLookup(companyId).subscribe(response => {
      this.spinner.hide();
      this.buildingList = response.data;
    });

  }

  /**
   * Get wing lookup by buildingId
   * @param buildingId
   */
  getWingList(buildingId) {
    this.spinner.show();
    this._service.getWingLookup(buildingId).subscribe(response => {
      this.spinner.hide();
      this.wingList = response.data;
    });
  }

  
  /**
   * validate end date using start date change
   * @param startdate
   */
  onChangeStartDate(startdate) {
    let date = moment(startdate).add(this._appConstant.minGap, 'm').format();
    this.endDateValidate = new Date(date);
  }

  /**
   * Get elevator lookup by wingId
   * @param wingId
   */
  getElevatorList(wingId) {
    this.spinner.show();
    this._service.getElevatorLookup(wingId).subscribe(response => {
      this.spinner.hide();
      this.elevatorList = response.data;
    });
  }

  /**
   * Get schedule maintenance details by guid
   * @param guid
   */
  //today's date
  getScheduledMaintenanceDetails(guid) {
    this.spinner.show();
    this._service.getScheduledMaintenanceDetails(guid).subscribe(response => {
      if (response.isSuccess === true) {
        this.maintenanceObject = response.data;
        // var now = moment(this.maintenanceObject['scheduledDate']).format('YYYY-MM-DD HH:mm:ss');
        // this.maintenanceObject['scheduledDate'] = moment.utc(now).local().format('YYYY-MM-DDTHH:mm:ss');
        this.maintenanceObject.startDateTime = moment(this.maintenanceObject.startDateTime + 'Z').local();
        this.maintenanceObject.endDateTime = moment(this.maintenanceObject.endDateTime + 'Z').local();
        // if (this.maintenanceObject['status'] === "Completed") {
        //   this.isCompleted = true;
        //   this.statusList[0].disabled=true;
        //   this.statusList[1].disabled=true;

        // }
        // if (this.maintenanceObject['status'] === "Under Maintenance") {
        //   this.statusList[0].disabled=true;
        //   console.log(this.statusList);

        // }
        this.getBuildingList(this.maintenanceObject['companyGuid']);
        this.getWingList(this.maintenanceObject['buildingGuid']);
        this.getElevatorList(this.maintenanceObject['entityGuid']);
        this.spinner.hide();
      }
    });
  }
}
