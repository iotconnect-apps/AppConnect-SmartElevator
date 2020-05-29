import { TestBed } from '@angular/core/testing';

import { ScheduledMaintenanceService } from './scheduled-maintenance.service';

describe('ScheduledMaintenanceService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ScheduledMaintenanceService = TestBed.get(ScheduledMaintenanceService);
    expect(service).toBeTruthy();
  });
});
