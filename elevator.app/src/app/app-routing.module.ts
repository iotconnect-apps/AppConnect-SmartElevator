import { NgModule } from '@angular/core'
import { RouterModule, Routes } from '@angular/router'

import { SelectivePreloadingStrategy } from './selective-preloading-strategy'
import { PageNotFoundComponent } from './page-not-found.component'
import {
	HomeComponent, UserListComponent, UserAddComponent, BuildingListComponent, BuildingDetailsComponent,
	BuildingAddComponent, DeviceAddComponent, DashboardComponent, DeviceListComponent,
	LoginComponent, RegisterComponent, MyProfileComponent, ResetpasswordComponent, SettingsComponent,
	RolesListComponent, RolesAddComponent,
	ChangePasswordComponent, AdminLoginComponent, SubscribersListComponent,
	HardwareListComponent, HardwareAddComponent, ElevatorListComponent, ElevatorAddComponent, AdminDashboardComponent, SubscriberDetailComponent,
	BulkuploadAddComponent, NotificationListComponent, NotificationAddComponent, AdminNotificationListComponent, AdminNotificationAddComponent,
	ScheduledMaintenanceListComponent, ScheduledMaintenanceAddComponent, ElevatorDetailsComponent, AdminUserListComponent, 
	AdminUserAddComponent,AlertsComponent
} from './components/index'

import { AuthService, AdminAuthGuired } from './services/index'

const appRoutes: Routes = [
	{
		path: 'admin',
		children: [
			{
				path: '',
				component: AdminLoginComponent
			},
			{
				path: 'dashboard',
				component: AdminDashboardComponent,
				canActivate: [AuthService]
			},
			{
				path: 'subscribers/:email/:companyId',
				component: SubscriberDetailComponent,
				canActivate: [AuthService]
			},
			{
				path: 'subscribers',
				component: SubscribersListComponent,
				canActivate: [AuthService]
			},
			{
				path: 'hardwarekits',
				component: HardwareListComponent,
				canActivate: [AuthService]
      },
      {
        path: 'hardwarekits/bulkupload',
        component: BulkuploadAddComponent,
        canActivate: [AuthService]
      },
			{
        path: 'hardwarekits/add',
				component: HardwareAddComponent,
				canActivate: [AuthService]
			},
			{
        path: 'hardwarekits/:hardwarekitGuid',
				component: HardwareAddComponent,
				canActivate: [AuthService]
			},
			{
				path: 'users',
				component: AdminUserListComponent,
				canActivate: [AuthService]
			},
			{
        path: 'users/add',
				component: AdminUserAddComponent,
				canActivate: [AuthService]
			},
			{
        path: 'users/:userGuid',
				component: AdminUserAddComponent,
				canActivate: [AuthService]
			},
			{
				path: 'notification/:notificationGuid',
				component: AdminNotificationAddComponent,
				canActivate: [AuthService]
			},
			{
				path: 'notification/add',
				component: AdminNotificationAddComponent,
				canActivate: [AuthService]
			},
			{
				path: 'notification',
				component: AdminNotificationListComponent,
				canActivate: [AuthService]
			}
		]
	},
	{
		path: '',
		component: HomeComponent
	},
	{
		path: 'login',
		component: LoginComponent
	},
	{
		path: 'register',
		component: RegisterComponent
	},
	//App routes goes here
	{
		path: 'my-profile',
		component: MyProfileComponent,
		// canActivate: [AuthService]
	},
	{
		path: 'change-password',
		component: ChangePasswordComponent,
		// canActivate: [AuthService]
	},
	{
		path: 'dashboard',
		component: DashboardComponent,
		canActivate: [AdminAuthGuired]
	},
	{
    path: 'buildings/:buildingGuid',
		component: BuildingAddComponent,
		canActivate: [AdminAuthGuired]
	}, {
    path: 'buildings/add',
		component: BuildingAddComponent,
		//canActivate: [AdminAuthGuired]
	},
	{
    path: 'buildings/details/:buildingGuid',
		component: BuildingDetailsComponent,
		pathMatch: 'full'
		//canActivate: [AdminAuthGuired]
	},
	{
		path: 'buildings',
		component: BuildingListComponent,
		canActivate: [AdminAuthGuired]
	}, {
    path: 'users/:userGuid',
		component: UserAddComponent,
		canActivate: [AdminAuthGuired]
	}, {
    path: 'users/add',
		component: UserAddComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'users',
		component: UserListComponent,
		canActivate: [AdminAuthGuired]
	},
	{
		path: 'roles/:roleGuid',
		component: RolesAddComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'roles',
		component: RolesListComponent,
		canActivate: [AdminAuthGuired]
	},
	{
		path: 'maintenance',
		component: ScheduledMaintenanceListComponent,
		canActivate: [AdminAuthGuired]
	},
	{
		path: 'maintenance/add',
		component: ScheduledMaintenanceAddComponent,
		//canActivate: [AdminAuthGuired]
	},
	{
		path: 'maintenance/:guid',
		component: ScheduledMaintenanceAddComponent,
		canActivate: [AdminAuthGuired]
	},
	{
		path: 'device/:parentDeviceGuid',
		component: DeviceAddComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'device/:parentDeviceGuid/:childDeviceGuid',
		component: DeviceAddComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'devices',
		component: DeviceListComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'roles/:roleGuid',

		component: RolesAddComponent,
		canActivate: [AdminAuthGuired]
	},
	{
		path: 'elevators',
		component: ElevatorListComponent,
		canActivate: [AdminAuthGuired]
	},
	{
    path: 'elevators/add',
		component: ElevatorAddComponent,
		canActivate: [AdminAuthGuired]
	},
	{
    path: 'elevators/:elevatorGuid',
		component: ElevatorAddComponent,
		canActivate: [AdminAuthGuired]
	},
	{
		path: 'elevators/details/:elevatorGuid',
		component: ElevatorDetailsComponent,
		canActivate: [AdminAuthGuired]
	},
	/*{
		path: 'elevator-listing',
		component: RolesListComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'elevator-details',
		component: RolesDetailsComponent,
		canActivate: [AdminAuthGuired]
	},*/
	{
		path: 'notification/:notificationGuid',
		component: NotificationAddComponent,
		canActivate: [AdminAuthGuired]
	},
	{
		path: 'notification/add',
		component: NotificationAddComponent,
		canActivate: [AdminAuthGuired]
	},
	{
		path: 'notification',
		component: NotificationListComponent,
		canActivate: [AdminAuthGuired]
	},
	{
		path: 'alerts',
		component: AlertsComponent,
		canActivate: [AdminAuthGuired]
	},
  {
    path: 'alerts/:buildingGuid',
    component: AlertsComponent,
    canActivate: [AdminAuthGuired]
  },
  {
    path: 'alert/:elevatorGuid',
    component: AlertsComponent,
    canActivate: [AdminAuthGuired]
  },
	{
		path: '**',
		component: PageNotFoundComponent
	}
];

@NgModule({
	imports: [
		RouterModule.forRoot(
			appRoutes, {
			preloadingStrategy: SelectivePreloadingStrategy
		}
		)
	],
	exports: [
		RouterModule
	],
	providers: [
		SelectivePreloadingStrategy
	]
})

export class AppRoutingModule { }
