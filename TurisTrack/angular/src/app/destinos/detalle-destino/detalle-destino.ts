import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ToasterService, ConfirmationService } from '@abp/ng.theme.shared'; 
import { AuthService, ConfigStateService } from '@abp/ng.core';

import { DestinoTuristicoService, DestinoTuristicoDto } from '../../proxy/destinos-turisticos';
import { CalificacionDestinoService, CalificacionDestinoDto, ResumenCalificacionDto } from '../../proxy/calificaciones-destinos';

@Component({
  selector: 'app-detalle-destino',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './detalle-destino.html',
  styleUrls: ['./detalle-destino.scss']
})
export class DetalleDestino implements OnInit {
  destino: DestinoTuristicoDto | null = null;
  loading = true;
  guardando = false;
  
  comentarios: CalificacionDestinoDto[] = [];
  resumen: ResumenCalificacionDto = { promedio: 0, totalCalificaciones: 0 };
  
  nuevaPuntuacion: number = 0;
  nuevoComentario: string = '';
  enviandoCalificacion = false;
  
  isLoggedIn = false;
  currentUserId: string | null = null;
  usuarioYaComento = false;

  comentarioEnEdicion: CalificacionDestinoDto | null = null; 
  editPuntuacion: number = 0;
  editComentario: string = '';
  editando = false; 

  constructor(
    private route: ActivatedRoute,
    private destinoService: DestinoTuristicoService,
    private calificacionService: CalificacionDestinoService,
    private toaster: ToasterService,
    private authService: AuthService,
    private configState: ConfigStateService,
    private cd: ChangeDetectorRef,
    private confirmation: ConfirmationService 
  ) {}

  ngOnInit(): void {
    this.isLoggedIn = this.authService.isAuthenticated;
    
    const currentUser = this.configState.getOne('currentUser');
    this.currentUserId = currentUser?.id || null;

    const idApi = Number(this.route.snapshot.params['id']);
    
    if (idApi) {
      this.cargarDetalle(idApi);
    } else {
       this.loading = false;
       this.toaster.error("ID de destino inválido");
    }
  }

  cargarDetalle(id: number) {
    this.destinoService.obtenerDestinoPorId(id).subscribe({
      next: (res) => {
        this.destino = res;
        this.loading = false;
        
        if (this.destino.estaGuardado && this.destino.id) {
             this.cargarDatosCalificaciones(this.destino.id);
        }
      },
      error: (err) => {
        this.toaster.error('Error al cargar el detalle.');
        this.loading = false;
      }
    });
  }

  cargarDatosCalificaciones(destinoId: string) {
    this.calificacionService.obtenerPromedioDestino(destinoId).subscribe({
        next: (res: any) => {
             this.resumen = {
                promedio: res.promedio || res.Promedio || 0,
                totalCalificaciones: res.totalCalificaciones || res.TotalCalificaciones || 0
            };
            this.cd.detectChanges();
        },
        error: (err) => console.error(err)
    });

    this.calificacionService.obtenerComentariosPorDestino(destinoId).subscribe(res => {
        
        if (this.isLoggedIn && this.currentUserId) {
            
            this.usuarioYaComento = res.some(c => c.userId === this.currentUserId);

            const miIndice = res.findIndex(c => c.userId === this.currentUserId);

            if (miIndice > 0) {
                const miComentario = res.splice(miIndice, 1)[0];
                res.unshift(miComentario);
            }

        } else {
            this.usuarioYaComento = false;
        }

        this.comentarios = res;
        this.cd.detectChanges();
    });
  }

  guardarEnBaseDeDatos() {
    if (!this.destino) return; 
    this.guardando = true; 

    this.destinoService.guardarDestino(this.destino).subscribe({
      next: (res: any) => {
        this.guardando = false; 
        
        if (res.idInterno) {
            this.destino!.id = res.idInterno;
        }
        this.destino!.estaGuardado = true; 
        this.toaster.success('¡Destino guardado correctamente!');
        
        if (this.destino!.id) this.cargarDatosCalificaciones(this.destino!.id);
      },
      error: (err) => {
        this.guardando = false;
        if (err.error && err.error.code === 'TurisTrack:DestinoDuplicado') {
             this.toaster.info('Este destino ya lo tenías guardado.');
             this.destino!.estaGuardado = true; 
             if (this.destino!.id) this.cargarDatosCalificaciones(this.destino!.id);
        } else {
             this.toaster.error('Error al intentar guardar.');
        }
      }
    });
  }

  seleccionarEstrella(stars: number) { this.nuevaPuntuacion = stars; }

  enviarCalificacion() {
    if (this.nuevaPuntuacion === 0) {
        this.toaster.warn('Selecciona las estrellas.');
        return;
    }
    this.enviandoCalificacion = true;

    this.calificacionService.crearCalificacion(
        this.destino!.id, 
        this.nuevaPuntuacion, 
        this.nuevoComentario
    ).subscribe({
        next: () => {
            this.toaster.success('¡Gracias por tu opinión!');
            this.enviandoCalificacion = false;
            this.nuevoComentario = '';
            this.nuevaPuntuacion = 0;
            
            this.usuarioYaComento = true; 
            this.cargarDatosCalificaciones(this.destino!.id);
        },
        error: (err) => {
            this.enviandoCalificacion = false;
            this.toaster.error(err.message || 'Error al enviar');
        }
    });
  }

  abrirModalEdicion(item: CalificacionDestinoDto) {
    this.comentarioEnEdicion = item;
    this.editPuntuacion = item.puntuacion;
    this.editComentario = item.comentario;
  }

  cerrarModalEdicion() {
    this.comentarioEnEdicion = null;
    this.editPuntuacion = 0;
    this.editComentario = '';
  }

  seleccionarEstrellaEdit(stars: number) { this.editPuntuacion = stars; }

  guardarEdicion() {
    if (!this.comentarioEnEdicion) return;
    this.editando = true;

    this.calificacionService.editarCalificacion(
        this.comentarioEnEdicion.id,
        this.editPuntuacion,
        this.editComentario
    ).subscribe({
        next: () => {
            this.toaster.success('Opinión actualizada');
            this.editando = false;
            this.cerrarModalEdicion();
            this.cargarDatosCalificaciones(this.destino!.id);
        },
        error: (err) => {
            this.editando = false;
            this.toaster.error(err.message || 'Error al editar');
        }
    });
  }

  eliminarComentario(calificacionId: string) {
    this.confirmation.warn(
        '¿Estás seguro de eliminar tu reseña? Podrás escribir una nueva luego.',
        'Eliminar Opinión'
    ).subscribe((status) => {
        if (status === 'confirm') {
            this.calificacionService.eliminarCalificacion(calificacionId).subscribe({
                next: () => {
                    this.toaster.success('Comentario eliminado');
                    this.usuarioYaComento = false;
                    this.cargarDatosCalificaciones(this.destino!.id);
                },
                error: (err) => this.toaster.error(err.message || 'Error al eliminar')
            });
        }
    });
  }

  mathRound(num: number): number { return Math.round(num); }
}