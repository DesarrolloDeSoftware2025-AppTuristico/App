import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DestinoTuristicoService, DestinoTuristicoDto } from '../../proxy/destinos-turisticos';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-destinos-populares',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule], 
  templateUrl: './destinos-populares.html'
})
export class DestinosPopulares implements OnInit {
  populares: DestinoTuristicoDto[] = [];
  loading = true;
  
  // 3. Nueva variable para el input
  cantidad: number = 10; 

  constructor(
    private destinoService: DestinoTuristicoService,
    private cd: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.cargarPopulares();
  }

  cargarPopulares() {
    // Validación simple para no enviar negativos o cero
    if (this.cantidad < 1) this.cantidad = 1;

    this.loading = true;
    
    // 4. Usamos la variable 'this.cantidad' en lugar del 10 fijo
    this.destinoService.listarDestinosPopulares(this.cantidad)
      .pipe(
        finalize(() => {
            this.loading = false;
            this.cd.detectChanges(); 
        })
      )
      .subscribe({
        next: (res) => {
          this.populares = res;
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error("Error cargando populares:", err);
        }
      });
  }
}