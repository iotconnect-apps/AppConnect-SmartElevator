import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ElevatorDetailsComponent } from './elevator-details.component';

describe('RolesDetailsComponent', () => {
  let component: ElevatorDetailsComponent;
  let fixture: ComponentFixture<ElevatorDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ElevatorDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ElevatorDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
