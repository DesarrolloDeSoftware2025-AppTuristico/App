import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetalleDestino } from './detalle-destino';

describe('DetalleDestino', () => {
  let component: DetalleDestino;
  let fixture: ComponentFixture<DetalleDestino>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetalleDestino]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetalleDestino);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
