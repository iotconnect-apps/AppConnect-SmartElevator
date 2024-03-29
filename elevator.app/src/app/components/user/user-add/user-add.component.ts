import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators, FormBuilder } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { Notification, NotificationService, GatewayService } from 'app/services';
import { CustomValidators } from 'app/helpers/custom.validators';
import { UserService } from 'app/services';

@Component({
  selector: 'app-user-add',
  templateUrl: './user-add.component.html',
  styleUrls: ['./user-add.component.css']
})
export class UserAddComponent implements OnInit {
  public contactNoError:boolean=false;
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
  moduleName = "Add User";
  userObject = {};
  userGuid = '';
  isEdit = false;
  userForm: FormGroup;
  checkSubmitStatus = false;
  buildingList = [];
  selectedBuilding = '';
  floorList = [];
  spaceList = [];
  roleList = [];
  cityList = [];
  buttonname = 'Submit'
  //arrystatus = [{ "name": "Active", "value": true }, { "name": "Inactive", "value": false }]
  timezoneList = [];
  enitityList = [];
  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private _notificationService: NotificationService,
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    public userService: UserService,
    public gatewayService: GatewayService,
  ) {
    this.activatedRoute.params.subscribe(params => {
      if (params.userGuid != 'add') {
        this.getUserDetails(params.userGuid);
        this.userGuid = params.userGuid;
        this.moduleName = "Edit User";
        this.isEdit = true;
        this.buttonname = 'Update'
      } else {
        this.userObject = { firstName: '', lastName: '', email: '', contactNo: '', roleGuid: '', timezoneGuid: '', isActive: '' }
      }
    });
    this.createFormGroup();
  }

  ngOnInit() {
    this.getRoleList();
    this.getTimezoneList();
  }

  /**
  * For Create form group for user
  */
  createFormGroup() {
    this.userForm = new FormGroup({
      firstName: new FormControl('', [Validators.required, Validators.pattern('^[A-Za-z0-9 ]+$')]),
      lastName: new FormControl('', [Validators.required, Validators.pattern('^[A-Za-z0-9 ]+$')]),
      email: new FormControl('', [Validators.required, Validators.email]),
      contactNo: new FormControl('', [Validators.required]),
      entityGuid: new FormControl(''),
      isActive: new FormControl('', [Validators.required]),
      roleGuid: new FormControl(null, [Validators.required]),
      timeZoneGuid: new FormControl('', [Validators.required])
    }), {
      validators: CustomValidators.checkPhoneValue('contactNo')
    };
  }

    /**
  * For Get role list
  */
  getRoleList() {
    this.spinner.show();
    this.userService.getroleList().subscribe(response => {
      this.spinner.hide();
      this.roleList = response.data;
    });
  }

    /**
  * For Get time zone list
  */
  getTimezoneList() {
    this.spinner.show();
    this.userService.getTimezoneList().subscribe(response => {
      this.spinner.hide();
      this.timezoneList = response.data;
    });
  }

      /**
  * For Get entity list
  */
  getEntityList() {
    this.spinner.show();
    this.gatewayService.getBuildingLookup().subscribe(response => {
      this.spinner.hide();
      this.enitityList = response.data;
    });
  }

  /**
  * For Add New User
  */
  addUser() {
    this.checkSubmitStatus = true;
    let contactNo = this.userForm.value.contactNo.replace("(", "")
    let contactno = contactNo.replace(")", "")
    let finalcontactno = contactno.replace("-", "")
    if(finalcontactno.match(/^0+$/)){
      this.contactNoError=true;
      return
    } else {
      this.contactNoError=false;
    }
    if (this.isEdit) {
      this.userForm.registerControl("id", new FormControl(''));
      this.userForm.patchValue({ "id": this.userGuid });
      this.userForm.get('isActive').setValue(this.userObject['isActive']);
    }
    else {
      this.userForm.get('isActive').setValue(true);
    }
    if (this.userForm.status === "VALID") {
      this.spinner.show();

      let currentUser = JSON.parse(localStorage.getItem('currentUser'));
      this.userForm.get('entityGuid').setValue(currentUser.userDetail.entityGuid);
      this.userForm.get('contactNo').setValue(contactno);
      this.userService.addUser(this.userForm.value).subscribe(response => {
        if (response.isSuccess === true) {
          this.spinner.hide();
          if (response.data.updatedBy != null) {
            this._notificationService.handleResponse({message:"User updated successfully."},"success");
          } else {
            this._notificationService.handleResponse({message:"User created successfully."},"success");
          }
          this.router.navigate(['/users']);
        } else {
          this.spinner.hide();
          this._notificationService.handleResponse(response,"error");
        }
      })
    }
  }

    /**
  * For Get User List
  */
  getUserDetails(userGuid) {
    this.spinner.show();
    this.userService.getUserDetails(userGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.userObject = response.data;
        //this.fileUrl = this.deviceObject['image'];
      }
    });
  }

      /**
  * For Convert text to lowercase
  */
  getdata(val) {
    return val = val.toLowerCase();
  }
}
