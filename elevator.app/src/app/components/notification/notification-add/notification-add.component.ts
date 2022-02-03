import { Component, OnInit } from '@angular/core';
import { DeviceService, RuleService, NotificationService, GatewayService } from 'app/services';
import { NgxSpinnerService } from 'ngx-spinner';
import { FormControl, FormArray, FormGroup, Validators } from '@angular/forms'
import { Router, ActivatedRoute } from '@angular/router';
import { Notification } from 'app/services/notification/notification.service';
import * as _ from "lodash";
import { upperCase } from '@rxweb/reactive-form-validators';

@Component({
  selector: 'app-notification-add',
  templateUrl: './notification-add.component.html',
  styleUrls: ['./notification-add.component.css']
})
export class NotificationAddComponent implements OnInit {

  templateList = [];
  attributeList = [];
  conditionList = [];
  severityList = [];
  commandList = [];
  userList = [];
  roleList = [];
  entityList = [];
  deviceList = [];
  postForm: FormGroup;
  checkSubmitStatus = false;
  isEdit = false;
  ruleType = '1';
  applyTo = '1';
  notificationGuid = '';
  notificationDetail = {};
  deviceCommandType = false;
  formLoad = false;
  myNotification: any = [
    {
      name: "DeviceCommand",
      value: "devicecommand",
      binary: 4,
      index: 2

    },
    {
      name: "Email",
      value: "email",
      binary: 8,
      index: 3
    },

    {
      name: "Push",
      value: "push",
      binary: 16,
      index: 4
    },

    {
      name: "SignalR",
      value: "singalr",
      binary: 64,
      index: 6
    },
    {
      name: "WebHook",
      value: "webhook",
      binary: 128,
      index: 7
    },
    {
      name: "UI Alert",
      value: "uialert",
      binary: 256,
      index: 8
    },
    {
      name: "MQTT",
      value: "mqtt",
      binary: 512,
      index: 9
    },



  ];
  selectedNotification: [string];
  attributeGuidValidationMsg = false;
  condtionValueValidationMsg = false;
  commandTextValidationMsg = false;
  commandValueValidationMsg = false;
  title = 'ADD NOTIFICATION';
  constructor(
    private gatewayService: GatewayService,
    private ruleService: RuleService,
    private spinner: NgxSpinnerService,
    private _notificationService: NotificationService,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) { }

  ngOnInit() {
    this.activatedRoute.params.subscribe(params => {
      if (params['notificationGuid'] !== 'add') {
        this.isEdit = true;
        this.notificationGuid = params['notificationGuid'];
        this.getNotificationDetail();
        this.title = 'Edit NOTIFICATION';

      } else {
        this.formLoad = true;
        this.getTemplateLookup();
        this.getSeveritylevelLookup();
        this.getConditionLookup();
        this.getUsersLookup();
        this.getRoleLookup();
        this.getEntityLookup();

      }
    });
    this.createFormGroup();
  }

