import { Component, OnInit, ViewEncapsulation } from '@angular/core'
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { UserService, AuthService, AdminAuthGuired, NotificationService, Notification } from '../../services/index'
import { IDSAuthService } from '../../services/auth/idsauth.service'


@Component({
  selector: 'app-callback',
  templateUrl: './callback.component.html',
  styleUrls: ['./callback.component.css']
})
export class CallbackComponent implements OnInit {
  loginform: FormGroup;
  checkSubmitStatus = false;
  loginObject = {};
  loginStatus = false;
  loggedIn: Boolean;
  constructor(
    private spinner: NgxSpinnerService,
    private router: Router,
    private _notificationService: NotificationService,
    public UserService: UserService,
    private authService: AdminAuthGuired,
    private IdsService: IDSAuthService
  ) {

  }

  ngOnInit() {
    
    let isLoggedInUser = this.IdsService.getLogedInUserIDSData();

    isLoggedInUser
      .subscribe((loggedin) => {

        if (loggedin != undefined && loggedin != null) {
          this.authService.logout();
          this.IdsService.endSignoutMainWindow();
          this.router.navigate(['/login']);
          return;
        }
        else {
          let isLoggedIn = this.IdsService.endSigninMainWindow();
          isLoggedIn
            .subscribe((loggedin) => {
              if (loggedin.access_token) {
                this.UserService.getUserDetailByToken(loggedin.access_token).subscribe(response => {
                  if (response.isSuccess === true) {
                    let data = {
                      "access_token": loggedin.access_token,
                      "userDetail": {
                        "id": response.data.data.userGuid,
                        "companyId": response.data.data.companyGuid,
                        "roleId": response.data.data.roleGuid,
                        "roleName": response.data.data.roleName,
                        "cpId": response.data.data.cpId,
                        "entityGuid": response.data.data.entityGuid,
                        "solutionGuid": response.data.data.solutionGuid,
                        "fullName": response.data.data.firstName + ' ' + response.data.data.lastName
                      }
                    }
                    localStorage.setItem('currentUser', JSON.stringify(data));
                    this._notificationService.handleResponse({ message: 'Logged in successfully' }, "success");
                    this.router.navigate(['/dashboard']);
                  }
                  else {
                    this.IdsService.endSignoutMainWindow();
                    this._notificationService.handleResponse({ message: response.message }, "error");
                    this.router.navigate(['/']);
                    setTimeout(() => {
                    }, 10000);
                  }
                }, error => {
                  this.spinner.hide();
                  this._notificationService.handleResponse(error, "error");
                });
              }
            })
        }
      }
      )


  }
}
