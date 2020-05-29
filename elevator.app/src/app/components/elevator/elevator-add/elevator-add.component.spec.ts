import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ElevatorAddComponent } from './elevator-add.component';

describe('ElevatorAddComponent', () => {
  let component: ElevatorAddComponent;
  let fixture: ComponentFixture<ElevatorAddComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ElevatorAddComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ElevatorAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
