"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var scheduled_maintenance_add_component_1 = require("./scheduled-maintenance-add.component");
describe('ScheduledMaintenanceAddComponent', function () {
    var component;
    var fixture;
    beforeEach(testing_1.async(function () {
        testing_1.TestBed.configureTestingModule({
            declarations: [scheduled_maintenance_add_component_1.ScheduledMaintenanceAddComponent]
        })
            .compileComponents();
    }));
    beforeEach(function () {
        fixture = testing_1.TestBed.createComponent(scheduled_maintenance_add_component_1.ScheduledMaintenanceAddComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
});
//# sourceMappingURL=scheduled-maintenance-add.component.spec.js.map