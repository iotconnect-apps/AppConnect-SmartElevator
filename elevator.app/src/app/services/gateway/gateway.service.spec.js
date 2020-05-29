"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var gateway_service_1 = require("./gateway.service");
describe('GatewayService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(gateway_service_1.GatewayService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=gateway.service.spec.js.map