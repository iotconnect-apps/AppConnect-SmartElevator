import { BrowserModule } from '@angular/platform-browser'
import { NgModule, APP_INITIALIZER } from '@angular/core'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { RxReactiveFormsModule } from '@rxweb/reactive-form-validators'
import { HttpModule } from '@angular/http'
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http'
import { NgxSpinnerModule } from 'ngx-spinner'
import { CookieService } from 'ngx-cookie-service'
import { SocketIoConfig, SocketIoModule } from 'ng-socket-io'
import { NgxPaginationModule } from 'ngx-pagination'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'
import { TagInputModule } from 'ngx-chips'
import { DateTimeAdapter, OWL_DATE_TIME_FORMATS, OWL_DATE_TIME_LOCALE, OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime'
import { MomentDateTimeAdapter } from 'ng-pick-datetime-moment'
import { MatButtonModule, MatCheckboxModule, MatInputModule, MatProgressBarModule, MatSelectModule, MatSlideToggleModule, MatTabsModule, MatRadioModule } from '@angular/material'
import { Ng2GoogleChartsModule } from 'ng2-google-charts'
import { FullCalendarModule } from '@fullcalendar/angular'
import { AgmCoreModule } from '@agm/core'
import { AgmJsMarkerClustererModule } from '@agm/js-marker-clusterer'
import { AgmDirectionModule } from 'agm-direction'

import { AppRoutingModule } from './app-routing.module'
import { AppComponent } from './app.component'
import { PageNotFoundComponent } from './page-not-found.component'
import { MatSidenavModule } from '@angular/material/sidenav'
import { MatTableModule } from '@angular/material/table'
import {
	MatDialogModule, MatIconModule, MatPaginatorModule,
	MatCardModule, MatTooltipModule, MatSortModule
} from '@angular/material';

import { RoundProgressModule } from 'angular-svg-round-progressbar';

import { JwtInterceptor } from './helpers/jwt.interceptor';

import { TextMaskModule } from 'angular2-text-mask';
import { AppConstant } from './app.constants';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { ClickOutsideModule } from 'ng-click-outside';

// import custom pipes
import { ShortNumberPipe } from './helpers/pipes/short-number.pipe';
import { ReplacePipe } from './helpers/pipes/replace.pipe';
import { ShortNumberFixnumberPipe } from './helpers/pipes/short-number-fixnumber.pipe';
import { MatDatepickerModule, MatNativeDateModule } from '@angular/material';
import { ChartsModule } from 'ng2-charts';

import {
	BulkuploadAddComponent, AdminDashboardComponent, AdminLoginComponent, HomeComponent, RolesListComponent, RolesAddComponent, UserListComponent, UserAddComponent,
	RegisterComponent, PaymentComponent,
	PurchasePlanComponent, DeviceAddComponent, FlashMessageComponent, ConfirmDialogComponent, DashboardComponent,
	DeleteDialogComponent, DeviceListComponent, ExtendMeetingComponent, FooterComponent, MessageDialogComponent,
	HeaderComponent, LoginHeaderComponent, LoginFooterComponent, LeftMenuComponent, LoginComponent,
	PageSizeRenderComponent, PaginationRenderComponent, ResetpasswordComponent,
	SearchRenderComponent, SettingsComponent,
	MyProfileComponent, ChangePasswordComponent,
	SubscribersListComponent, HardwareListComponent, HardwareAddComponent, SubscriberDetailComponent,
	NotificationListComponent, NotificationAddComponent, AdminNotificationListComponent, AdminNotificationAddComponent,
	ElevatorAddComponent, ElevatorListComponent,
	BuildingListComponent, BuildingAddComponent, BuildingDetailsComponent,
  ScheduledMaintenanceListComponent, ScheduledMaintenanceAddComponent, AdminUserListComponent, 
  AdminUserAddComponent,AlertsComponent
} from './components/index'

import {
	AuthService, AdminAuthGuired, NotificationService, ElevatorService,
	ApiConfigService, DashboardService, UsersService, DeviceService, RolesService, SettingsService,
	ConfigService, LookupService, SubscriptionService, RuleService
} from './services/index';
import { TooltipDirective } from './helpers/tooltip.directive';
import { ElevatorDetailsComponent } from './components/elevator/elevator-details/elevator-details.component';
//import { RolesDetailsComponent } from './components/roles/roles-details/roles-details.component';
import { NgSelectModule } from '@ng-select/ng-select';


const config: SocketIoConfig = { url: 'http://localhost:2722', options: {} };
const MY_NATIVE_FORMATS = {
	parseInput: 'DD-MM-YYYY',
	fullPickerInput: 'DD-MM-YYYY hh:mm a',
	datePickerInput: 'DD-MM-YYYY',
	timePickerInput: 'HH:mm',
	monthYearLabel: 'MMM-YYYY',
	dateA11yLabel: 'HH:mm',
	monthYearA11yLabel: 'MMMM-YYYY'
};
export function initializeApp(appConfigService: ApiConfigService) {
	return (): Promise<any> => { 
	  return appConfigService.load();
	}
  }

@NgModule({
	declarations: [
		AppComponent,
		PageNotFoundComponent,
		LoginComponent,
		HeaderComponent,
		LoginHeaderComponent,
		LoginFooterComponent,
		FooterComponent,
		LeftMenuComponent,
		ExtendMeetingComponent,
		DashboardComponent,
		PageSizeRenderComponent,
		PaginationRenderComponent,
		SearchRenderComponent,
		ConfirmDialogComponent,
		DeleteDialogComponent,
		DeviceListComponent,
		ResetpasswordComponent,
		SettingsComponent,
		FlashMessageComponent,
		DeviceAddComponent,
		UserListComponent,
		UserAddComponent,
		RolesListComponent,
		RolesAddComponent,
		MyProfileComponent,
		ChangePasswordComponent,
		HomeComponent,
		RegisterComponent,
		PaymentComponent,
		PurchasePlanComponent,
		ShortNumberPipe,
		ReplacePipe,
		ShortNumberFixnumberPipe,
		AdminLoginComponent,
		AdminDashboardComponent,
		SubscribersListComponent,
		HardwareListComponent,
		HardwareAddComponent,
		BulkuploadAddComponent,
		SubscriberDetailComponent,
		TooltipDirective,
		NotificationListComponent,
		NotificationAddComponent,
		AdminNotificationListComponent,
		AdminNotificationAddComponent,
		MessageDialogComponent,
		ElevatorDetailsComponent,
		BuildingListComponent,
		BuildingAddComponent,
		BuildingDetailsComponent,
		ElevatorListComponent,
		ElevatorAddComponent,
		ScheduledMaintenanceListComponent,
		ScheduledMaintenanceAddComponent,
		AdminUserAddComponent,
		AdminUserListComponent,
		AlertsComponent
	],
	entryComponents: [DeleteDialogComponent, MessageDialogComponent],
	imports: [
		MatSelectModule,
		MatRadioModule,
		MatButtonModule,
		MatCheckboxModule,
		MatTabsModule,
		MatProgressBarModule,
		MatSlideToggleModule,
		MatInputModule,
		MatSidenavModule,
		MatTableModule,
		MatDialogModule,
		MatIconModule,
		MatPaginatorModule,
		MatSortModule,
		MatCardModule,
		MatTooltipModule,
		BrowserModule,
		TagInputModule,
		BrowserAnimationsModule,
		FormsModule,
		ReactiveFormsModule,
		RxReactiveFormsModule,
		AppRoutingModule,
		HttpModule,
		HttpClientModule,
		NgxSpinnerModule,
		NgxPaginationModule,
		OwlDateTimeModule,
		Ng2GoogleChartsModule,
		OwlNativeDateTimeModule,
		FullCalendarModule,
		SocketIoModule.forRoot(config),
		AgmCoreModule.forRoot({ apiKey: '[YOUR GOOGLE MAP API KEY]' }),
		AgmJsMarkerClustererModule,
		AgmDirectionModule,
		TextMaskModule,
		NgScrollbarModule,
		ClickOutsideModule,
		RoundProgressModule,
		MatDatepickerModule,
		MatNativeDateModule,
		ChartsModule,
		NgSelectModule
	],
	providers: [
		ElevatorService,
		AuthService,
		AdminAuthGuired,
		SettingsService,
		CookieService,
		DeviceService,
		RolesService,
		DashboardService,
		ConfigService,
		NotificationService,
		UsersService,
		LookupService,
		RuleService,
		SubscriptionService,
		AppConstant,
		ApiConfigService,
		{
			provide: DateTimeAdapter,
			useClass: MomentDateTimeAdapter,
			deps: [OWL_DATE_TIME_LOCALE]
		}, {
			provide: OWL_DATE_TIME_FORMATS,
			useValue: MY_NATIVE_FORMATS
		}, {
			provide: HTTP_INTERCEPTORS,
			useClass: JwtInterceptor,
			multi: true
		},
		{ provide: APP_INITIALIZER,useFactory: initializeApp, deps: [ApiConfigService], multi: true}
		
	],
	bootstrap: [AppComponent]
})

export class AppModule { }
