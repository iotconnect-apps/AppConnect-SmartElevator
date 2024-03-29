import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { AppConstant } from "../../../app.constants";
import { Guid } from "guid-typescript";
import { Observable, forkJoin } from 'rxjs';
import { NotificationService, Notification, DeviceService, GatewayService, LookupService } from '../../../services';



export interface DeviceTypeList {
	id: number;
	type: string;
}
export interface StatusList {
	id: boolean;
	status: string;
}
@Component({
	selector: 'app-device-add',
	templateUrl: './device-add.component.html',
	styleUrls: ['./device-add.component.css']
})


export class DeviceAddComponent implements OnInit {
	currentUser: any;
	fileUrl: any;
	fileName = '';
	fileToUpload: any = null;
	status;
	moduleName = "Add Device";
	parentDeviceObject: any = {};
	deviceObject = {};
	deviceGuid = '';
	parentDeviceGuid = '';
	isEdit = false;
	deviceForm: FormGroup;
	checkSubmitStatus = false;
	templateList = [];
	buildingList = [];
	tagList = [];


	statusList: StatusList[] = [
		{
			id: true,
			status: 'Active'
		},
		{
			id: false,
			status: 'In-active'
		}

	];
	constructor(
		private router: Router,
		private _notificationService: NotificationService,
		private activatedRoute: ActivatedRoute,
		private spinner: NgxSpinnerService,
		private deviceService: DeviceService,
		private gatewayService: GatewayService,
		private lookupService: LookupService,
		public _appConstant: AppConstant
	) {
		this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
		this.activatedRoute.params.subscribe(params => {
			// set data for parent device
			if (params.parentDeviceGuid != 'add' && !params.childDeviceGuid) {
				this.getAllDeviceData(params.parentDeviceGuid);
				this.parentDeviceGuid = params.parentDeviceGuid;
				this.moduleName = "Edit Device";
				this.isEdit = true;
			}
			// set data for child device
			if(params.parentDeviceGuid != 'add' && params.childDeviceGuid){
				this.getChildDeviceData(params.childDeviceGuid);
				this.parentDeviceGuid = params.parentDeviceGuid;
				this.deviceGuid = params.childDeviceGuid;
				this.moduleName = "Edit Child Device";
				this.isEdit = true;

			}

		});
	}

	// before view init
	ngOnInit() {
		this.createFormGroup();
	}

	

	createFormGroup() {
		this.deviceForm = new FormGroup({
			uniqueId: new FormControl('', [Validators.required]),
			name: new FormControl('', [Validators.required]),
			type: new FormControl(0),
			parentDeviceGuid: new FormControl((this.parentDeviceGuid) ? this.parentDeviceGuid : null),
			tag: new FormControl('', [Validators.required]),
			note: new FormControl(''),
			companyGuid: new FormControl(null),
			buildingGuid: new FormControl(Guid.EMPTY),
			templateGuid: new FormControl(Guid.EMPTY),
			isProvisioned: new FormControl(false),
			isActive: new FormControl(true)
		});
	}

	/**
	 * Get all the data related to parent device using forkjoin (Combine services)
	 * 
	 * @param deviceGuid 
	 * 
	 */
	getAllDeviceData(deviceGuid) {

		this.spinner.show();
		let getParentDeviceDetails = this.gatewayService.getgatewayDetails(deviceGuid);
		//let getBuildingLookup = this.gatewayService.getBuildingLookup(); forkJoin([getParentDeviceDetails, getBuildingLookup, getTemplateLookup])
		let getTemplateLookup = this.gatewayService.getTemplateLookup();
		// capture response until all the APIs got success
		forkJoin([getParentDeviceDetails, getTemplateLookup])
			.subscribe(response => {
				this.setParentDeviceDetails(response[0]);
				//this.setBuildingLookup(response[1]);
				this.setTemplateLookup(response[1]);
				this.spinner.hide();
			}, error => {
				this.spinner.hide();
				this._notificationService.handleResponse(error,"error");
			});

	}


	/**
	 * Get all the data related to parent device using forkjoin (Combine services)
	 * 
	 * @param deviceGuid 
	 * 
	 */
	getChildDeviceData(deviceGuid) {

		this.spinner.show();
		this.deviceService.getDeviceDetails(deviceGuid).subscribe(response => {
			if (response.isSuccess === true) {
				this.deviceObject = response.data;
			} else {
				this._notificationService.handleResponse(response,"error");
			}
		}, error => {
			this.spinner.hide();
			this._notificationService.handleResponse(error,"error");
		});
	}

	/**
	 * set parent device details
	 * @param response 
	 */
	setParentDeviceDetails(response) {
		if (response.isSuccess === true) {
			this.parentDeviceObject = response.data;
			//Get tags lookup once parent device data is fetched
			this.getTagsLookup();
		} else {
			this._notificationService.handleResponse(response,"error");
		}

	}
	/**
	 * set Building lookup
	 * @param response 
	*/
	setBuildingLookup(response) {
		if (response.isSuccess === true) {
			this.buildingList = response['data'];
		} else {
			this._notificationService.handleResponse(response,"error");
		}
	}

	/**
	 * set template lookup
	 * only gateway supported template
	 *  @param response 
	 */
	setTemplateLookup(response) {
		if (response.isSuccess === true) {
			this.templateList = response['data'];
		} else {
			this._notificationService.handleResponse(response,"error");
		}
	}

	/**
	 * Get tags lookup once parent device data is fetched
	 */
	getTagsLookup() {

		if (this.parentDeviceObject) {
			this.lookupService.getTagsLookup(this.parentDeviceObject.templateGuid).
				subscribe(response => {
					if (response.isSuccess === true) {
						this.tagList = response['data'];
					} else {
						this._notificationService.handleResponse(response,"error");
					}
				}, error => {
					this.spinner.hide();
					this._notificationService.handleResponse(error,"error");
				})
		}

	}
	
	/**
	 * Find a value from the look up data
	 * 
	 * @param obj 
	 * 
	 * @param findByvalue 
	 * 
	 */
	getIndexByValue(obj, findByvalue) {
		let index = obj.findIndex(
			(tmpl) => { return (tmpl.value == findByvalue.toUpperCase()) }
		);
		if (index > -1) return obj[index].text;
		return;
	}


	/**
	 * Add device under gateway
	 * only gateway supported device
	 */
	addChildDevice() {
		this.checkSubmitStatus = true;
		if (this.deviceForm.status === "VALID") {
			this.spinner.show();
			// overwrite default values
			this.deviceForm.patchValue({ 'companyGuid': this.currentUser.userDetail.companyId });
			(this.deviceForm.value.note == "" || typeof this.deviceForm.value.note === "undefined")
				? this.deviceForm.patchValue({ 'note': "" })
				: "";

			let successMessage = this._appConstant.msgCreated.replace("modulename", "Device");
			this.deviceService.addUpdateDevice(this.deviceForm.value).subscribe(response => {
				this.spinner.hide();
				if (response.isSuccess === true) {
					this.deviceForm.reset();
					this._notificationService.handleResponse({message:successMessage},"success");
				} else {
					this._notificationService.handleResponse(response,"error");
				}
			}, error => {
				this.spinner.hide();
				this._notificationService.handleResponse(error,"error");
			});

		}
	}


}
