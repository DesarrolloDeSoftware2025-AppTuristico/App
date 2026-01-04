import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardMetricas } from './dashboard-metricas';

describe('DashboardMetricas', () => {
  let component: DashboardMetricas;
  let fixture: ComponentFixture<DashboardMetricas>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardMetricas]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DashboardMetricas);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
