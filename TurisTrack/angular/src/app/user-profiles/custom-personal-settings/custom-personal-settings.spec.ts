import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomPersonalSettings } from './custom-personal-settings';

describe('CustomPersonalSettings', () => {
  let component: CustomPersonalSettings;
  let fixture: ComponentFixture<CustomPersonalSettings>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomPersonalSettings]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CustomPersonalSettings);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
