"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var elevator_service_1 = require("./elevator.service");
describe('ElevatorService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(elevator_service_1.ElevatorService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=elevator.service.spec.js.map