import { Component, OnInit, ViewEncapsulation } from '@angular/core'
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { UserService, AdminAuthGuired  } from '../../services/index'
import { Notification, NotificationService } from 'app/services';
import { IDSAuthService } from 'app/services/auth/idsauth.service'


@Component({
	selector: 'app-login',
	templateUrl: './login.component.html',
	styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
	loginform: FormGroup;
	checkSubmitStatus = false;
	loginObject = {};
	loginStatus = false;
	loggedIn: Boolean;
	constructor(
		private spinner: NgxSpinnerService,
		private router: Router,
		private _notificationService: NotificationService,
		public UserService : UserService,
		private authService: AdminAuthGuired,
		private IdsService: IDSAuthService
	) { 
		if (localStorage.getItem('currentUser'))  {
			let currentUser = JSON.parse(localStorage.getItem('currentUser'))
			if (currentUser.userDetail.isAdmin) {
			  this.router.navigate(['/admin/dashboard']);
			} else {
			  this.router.navigate(['/dashboard']);
			}
		  } else {
			this.spinner.show();
			this.IdsService.isLoggedInObs()
			.subscribe(flag => {
			  
			  this.loggedIn = flag;
			  if (!flag) {
			  this.IdsService.startSigninMainWindow(this.IdsService.browserCallBackURL);
			  }
			  else {
			  this.router.navigate(['/login']);
			  }
			  this.spinner.hide();
			});
		  }
	}

	ngOnInit() {
		/*this.loginStatus = this.authService.isCheckLogin();
		if (this.loginStatus === true) {
		  this.router.navigate(['dashboard']);
		}*/
		this.createFormGroup();
		// logout the person when he opens the app for the first time
		//this.UserService.logout();
	}

	/**
    * For create login formGroup
	**/
	createFormGroup() {
		this.loginform = new FormGroup({
			username: new FormControl('',[Validators.required,Validators.email]),
			password: new FormControl('',[Validators.required]),
		});

		this.loginObject = {
			username: '',
			password: '',
		};
	}

	/**
    * For fire login 
	**/
	login() {
		this.checkSubmitStatus = true;
		if (this.loginform.status === "VALID" && this.checkSubmitStatus) {
			this.spinner.show();
			this.UserService.login(this.loginObject).subscribe(response => {
				this.spinner.hide();
				if (response.isSuccess === true && response.data.access_token) {
					this._notificationService.handleResponse({message:'Logged in successfully'},"success");
					this.router.navigate(['/dashboard']);
				}
				else {
					this._notificationService.handleResponse(response,"error");
					this.router.navigate(['/login']);
				}
			}, error => {
				this.spinner.hide();
				this._notificationService.handleResponse(error,"error");
			});
		}
	}

	forgotPassword($event) {
		$("#divLoginSection").hide();
		$("#divForgotPwdSection").show();
	}
	
	forgotPasswordCancel($event) {
		$("#divForgotPwdSection").hide();
		$("#divLoginSection").show();

	}
}