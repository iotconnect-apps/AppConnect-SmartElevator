import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduledMaintenanceListComponent } from './scheduled-maintenance-list.component';

describe('ScheduledMaintenanceListComponent', () => {
  let component: ScheduledMaintenanceListComponent;
  let fixture: ComponentFixture<ScheduledMaintenanceListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ScheduledMaintenanceListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScheduledMaintenanceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
