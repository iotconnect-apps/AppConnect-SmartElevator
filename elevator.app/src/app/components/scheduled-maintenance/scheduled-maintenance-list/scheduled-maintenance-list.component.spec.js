"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var scheduled_maintenance_list_component_1 = require("./scheduled-maintenance-list.component");
describe('ScheduledMaintenanceListComponent', function () {
    var component;
    var fixture;
    beforeEach(testing_1.async(function () {
        testing_1.TestBed.configureTestingModule({
            declarations: [scheduled_maintenance_list_component_1.ScheduledMaintenanceListComponent]
        })
            .compileComponents();
    }));
    beforeEach(function () {
        fixture = testing_1.TestBed.createComponent(scheduled_maintenance_list_component_1.ScheduledMaintenanceListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
});
//# sourceMappingURL=scheduled-maintenance-list.component.spec.js.map