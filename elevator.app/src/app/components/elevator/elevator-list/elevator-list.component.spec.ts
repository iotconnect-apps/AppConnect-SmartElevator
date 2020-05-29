import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ElevatorListComponent } from './elevator-list.component';

describe('ElevatorListComponent', () => {
  let component: ElevatorListComponent;
  let fixture: ComponentFixture<ElevatorListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ElevatorListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ElevatorListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
