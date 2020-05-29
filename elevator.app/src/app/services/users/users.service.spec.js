"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var users_service_1 = require("./users.service");
describe('UsersService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(users_service_1.UsersService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=users.service.spec.js.map