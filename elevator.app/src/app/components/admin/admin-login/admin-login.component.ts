import { Component, OnInit, ViewEncapsulation } from '@angular/core'
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { UserService, AuthService } from '../../../services/index'
import { Notification, NotificationService } from 'app/services';


@Component({
	selector: 'app-admin-login',
	templateUrl: './admin-login.component.html',
	styleUrls: ['./admin-login.component.css']
})
export class AdminLoginComponent implements OnInit {
	loginform: FormGroup;
	checkSubmitStatus = false;
	loginObject = {};
	loginStatus = false;
	currentUser: any
	constructor(
		private spinner: NgxSpinnerService,
		private router: Router,
		private _notificationService: NotificationService,
		public UserService: UserService,
		private authService: AuthService
	) { }

	ngOnInit() {
		this.createFormGroup();
		if (localStorage.getItem("currentUser")) {
			this.logout();
		}
		// logout the person when he opens the app for the first time
	}

	/**
   	* For Logout from application 
   	**/
	logout() {
		this.currentUser = JSON.parse(localStorage.getItem("currentUser"));
		this.authService.logout();
		if (this.currentUser.userDetail.isAdmin) {
			this.router.navigate(['/admin'])
		} else {
			this.router.navigate(['/login'])
		}
	}

	/**
   	* For Create Login FormGroup 
   	**/
	createFormGroup() {
		this.loginform = new FormGroup({
			username: new FormControl('', [Validators.required, Validators.email]),
			password: new FormControl('', [Validators.required]),
		});

		this.loginObject = {
			username: '',
			password: '',
		};
	}

	/**
   	* For Create Login Call  
   	**/
	login() {
		this.checkSubmitStatus = true;
		if (this.loginform.status === "VALID" && this.checkSubmitStatus) {
			this.spinner.show();
			this.UserService.loginadmin(this.loginObject).subscribe(response => {
				this.spinner.hide();
				if (response.isSuccess === true && response.data.access_token) {
					this._notificationService.handleResponse({message:"Logged in successfully."},"success");
					this.router.navigate(['admin/dashboard']);
				}
				else {
					this._notificationService.handleResponse(response,"error");
					this.router.navigate(['/admin']);
				}
			}, error => {
				this.spinner.hide();
				this._notificationService.handleResponse(error,"error");
			});
		}
	}

	/**
   	* For handle forgot password button  
   	**/
	forgotPassword($event) {
		$("#divLoginSection").hide();
		$("#divForgotPwdSection").show();
	}

	/**
   	* For handle forgot password cancel button  
   	**/
	forgotPasswordCancel($event) {
		$("#divForgotPwdSection").hide();
		$("#divLoginSection").show();
	}
}
