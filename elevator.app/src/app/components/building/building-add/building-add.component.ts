import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { BuildingService } from '../../../services/building/building.service';
import { Notification, NotificationService} from '../../../services';
import { MessageDialogComponent } from '../../../components/common/message-dialog/message-dialog.component';
import { MatDialog } from '@angular/material';
import { AppConstant,MessageAlertDataModel, DeleteAlertDataModel } from '../../../app.constants';
import { DeleteDialogComponent } from '../..';
export interface DeviceTypeList {
  id: number;
  type: string;
}

@Component({
  selector: 'app-building-add',
  templateUrl: './building-add.component.html',
  styleUrls: ['./building-add.component.css']
})
export class BuildingAddComponent implements OnInit {
  @ViewChild('myFile',{static:false}) myFile: ElementRef;
  currentImage: any;
  validstatus = false;
  hasImage = false;
  MesageAlertDataModel: MessageAlertDataModel;
  deleteAlertDataModel: DeleteAlertDataModel;
  fileUrl: any;
  fileName = '';
  fileToUpload: any;
  status;
  moduleName = "Add Building";
  buildingObject:any = {};
  buildingGuid = '';
  isEdit = false;
  buildingForm: FormGroup;
  checkSubmitStatus = false;
  countryList = [];
  stateList = [];
  mediaUrl: any;
  buttonname = 'SUBMIT'
  arrystatus = [{ "name": "Active", "value": true }, { "name": "Inactive", "value": false }]
  constructor(
    private router: Router,
    private _notificationService: NotificationService,
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    public buildingService: BuildingService,
    public _appConstant: AppConstant,
    public dialog: MatDialog,
  ) {
    this.createFormGroup();
    this.activatedRoute.params.subscribe(params => {
      if (params.buildingGuid != 'add') {
        this.getBuildingDetails(params.buildingGuid);
        this.buildingGuid = params.buildingGuid;
        this.moduleName = "Edit Building";
        this.isEdit = true;
        this.buttonname = 'UPDATE'
      } else {
        this.buildingObject = { name: '', zipcode: '', countryGuid: '', stateGuid: '', isactive: 'true', city: '', latitude: '', longitude: '' }
      }
    });
  }

  ngOnInit() {
    this.mediaUrl = this._notificationService.apiBaseUrl;
    this.getcountryList();

  }

  /**
	* For create form group for building
	**/
  createFormGroup() {
    this.buildingForm = new FormGroup({
      parentEntityGuid: new FormControl(''),
      countryGuid: new FormControl(null, [Validators.required]),
      stateGuid: new FormControl(null, [Validators.required]),
      city: new FormControl('', ),
      name: new FormControl('',),
      zipcode: new FormControl('', [Validators.required, Validators.pattern('^[A-Z0-9 _]*$')]),
      description: new FormControl(''),
      address: new FormControl('', [Validators.required]),
      isactive: new FormControl('', [Validators.required]),
      guid: new FormControl(null),
      latitude: new FormControl('', [Validators.required, Validators.pattern('^(\\+|-)?(?:90(?:(?:\\.0{1,6})?)|(?:[0-9]|[1-8][0-9])(?:(?:\\.[0-9]{1,6})?))$')]),
      longitude: new FormControl('', [Validators.required, Validators.pattern('^(\\+|-)?(?:180(?:(?:\\.0{1,6})?)|(?:[0-9]|[1-9][0-9]|1[0-7][0-9])(?:(?:\\.[0-9]{1,6})?))$')]),
      imageFile: new FormControl(''),
    });
  }

  /**
	* For remove image from array
	**/
  imageRemove() {
    this.myFile.nativeElement.value = "";
    if (this.buildingObject['image'] == this.currentImage) {
      this.deleteImgModel();
    }
    else {
      if (this.currentImage) {
        this.spinner.hide();
        this.buildingObject['image'] = this.currentImage;
        this.fileToUpload = false;
        this.fileName = '';
      }
      else {
        this.spinner.hide();
        this.buildingObject['image'] = null;
        this.buildingForm.get('imageFile').setValue('');
        this.fileToUpload = false;
        this.fileName = '';
      }
    }
  }

