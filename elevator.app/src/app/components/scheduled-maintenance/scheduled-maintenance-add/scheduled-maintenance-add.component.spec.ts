import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduledMaintenanceAddComponent } from './scheduled-maintenance-add.component';

describe('ScheduledMaintenanceAddComponent', () => {
  let component: ScheduledMaintenanceAddComponent;
  let fixture: ComponentFixture<ScheduledMaintenanceAddComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ScheduledMaintenanceAddComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScheduledMaintenanceAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
