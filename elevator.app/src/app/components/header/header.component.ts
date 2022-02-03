import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef } from '@angular/core'
import { TitleCasePipe } from '@angular/common'
import { Router, NavigationEnd } from '@angular/router';

import { UserService, AuthService, RuleService } from '../../services/index';
import { StompService, StompRService } from '@stomp/ng2-stompjs';
// import { ConfigService } from './../../services/index'
import { Message } from "@stomp/stompjs";
import { Subscription } from "rxjs";
import { Observable } from "rxjs/Observable";
import * as moment from "moment";
import { AppConstant } from '../../app.constants';
import { IDSAuthService } from 'app/services/auth/idsauth.service';
@Component({
	selector: 'app-header',
	templateUrl: './header.component.html',
	styleUrls: ['./header.css'],
	providers: [TitleCasePipe, StompRService]
})

export class HeaderComponent implements OnInit {
	respondShow: boolean = false;
	cookieName = 'FM';
	userName;
	isMenuOpen: boolean = false;
	isAdmin = false;
	subscription: Subscription;
	messages: Observable<Message>;
	currentUser = JSON.parse(localStorage.getItem("currentUser"));
	cpId = "";
	alerts = [];
	unReadAlerts = 0;
	stompConfiguration = {
		url: "",
		headers: {
			login: "",
			passcode: "",
			host: ""
		},
		heartbeat_in: 0,
		heartbeat_out: 2000,
		reconnect_delay: 5000,
		debug: true
	};
	subscribed;
	@ViewChild('showDropdown', { static: false }) public elementRef: ElementRef;

	constructor(
		public router: Router,
		private cd: ChangeDetectorRef,
		private authService: AuthService,
		private ruleService: RuleService,
    	private stompService: StompRService,
		public _appConstant: AppConstant,
		private IdsService: IDSAuthService
	) {

		router.events.subscribe((val) => {
			if (val instanceof NavigationEnd) {
				if (this.elementRef)
					this.elementRef.nativeElement.classList.remove("show");
			}
		});

	}

	ngOnInit() {
		this.userName = this.currentUser.userDetail.fullName;
		//this.getStompConfig();
		this.isAdmin = this.currentUser.userDetail.isAdmin;
		this._appConstant.username = this.userName;
	}


	/**
    * For get stomp config
	**/
	getStompConfig() {
		if (this.currentUser.userDetail.cpId) {
			this.ruleService.getStompConfig("UIAlert").subscribe(response => {
				if (response.isSuccess) {
					if (response.data.url) {
						let cpId = this.currentUser.userDetail.cpId;
						this.stompConfiguration.url = response.data.url;
						this.stompConfiguration.headers.login = response.data.user;
						this.stompConfiguration.headers.passcode = response.data.password;
						this.stompConfiguration.headers.host = response.data.vhost;
						this.cpId = cpId.toLowerCase();
						if (cpId) {
							this.initStomp();
						}
					}
				}
			});
		}
	}

	/**
    * For init stomp
	**/
	initStomp() {
		let config = this.stompConfiguration;
		this.stompService.config = config;
		this.stompService.initAndConnect();
		this.stompSubscribe();
	}


	/**
    * For subcribe stomp
	**/
	public stompSubscribe() {
		if (this.subscribed) {
			return;
		}

		this.messages = this.stompService.subscribe("/topic/" + this.cpId);
		this.subscription = this.messages.subscribe(this.on_next);
		this.subscribed = true;
	}


	public on_next = (message: Message) => {
		let user_id = this.currentUser.userDetail.id;
		let obj: any = JSON.parse(message.body);
		let users = obj.data.data.users;
		if (users.length && user_id) {
			let isAlert = users.find(o => o.guid === user_id);
			if (isAlert) {
				let now = moment(obj.data.data.time).format("MMM DD, YYYY h:mm:ss A");
				this.alerts.unshift({
					time: now,
					message: obj.data.data.data.message,
					severity: obj.data.data.severity.toLowerCase()
				});
				this.unReadAlerts++;
			}
		}
	};

	/**
    * For logout current user
	**/
	logout() {
		this.authService.logout();
		if (this.currentUser.userDetail.isAdmin) {
			this.router.navigate(['/admin'])
		} else {
			this.IdsService.endSignoutMainWindow();
			this.router.navigate(['/login'])
		}
	}

	onClickedOutside(e) {
		if (e.path[0].className == "dropdown-toggle" || e.path[1].className == "dropdown-toggle" || e.path[2].className == "dropdown-toggle") {
			return false;
		}
		this.isMenuOpen = false;
	}
	Respond() {
		this.respondShow = !this.respondShow;
		if (!this.respondShow) {
			this.alerts = [];
			this.unReadAlerts = 0;
		}
	}
}
