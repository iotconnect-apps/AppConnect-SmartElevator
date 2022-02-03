import { ChangeDetectorRef, ViewRef , OnInit, Component, Input, Output, EventEmitter, ViewChild, OnDestroy } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner'
import { Notification, NotificationService, DashboardService } from 'app/services';
import {Subscription} from 'rxjs/Subscription';
import { ChartReadyEvent, GoogleChartComponent } from 'ng2-google-charts'

@Component({
	selector: 'app-widget-chart-a',
	templateUrl: './widget-chart-a.component.html',
	styleUrls: ['./widget-chart-a.component.css']
})
export class WidgetChartAComponent implements OnInit,OnDestroy {
	@Input() widget;
	@Input() gridster;
	@Input() count;
	@Input() resizeEvent: EventEmitter<any>;
	resizeSub: Subscription;
	@Input() chartTypeChangeEvent: EventEmitter<any>;
	chartTypeChangeSub: Subscription;
	

	@ViewChild('cchart', { static: false }) cchart: GoogleChartComponent;
	currentUser = JSON.parse(localStorage.getItem("currentUser"));
	comboChart = {
		chartType: 'ComboChart',
		dataTable: [],
		options: {
			width:400,
			height:400,
			chartArea: {right: 0,left: 100},
			title: "",
			vAxis: {
				title: "Hours && KW",
				titleTextStyle: {
					bold: true
				},
				viewWindow: {
					min: 0
				}
			},
			hAxis: {
				titleTextStyle: {
					bold: true
				},
			},
			legend: { position: "bottom", alignment: "start" },
			seriesType : 'bars',
			series: { 0: { type: 'bar' },1: { type: 'line' } },
			colors : ["#3366cc","#dc3912"]
		}
	};
	buildings = [];
	buildingsDetail = {}
	type: any = "d";
	constructor(
		public dashboardService: DashboardService,
		private spinner: NgxSpinnerService,
		private _notificationService: NotificationService,
		private changeDetector : ChangeDetectorRef,
		){
	}

	ngOnInit() {
		if(this.widget.widgetProperty.chartColor.length > 0){
			this.comboChart.options.colors = [];
			for (var i = 0; i <= (this.widget.widgetProperty.chartColor.length - 1); i++) {
				this.comboChart.options.colors.push(this.widget.widgetProperty.chartColor[i].color);
			}
		}
		this.comboChart.options.width = (this.widget.properties.w > 0 ? parseInt((this.widget.properties.w - 200).toString()) : 400);
		this.comboChart.options.height = (this.widget.properties.h > 0 ? parseInt((this.widget.properties.h / 2).toString()) : 400);
		this.resizeSub = this.resizeEvent.subscribe((widget) => {
			if(widget.id == this.widget.id){
				this.widget = widget;
				this.changeChartType();
			}
		});

		this.chartTypeChangeSub = this.chartTypeChangeEvent.subscribe((widget) => {
			if(widget.id == this.widget.id){
				this.changeChartType();
			}
		});
		this.changeChartType();
		this.getBuildingList();
	}

