"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var scheduled_maintenance_service_1 = require("./scheduled-maintenance.service");
describe('ScheduledMaintenanceService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(scheduled_maintenance_service_1.ScheduledMaintenanceService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=scheduled-maintenance.service.spec.js.map