import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormGroup, Validators, FormBuilder } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { AppConstant, MessageAlertDataModel } from '../../../app.constants';
import { MessageDialogComponent } from '../../../components/common/message-dialog/message-dialog.component';
import { MatDialog } from '@angular/material';
import { Notification, NotificationService, UserService, ElevatorService } from '../../../services';
@Component({
  selector: 'app-elevator-add',
  templateUrl: './elevator-add.component.html',
  styleUrls: ['./elevator-add.component.css']
})
export class ElevatorAddComponent implements OnInit {
  @ViewChild('myFile', { static: false }) myFile: ElementRef;

  hasWing = true;
  addWingMsg: any = 'Add a wing first when you create the building';
  selectWing: string = "Select Wing";
  validstatus = false;
  MesageAlertDataModel: MessageAlertDataModel;
  parentEntityId = '';
  public mask = {
    guide: true,
    showMask: false,
    keepCharPositions: true,
    mask: ['(', /[0-9]/, /\d/, ')', '-', /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/]
  };
  fileUrl: any;
  fileName = '';
  fileToUpload: any = null;
  status;
  moduleName = "Add Elevator";
  userObject = {};
  userGuid = '';
  isEdit = false;
  elevatorForm: FormGroup;
  checkSubmitStatus = false;
  buildingList = [];
  wingList = [];
  selectedBuilding = '';
  floorList = [];
  spaceList = [];
  roleList = [];
  cityList = [];
  templateList = [];
  buttonname = 'SUBMIT'
  arrystatus = [{ "name": "Active", "value": true }, { "name": "Inactive", "value": false }]
  timezoneList = [];
  enitityList = [];
  elevatorGuid: any;
  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private _notificationService: NotificationService,
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    public userService: UserService,
    public elevatorService: ElevatorService,
    public dialog: MatDialog,
    public _appConstant: AppConstant,

  ) {
    this.activatedRoute.params.subscribe(params => {
      if (params.elevatorGuid != null) {
        this.getelevatorDetails(params.elevatorGuid);
        this.elevatorGuid = params.elevatorGuid;
        this.moduleName = "Edit Elevator";
        this.isEdit = true;
        this.buttonname = 'UPDATE'
      } else {
        this.userObject = { firstName: '', lastName: '', email: '', contactNo: '', password: '' }
      }
    });
    this.createFormGroup();
  }

  ngOnInit() {
    this.gettemplateLookup();
    this.getBuildingLookup(this.parentEntityId);
  }

	/**
	* For Create elevator FormGroup 
	**/
  createFormGroup() {

    this.elevatorForm = this.formBuilder.group({
      imageFile: [''],
      buildingGuid: ['', Validators.required],
      entityGuid: ['', Validators.required],
      name: ['', Validators.required],
      uniqueId: ['', [Validators.required, Validators.pattern('^[A-Za-z0-9]+$')]],
      specification: [''],
      description: ['']
    });
  }

	/**
	* For Add new elevator
	**/
  addElevator() {
    this.checkSubmitStatus = true;
    if (this.elevatorForm.status === "VALID") {
      if (this.validstatus == true || this.elevatorForm.value.imageFile == '') {
        if (this.fileToUpload) {
          this.elevatorForm.get('imageFile').setValue(this.fileToUpload);
        }
        this.spinner.show();
        let currentUser = JSON.parse(localStorage.getItem('currentUser'));
        this.elevatorService.addUpdateEveletor(this.elevatorForm.value).subscribe(response => {
          if (response.isSuccess === true) {
            this.spinner.hide();
            this._notificationService.handleResponse({ message: "Elevator created successfully." }, "success");
            this.router.navigate(['elevators']);
          } else {
            this.spinner.hide();
            this._notificationService.handleResponse(response, "error");
          }
        })
      } else {
        this.MesageAlertDataModel = {
          title: "Elevator Image",
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
	* For Get Elevator detail
	**/
  getelevatorDetails(elevetorGuid) {
    this.spinner.show();
    this.elevatorService.getelevatorDetails(elevetorGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
      }
    });
  }

  getdata(val) {
    return val = val.toLowerCase();
  }

	/**
	* For Get template lookup
	**/
  gettemplateLookup() {
    this.elevatorService.gettemplatelookup().
      subscribe(response => {
        if (response.isSuccess === true) {
          this.templateList = response['data'];
        } else {
          this._notificationService.handleResponse(response, "error");
        }
      }, error => {
        this.spinner.hide();
        this._notificationService.handleResponse(error, "error");
      })

  }

	/**
	* For Get building lookup
	**/
  getBuildingLookup(parentEntityId) {
    this.elevatorService.getBuildinglookup(parentEntityId).
      subscribe(response => {
        if (response.isSuccess === true) {
          this.buildingList = response.data.items;
          this.buildingList = this.buildingList.filter(word => word.isactive === true);
        } else {
          this._notificationService.handleResponse(response, "error");
        }
      }, error => {
        this.spinner.hide();
        this._notificationService.handleResponse(error, "error");
      })

  }

	/**
	* For Get wings of building
	**/
  getwings(parentEntityId) {
    this.spinner.show();
    this.elevatorService.getBuildinglookup(parentEntityId).
      subscribe(response => {
        if (response.isSuccess === true) {
          if (response.data.count > 0) {
            this.selectWing = "Select Wing";
            this.hasWing = true;
          }
          else {
            this.selectWing = "No Wing";
            this.hasWing = false;
          }
          this.wingList = response.data.items;
        } else {
          this.hasWing = false;
          this._notificationService.handleResponse(response, "error");
        }
        this.spinner.hide();
      }, error => {
        this.spinner.hide();
        this._notificationService.handleResponse(error, "error");
      })
  }

	/**
	* For preview image
	**/
  handleImageInput(event) {
    let files = event.target.files;
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
          }
        }
      } else {
        this.MesageAlertDataModel = {
          title: "Elevator Image",
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
   * Remove image
   * */
  imageRemove() {
    this.myFile.nativeElement.value = "";
    this.elevatorForm.get('imageFile').setValue('');
    this.fileUrl = null;
    this.fileToUpload = false;
    this.fileName = '';
  }

}