  /**
  * For get user lookup
  */
  getUsersLookup() {
    this.spinner.show();
    this.ruleService.getUsersLookup().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.userList = response.data;
        if (this.isEdit) {
          this.postForm.patchValue({ users: this.notificationDetail['users'] })
        }
      } else {
        this.userList = [];
      }
    }, error => {
      this.spinner.hide();
      this.userList = [];
    });
  }

  /**
  * For get role lookup
  */
  getRoleLookup() {
    this.spinner.show();
    this.ruleService.getRoleLookup().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.roleList = response.data;
        if (this.isEdit) {
          this.postForm.patchValue({ roles: this.notificationDetail['roles'] })
        }
      } else {
        this.roleList = [];
      }
    }, error => {
      this.spinner.hide();
      this.roleList = [];
    });
  }

  /**
  * For get Entity lookup
  */
  getEntityLookup() {
    this.spinner.show();
    this.ruleService.getEntityLookup().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.entityList = response.data;
        if (this.isEdit) {
          this.postForm.patchValue({ entityGuid: this.notificationDetail['entityGuid'] })
        }
      } else {
        this.entityList = [];
      }
    }, error => {
      this.spinner.hide();
      this.entityList = [];
    });
  }

  /**
  * For manage permission check
  */
  checkIschecked(modulePermission, index) {
    if ((modulePermission & (1 << index)) >> index) {
      return true;
    } else {
      return false;
    }
  }

  /**
  * For manage attribute
  */
  clickAttribute(tag, name) {
    let val = this.postForm.controls['conditionText'].value;
    this.postForm.patchValue({ conditionText: val + tag + '#' + name });
  }

  /**
  * For get notification detail
  */
  getNotificationDetail() {
    this.spinner.show();
    this.ruleService.getUserRuleDetail(this.notificationGuid).subscribe(response => {
      this.spinner.hide();
      let that = this;
      let dataNotifcation = [];
      if (response.isSuccess === true) {
        this.notificationDetail = response.data;
        this.myNotification.forEach(function (element, i) {
          element['selected'] = that.checkIschecked(that.notificationDetail['eventSubscription']['deliveryMethod'], element['index']);
          dataNotifcation.push(element['selected']);
        });
        this.getSelectedNotification();
        this.postForm.patchValue({ notificationTypes: dataNotifcation })
        that.selectedNotification = this.postForm.controls['notificationTypes'].value;
        this.deviceCommandType = dataNotifcation[0];
        //that.selectedNotification.push(element['selected']);
        //
        this.createFormGroup();
        this.formLoad = true;
        this.postForm.patchValue({
          name: this.notificationDetail['name'],
          ruleType: this.notificationDetail['ruleType'].toString(),
          applyTo: this.notificationDetail['applyTo'].toString(),
          ignorePreference: this.notificationDetail['ignorePreference']
        });
        this.postForm.patchValue({});
        this.ruleType = this.notificationDetail['ruleType'].toString();
        this.applyTo = this.notificationDetail['applyTo'].toString();
        this.getTemplateLookup();
        this.getSeveritylevelLookup();
        this.getConditionLookup();
        this.getTemplateAttributeLookup();
        this.getUsersLookup();
        this.getRoleLookup();
        this.getEntityLookup();
      } else {
        this.router.navigate(['notification']);
        this._notificationService.handleResponse({message:'Notification not found'},"error");
      }
    }, error => {
      this.spinner.hide();
      this.router.navigate(['notification']);
      this._notificationService.handleResponse({message:'Notification not found'},"error");
    });
  }

  /**
  * For create form group for notification
  */
  createFormGroup() {
    this.postForm = new FormGroup({
      templateGuid: new FormControl('', [Validators.required]),
      ruleType: new FormControl('', [Validators.required]),
      applyTo: new FormControl('', [Validators.required]),
      name: new FormControl('', [Validators.required]),
      conditionText: new FormControl('', [Validators.required]),
      conditionValue: new FormControl(''),
      severityLevelGuid: new FormControl('', [Validators.required]),
      attributeGuid: new FormControl(''),
      notificationTypes: this.createNotifications(this.myNotification),
      commandText: new FormControl(''),
      commandValue: new FormControl(''),
      users: new FormControl(''),
      roles: new FormControl(''),
      entityGuid: new FormControl(''),
      devices: new FormControl(''),
      ignorePreference: new FormControl(''),
    });


    if (!this.isEdit) {
      this.postForm.patchValue({ ruleType: this.ruleType });
      this.postForm.patchValue({ applyTo: this.applyTo });
      this.postForm.patchValue({ ignorePreference: false });
    }

  }

  /**
  * For check entity
  */
  checkEntiy() {
    if (this.applyTo === '1') {
      this.applyTo = '2'
    } else {
      this.applyTo = '1'
    }
    this.postForm.patchValue({ applyTo: this.applyTo });
  }

  /**
  * For get selected notification
  */
  getSelectedNotification() {
    this.selectedNotification = _.map(
      this.postForm.controls.notificationTypes["controls"],
      (note, i) => {
        if (this.myNotification[i]['name'] === 'DeviceCommand') {
          this.deviceCommandType = note.value;
          this.commandTextValidationMsg = false;
          this.commandValueValidationMsg = false;
        }

        return note.value && this.myNotification[i].value;
      }
    );
    this.getSelectedNotificationName();
  }

  /**
  * For get selected notification name
  */
  getSelectedNotificationName() {
    this.selectedNotification = _.filter(
      this.selectedNotification,
      function (note) {
        if (note !== false) {
          return note;
        }
      }
    );
  }

  /**
  * For create notification
  */
  createNotifications(notesInputs) {
    const arr = notesInputs.map(note => {
      return new FormControl(note.selected || false);
    });
    return new FormArray(arr);
  }

  /**
  * For rule type change
  */
  ruleTypeChange() {
    this.ruleType = this.postForm.controls['ruleType'].value;
    this.postForm.patchValue({
      conditionValue: '',
      conditionText: '',
      attributeGuid: ''
    });
    this.attributeGuidValidationMsg = false;
    this.condtionValueValidationMsg = false;
  }

  /**
  * For rule apply on change
  */
  ruleApplyONChange() {
    this.applyTo = this.postForm.controls['applyTo'].value;
  }

  /**
  * For submit notification
  */
  submitForm() {

    this.checkSubmitStatus = true;
    let typeOneValidation = true;
    if (this.ruleType === '1') {
      if (this.postForm.controls["attributeGuid"].value === '' || this.postForm.controls["conditionValue"].value === '') {
        typeOneValidation = false;
        if (this.postForm.controls["attributeGuid"].value === '') {
          this.attributeGuidValidationMsg = true;
        }
        if (this.postForm.controls["conditionValue"].value === '') {
          this.condtionValueValidationMsg = true;
        }

      } else {
        this.attributeGuidValidationMsg = false;
        this.condtionValueValidationMsg = false;
      }
    }
    if (this.deviceCommandType) {
      if (this.postForm.controls["commandText"].value === '' || this.postForm.controls["commandValue"].value === '') {
        typeOneValidation = false;
        if (this.postForm.controls["commandText"].value === '') {
          this.commandTextValidationMsg = true;
        }
        if (this.postForm.controls["commandValue"].value === '') {
          this.commandValueValidationMsg = true;
        }

      } else {

        this.commandTextValidationMsg = false;
        this.commandValueValidationMsg = false;
      }

    }
    if (this.postForm.status === "VALID" && typeOneValidation) {
      this.spinner.show();
      let postData = this.postForm.value;
      if (this.isEdit) {
        postData["guid"] = this.notificationGuid;
      }
      postData["ruleType"] = parseInt(postData["ruleType"]);
      if (postData["ruleType"] === 2) {
        postData["attributeGuid"] = '';
        postData["conditionValue"] = '';
      } else {
        let attributeInfo = postData["attributeGuid"];
        postData["attributeGuid"] = attributeInfo['guid'];
        postData["conditionText"] = attributeInfo['tag'] + '#' + attributeInfo['localName'] + ' ' + postData["conditionText"] + ' ' + postData["conditionValue"];

      }
      postData["applyTo"] = parseInt(postData["applyTo"]);
      if (postData["applyTo"] === 3) {
        postData["enityGuid"] = '';
      } else {
        postData["devices"] = [];
      }
      if (this.deviceCommandType) {
        postData['commandGuid'] = postData['commandText'];
        postData['parameterValue'] = postData['commandValue'];
      }
      postData["notificationType"] = 0;
      postData["deliveryMethod"] = [];
      let that = this;
      this.postForm.controls['notificationTypes'].value.forEach(function (element, i) {
        if (element) {
          postData["notificationType"] = postData["notificationType"] + that.myNotification[i]['binary'];
          postData["deliveryMethod"].push(that.myNotification[i]['name']);
        }
      });
      if (postData["users"] === '') {
        postData["users"] = [];
      }
      if (postData["roles"] === '') {
        postData["roles"] = [];
      }
      this.ruleService.manageUserRule(postData).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          this.router.navigate(['/notification']);
          if (this.isEdit) {
            this._notificationService.handleResponse({message:'Rule updated successfully.'},"success");
          } else {
            this._notificationService.handleResponse({message:'Rule created successfully.'},"success");

          }
        }
        else {
          this._notificationService.handleResponse(response,"error");
        }
      }, error => {
        this.spinner.hide();
      });

    }
  }

  /**
  * For verify condition
  */
  verifyCondition() {
    if (this.postForm.controls['conditionText'].value && this.postForm.controls['templateGuid'].value) {
      this.ruleService.verifyCondtion({ expression: this.postForm.controls['conditionText'].value, deviceTemplateGuid: this.postForm.controls['templateGuid'].value }).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          this._notificationService.handleResponse({message:"Condition verified successfully"},"success");
        }
        else {
          this._notificationService.handleResponse(response,"error");
        }
      }, error => {
        this.spinner.hide();
      });
    }
    else{
      if(this.postForm.controls['templateGuid'].value === ''){
        this._notificationService.handleResponse({message:"please select template"},"error");
      }else{
        this._notificationService.handleResponse({message:"please enter condtions"},"error");
      }
    }
  }

  
  /**
  * For get template lookup
  */
  getTemplateLookup() {
    this.gatewayService.getTemplateLookup().subscribe(response => {
      if (response.isSuccess === true) {
        this.templateList = response.data;
        if (this.isEdit) {
          this.postForm.patchValue({ templateGuid: (this.notificationDetail['templateGuid'].toUpperCase()) })
          this.getTemplateAttributeLookup();
        }
      } else {
        this.templateList = [];
      }

    }, error => {
      this.spinner.hide();
      this.templateList = [];
    });
  }

  /**
  * For get Template Attribute Lookup
  */
  getTemplateAttributeLookup() {
    if (this.postForm.controls['templateGuid'].value !== '') {

      this.gatewayService.getKitAttribueLookup(this.postForm.controls['templateGuid'].value).subscribe(response => {
        if (response.isSuccess === true) {
          this.attributeList = response.data;
          if (this.isEdit) {
            let that = this;
            var filterObj = this.attributeList.filter(function (e) {
              return e.guid == that.notificationDetail['attributeGuid'][0];
            });
            this.postForm.patchValue({ attributeGuid: filterObj[0] })
          }
        } else {
          this.attributeList = [];
        }

      }, error => {
        this.spinner.hide();
        this.attributeList = [];
      });

      this.gatewayService.getTemplateCommandLookup(this.postForm.controls['templateGuid'].value).subscribe(response => {
        if (response.isSuccess === true) {
          this.commandList = response.data;
          if (this.isEdit && this.notificationDetail['eventSubscription']['dataXml']['command']['guid']) {
            this.postForm.patchValue({ commandText: this.notificationDetail['eventSubscription']['dataXml']['command']['guid'] })
            this.postForm.patchValue({ commandValue: this.notificationDetail['eventSubscription']['dataXml']['command']['text'] })
          }
        } else {
          this.commandList = [];
        }

      }, error => {
        this.spinner.hide();
        this.commandList = [];

      });

      this.ruleService.getDeviceLookup(this.postForm.controls['templateGuid'].value).subscribe(response => {
        if (response.isSuccess === true) {
          this.deviceList = response.data;
          if (this.isEdit) {
            this.postForm.patchValue({ devices: (this.notificationDetail['devices']) })
          }
        } else {
          this.deviceList = [];
        }

      }, error => {
        this.spinner.hide();
        this.deviceList = [];

      });
    }
  }

  /**
  * For get Severity level Lookup
  */
  getSeveritylevelLookup() {
    this.spinner.show();
    this.ruleService.getSeveritylevelLookup().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.severityList = response.data;


        if (this.isEdit && this.notificationDetail['eventSubscription']['severityLevelGuid']) {
          this.postForm.patchValue({ severityLevelGuid: this.notificationDetail['eventSubscription']['severityLevelGuid'] })
        }
      } else {
        this.severityList = [];
      }
    }, error => {
      this.spinner.hide();
      this.severityList = [];
    });
  }

  /**
  * For get Condition Lookup
  */
  getConditionLookup() {
    this.spinner.show();
    this.ruleService.getConditionLookup().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.conditionList = response.data;
        if (this.isEdit) {
          if (this.notificationDetail['ruleType'] === 1) {
            var conditionText = this.notificationDetail['conditionText'];
            conditionText = conditionText.split(' ');
            this.postForm.patchValue({ conditionText: conditionText[1] });
            this.postForm.patchValue({ conditionValue: conditionText[2] });
          } else {
            this.postForm.patchValue({ conditionText: this.notificationDetail['conditionText'] });
            this.postForm.patchValue({ conditionValue: '' });
          }

        }
      } else {
        this.conditionList = [];
      }
    }, error => {
      this.spinner.hide();
      this.conditionList = [];
    });
  }

}