  /**
	* For open delete confirmation model
	**/
  deleteImgModel() {
    this.deleteAlertDataModel = {
      title: "Delete Image",
      message: this._appConstant.msgConfirm.replace('modulename', "Building Image"),
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
        this.deletebuildingImg();
      }
    });
  }

  /**
	* For delete building
	**/
  deletebuildingImg() {
    this.spinner.show();
    this.buildingService.removeBuildingImage(this.buildingGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.buildingObject['image'] = null;
        this.buildingForm.get('imageFile').setValue('');
        this._notificationService.handleResponse({message:this._appConstant.msgDeleted.replace("modulename", "Building Image")},"success");
      } else {
        this._notificationService.handleResponse(response,"error");
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.handleResponse(error,"error");
    });
  }

  /**
	* For add new building
	**/
  addBuilding() {
    this.checkSubmitStatus = true;

    if (this.isEdit) {
      this.buildingForm.get('guid').setValue(this.buildingGuid);
      this.buildingForm.get('isactive').setValue(this.buildingObject['isactive']);
    } else {
      this.buildingForm.get('isactive').setValue(true);
    }
    if (this.buildingForm.status === "VALID") {
      if (this.validstatus == true || !this.buildingForm.value.imageFile) {
      if (this.fileToUpload) {
        this.buildingForm.get('imageFile').setValue(this.fileToUpload);
      }
      this.spinner.show();
      let currentUser = JSON.parse(localStorage.getItem('currentUser'));
      this.buildingForm.get('parentEntityGuid').setValue(currentUser.userDetail.entityGuid);
      
      this.buildingService.addBuilding(this.buildingForm.value).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          if (this.isEdit) {
            this._notificationService.handleResponse({message:"Building updated successfully."},"success");
          } else {
            this._notificationService.handleResponse({message:"Building created successfully."},"success");
          }
          this.router.navigate(['/buildings']);
        } else {
          this._notificationService.handleResponse(response,"error");
        }
      })
    }else{
			this.MesageAlertDataModel = {
        title: "Building Image",
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
	* For remove image file 
	**/
  removeFile(type) {
    if (type === 'image') {
      this.fileUrl = '';
      //this.floor_image_Ref.nativeElement.value = '';
    }
  }

  /**
	* For handle on file change
	**/
  handleImageInput(event) {
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
            that.buildingObject.image= this.fileUrl  
          }
        }
      } else {
        this.MesageAlertDataModel = {
          title: "Building Image",
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
	* For get building detail
	**/
  getBuildingDetails(buildingGuid) {
    this.spinner.show();
    this.buildingService.getbuildingDetails(buildingGuid).subscribe(response => {
      if (response.isSuccess === true) {
        if(response.data){
        this.buildingObject = response.data;
        if(this.buildingObject.image){
          this.buildingObject.image = this.mediaUrl + this.buildingObject.image;
          this.currentImage = this.buildingObject.image;
          this.hasImage=true;
        } else {
          this.hasImage=false;
        }
        this.buildingService.getcitylist(response.data.countryGuid).subscribe(response => {
          this.spinner.hide();
          this.stateList = response.data;
         
        
        });
      }else{
        this.router.navigate(['/buildings']);
      }
      }
    });
  }

  /**
	* For get country list
	**/
  getcountryList() {
    this.spinner.show();
    this.buildingService.getcountryList().subscribe(response => {
      this.spinner.hide();
      this.countryList = response.data;
    });
  }

  /**
	* For get state using country
	**/
  changeCountry(event) {
    this.buildingForm.controls['stateGuid'].setValue(null, { emitEvent: true })
    if(event){
      let id = event.value;
      this.spinner.show();
      this.buildingService.getcitylist(id).subscribe(response => {
        this.spinner.hide();
        this.stateList = response.data;
      });
    }
  }
}
