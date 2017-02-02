import { inject, async, TestBed  } from "@angular/core/testing";
import { TestingHelpers } from "../../common/utils/testing-helpers";
import { PublicAppComponent } from "./app.component";

describe("Public App Tests", () => {
    beforeEach(async(() => {
        let moduleDef = TestingHelpers.GetDefaultTestBedSetupConfiguration();
        moduleDef.providers.push(PublicAppComponent);

        TestBed.configureTestingModule(moduleDef);
        TestBed.compileComponents();
    }));

    it("First Test", inject([PublicAppComponent], (cat: PublicAppComponent) => {
        cat.ngOnInit();

        expect(true).toBe(true);
    }));
});
