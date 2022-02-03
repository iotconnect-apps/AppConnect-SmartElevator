import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DeviceService, NotificationService, Notification } from 'app/services';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-subscriber-detail',
  templateUrl: './subscriber-detail.component.html',
  styleUrls: ['./subscriber-detail.component.css']
})
export class SubscriberDetailComponent implements OnInit {
 
  changeStatusDeviceName:any;
  changeStatusDeviceStatus:any;
  changeDeviceStatus:any;
  params = {};
  searchParameters = {
    pageNo: 0,
    pageSize: 5,
    searchText: '',
    sortBy: 'kitTypeName asc'
  };
  pageSizeOptions: number[] = [5, 10, 25, 100];
  isSearch = false;
  SubscriberDetail = {};
  SubscriberKitList = [];
  totalKits = 0;
  displayedColumns: string[] = ['kitTypeName', 'kitCode'];

  white = true;
  assig = false;
  fieldshow = false;
  displayName: any;

  constructor(
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    private deviceService: DeviceService,
    private _notificationService: NotificationService,
    private router: Router
  ) {
    this.activatedRoute.params.subscribe(params => {
      if (params.email) {
        this.params = params
      }

    })
  }

  ngOnInit() {
    this.getSubscriberDetail();
    //this.getSubscriberKitList();
    this.getSubscriberSync();
  }

  /**
	* For Manage Tabs
	**/
  clickassigned(event) {
    if (event.tab.textLabel == 'Assigned') {
      this.fieldshow = true;
      this.assig = true;
      this.white = false;
      //this.searchParameters.isAssigned = true;
    } else {
      this.fieldshow = false;
      this.assig = false;
      this.white = true;
      //this.searchParameters.isAssigned = false;
    }
  }

  /**
	* For set order of subscriber list
	**/
  setOrder(sort: any) {
    if (!sort.active || sort.direction === '') {
      return;
    }
    this.searchParameters.sortBy = sort.active + ' ' + sort.direction;
    this.getSubscriberKitList();
  }

  /**
	* For search text call back
	**/
  searchTextCallback(filterText) {
    this.searchParameters.searchText = filterText;
    this.searchParameters.pageNo = 0;
    this.isSearch = true;
    this.getSubscriberKitList();
  }

  /**
	* For manage paggination
	**/
  ChangePaginationAsPageChange(pagechangeresponse) {
    this.searchParameters.pageNo = pagechangeresponse.pageIndex;
    this.searchParameters.pageSize = pagechangeresponse.pageSize;
    this.isSearch = true;
    this.getSubscriberKitList();
  }

  /**
	* For Get Subscriber Sync
	**/
  getSubscriberSync() {
    this.spinner.show();
    this.deviceService.getsubcribesyncdata().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
       this.displayName = response.data.displayName
      }
    })
  }
  

  /**
	* For Get Subscriber Detail
	**/
  getSubscriberDetail() {
    this.spinner.show();
    this.deviceService.getsubscriberDetail(this.params).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.SubscriberDetail = response.data;
      }
      else {
        this._notificationService.handleResponse(response,"error");
        this.router.navigateByUrl['/admin/subscriber'];
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.handleResponse(error,"error");
      this.router.navigateByUrl['/admin/subscriber'];
    });
  }

  /**
	* For Get Subscriber kit list
	**/
  getSubscriberKitList() {
    this.spinner.show();
    this.deviceService.getSubscriberKitList(this.searchParameters).subscribe(response => {
      if (this.isSearch) {
        this.spinner.hide();
      }
      if (response.isSuccess === true) {
        this.SubscriberKitList = response.data.items;
        this.totalKits = response.data.count;
      }
      else {
        this._notificationService.handleResponse(response,"error");
        this.SubscriberKitList = [];
      }
    }, error => {
      this.SubscriberKitList = [];
      this.spinner.hide();
      this._notificationService.handleResponse(error,"error");
    });
  }

}
