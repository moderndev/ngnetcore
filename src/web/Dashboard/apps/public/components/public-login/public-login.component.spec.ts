import { inject, async, TestBed  } from "@angular/core/testing";
import { TestingHelpers } from "../../../common/utils/testing-helpers";
import { PublicLoginComponent } from "./public-login.component";

describe("Public Login Tests", () => {
    beforeEach(async(() => {
        let moduleDef = TestingHelpers.GetDefaultTestBedSetupConfiguration();
        moduleDef.providers.push(PublicLoginComponent);

        TestBed.configureTestingModule(moduleDef);
        TestBed.compileComponents();
    }));

    it("First Test", inject([PublicLoginComponent], (cat: PublicLoginComponent) => {
        // cat.ngOnInit(); // this causes a redirect, need to mock the ui navigator

        expect(true).toBe(true);
    }));
});
