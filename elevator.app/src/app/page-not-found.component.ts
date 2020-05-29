import { Component, OnInit } from '@angular/core';
import { AppComponent } from './app.component';


@Component({
    selector: 'app-404-page',
	templateUrl: './page-not-found.component.html',
})
export class PageNotFoundComponent implements OnInit {
    currentUser = JSON.parse(localStorage.getItem("currentUser"));
	isAdmin = false;
    constructor(){
    }

    ngOnInit() {
        this.isAdmin = this.currentUser.userDetail.isAdmin;
	}

    ngAfterViewInit(){
    }
}
