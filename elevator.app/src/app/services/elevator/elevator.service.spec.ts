import { TestBed } from '@angular/core/testing';

import { ElevatorService } from './elevator.service';

describe('ElevatorService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ElevatorService = TestBed.get(ElevatorService);
    expect(service).toBeTruthy();
  });
});