	getBuildingList() {
		this.spinner.show();
		let companyId = this.currentUser.userDetail.companyId;
		this.dashboardService.getBuildingLookup(companyId).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this.buildings = response.data;
				if (this.buildings.length) {
					this.getBuildingOverview(this.buildings[0]['value'],this.type);
          			this.getBuildingGraph(this.buildings[0]['value'],'',this.type);
				}
			}
			else {
				this._notificationService.handleResponse(response,"error");
			}
		}, error => {
			this.spinner.hide();
			this._notificationService.handleResponse(error,"error");
		});
	}

	getBuildingGraph(buildingId, elevatorId = '', type) {
		this.spinner.show();
		this.comboChart.dataTable = []
		this.comboChart.dataTable.push(["WeekDays", "Operating Hours", "Energy Consumption"]);
		
		this.dashboardService.getBuildingGraph(buildingId, type).subscribe(response => {
			if (response.isSuccess === true) {
				response.data.forEach((element,index) => {
					this.comboChart.dataTable.push([element.name, parseFloat(element.operatingHours), parseFloat(element.energyConsumption)]);
					if(index == (response.data.length - 1)){
						this.changeChartType();
					}
				});
				if (this.changeDetector && !(this.changeDetector as ViewRef).destroyed) {
					this.changeDetector.detectChanges();
				}
			}
			else {
				this._notificationService.handleResponse(response,"error");
			}
			this.spinner.hide();
		}, error => {
			this.spinner.hide();
			this._notificationService.handleResponse(error,"error");
		});
	}

	getBuildingOverview(buildingId, type) {
		this.spinner.show();
		this.dashboardService.getBuildingOverview(buildingId, type).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this.buildingsDetail['totalElevator'] = (response.data['totalElevator']) ? response.data['totalElevator'] : 0;
				this.buildingsDetail['totalOperatingHours'] = (response.data['totalOperatingHours']) ? response.data['totalOperatingHours'] : 0;
				this.buildingsDetail['totalTrips'] = (response.data['totalTrips']) ? response.data['totalTrips'] : 0;
				this.buildingsDetail['totalEnergy'] = (response.data['totalEnergy']) ? response.data['totalEnergy'] : 0;
			}
			else {
				this.buildingsDetail['totalElevator'] = 0;
				this.buildingsDetail['totalOperatingHours'] = 0;
				this.buildingsDetail['totalTrips'] = 0;
				this.buildingsDetail['totalEnergy'] = 0;
				this._notificationService.handleResponse(response,"error");

			}
		}, error => {
			this.buildingsDetail['totalElevator'] = 0;
			this.buildingsDetail['totalOperatingHours'] = 0;
			this.buildingsDetail['totalTrips'] = 0;
			this.buildingsDetail['totalEnergy'] = 0;
			this.spinner.hide();
			this._notificationService.handleResponse(error,"error");
		});
	}

	changeChartType(){
		if(this.widget.widgetProperty.chartColor.length > 0){
			this.comboChart.options.colors = [];
			for (var i = 0; i <= (this.widget.widgetProperty.chartColor.length - 1); i++) {
				this.comboChart.options.colors.push(this.widget.widgetProperty.chartColor[i].color);
			}
		}
		this.comboChart.options.width = (this.widget.properties.w > 0 ? parseInt((this.widget.properties.w - 200).toString()) : 400);
		this.comboChart.options.height = (this.widget.properties.h > 0 ? parseInt((this.widget.properties.h / 2).toString()) : 400);
		if(this.widget.widgetProperty.chartTypeHours && this.widget.widgetProperty.chartTypeHours != '' && this.widget.widgetProperty.chartTypeEnergy && this.widget.widgetProperty.chartTypeEnergy != ''){
			let chartTypeHours = (this.widget.widgetProperty.chartTypeHours == 'bar' ? 'bar' : 'line');
			let chartTypeEnergy = (this.widget.widgetProperty.chartTypeEnergy == 'bar' ? 'bar' : 'line');
			this.comboChart.options.series = {0 : { type: chartTypeHours }, 1: { type: chartTypeEnergy } };
			if(this.comboChart.dataTable.length > 1 && this.cchart){
				let ccWrapper = this.cchart.wrapper;
				this.cchart.draw();
				ccWrapper.draw();
			}
			if (this.changeDetector && !(this.changeDetector as ViewRef).destroyed) {
				this.changeDetector.detectChanges();
			}
		}
	}

	onTabChange(event) {
		this.getBuildingOverview(this.buildings[event.index]['value'],this.type);
		this.getBuildingGraph(this.buildings[event.index]['value'],'',this.type);
	}

	changeGraphFilter(buiding, event) {
		let type = 'd';
		if (event.value === 'Week') {
			type = 'w';
		} else if (event.value === 'Month') {
			type = 'm';
		}
		this.type = type;
		this.getBuildingOverview(buiding['value'], type);
		this.getBuildingGraph(buiding['value'], '', type);
	}

	ngOnDestroy() {
		this.resizeSub.unsubscribe();
		this.chartTypeChangeSub.unsubscribe();
	}
}
