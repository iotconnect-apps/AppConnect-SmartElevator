"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var rule_service_1 = require("./rule.service");
describe('RuleService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(rule_service_1.RuleService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=rule.service.spec.js.map