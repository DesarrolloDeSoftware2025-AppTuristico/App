import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ToasterService } from '@abp/ng.theme.shared';
import { DestinoTuristicoService, DestinoTuristicoDto } from '../../proxy/destinos-turisticos';

@Component({
  selector: 'app-detalle-destino',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './detalle-destino.html'
})
export class DetalleDestino implements OnInit {
  destino: DestinoTuristicoDto | null = null;
  loading = true;
  guardando = false;

  constructor(
    private route: ActivatedRoute,
    private destinoService: DestinoTuristicoService,
    private toaster: ToasterService
  ) {}

  ngOnInit(): void {
    // 1. Capturamos el ID de la URL
    const idApi = Number(this.route.snapshot.params['id']);
    
    if (idApi) {
      this.cargarDetalle(idApi);
    } else {
        this.loading = false;
        this.toaster.error("ID de destino inválido");
    }
  }

  cargarDetalle(id: number) {
    // 2. Llamamos al método 3.3 (Obtener Por ID)
    this.destinoService.obtenerDestinoPorId(id).subscribe({
      next: (res) => {
        this.destino = res;
        this.loading = false;
      },
      error: (err) => {
        this.toaster.error('Error al cargar el detalle del destino.');
        this.loading = false;
      }
    });
  }

  guardarEnBaseDeDatos() {
    if (!this.destino) return; 

    this.guardando = true; 

    this.destinoService.guardarDestino(this.destino).subscribe({
      next: (res) => {
        this.guardando = false; 

        this.destino!.estaGuardado = true; 
        
        this.toaster.success('¡Destino guardado correctamente!');
      },
      error: (err) => {
        this.guardando = false;

        // Si el backend dice que ya existe, lo marcamos como guardado visualmente también
        if (err.error && err.error.code === 'TurisTrack:DestinoDuplicado') {
             this.toaster.info('Este destino ya lo tenías guardado.');
             this.destino!.estaGuardado = true; 
        } else {
             this.toaster.error('Error al intentar guardar.');
             console.error(err);
        }
      }
    });
  }
}