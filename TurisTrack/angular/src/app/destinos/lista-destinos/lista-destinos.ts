import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { DestinoTuristicoService } from '../../proxy/destinos-turisticos/destino-turistico.service';
import { DestinoTuristicoDto } from '../../proxy/destinos-turisticos/models';
import { finalize } from 'rxjs/operators';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-destinations-list',
  standalone: true,
  imports: [CommonModule, FormsModule, CoreModule, NgbPaginationModule],
  templateUrl: './lista-destinos.html',
  styleUrls: ['./lista-destinos.scss'],
})
export class ListaDestinos implements OnInit {
  private readonly destinationService = inject(DestinoTuristicoService);

  /**
   * Lista TOTAL de destinos recibidos del backend
   */
  destinations: DestinoTuristicoDto[] = [];

  /**
   * Lista visible (paginada localmente)
   */
  pagedDestinations: DestinoTuristicoDto[] = [];

  loading = false;

  /**
   * Parámetros de búsqueda
   */
  searchParams = {
    query: '',
    country: '',
  };

  /**
   * Para paginación local
   */
  totalCount = 0;
  currentPage = 1;
  pageSize = 10;

  readonly defaultImage = 'assets/images/destination-placeholder.svg';

  ngOnInit(): void {
    this.loadDestinations();
  }

  /**
   * Llama al backend (que devuelve una LISTA SIMPLE)
   */
  private loadDestinations(): void {
    this.loading = true;

    this.destinationService
      .buscarDestinos(
        this.searchParams.query,
        this.searchParams.country,
        undefined, // region (no usada)
        undefined  // poblacionMinima (no usada)
      )
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (result: DestinoTuristicoDto[]) => {
          // Guardamos lista completa
          this.destinations = result;

          // Actualizamos conteo total
          this.totalCount = result.length;

          // Calculamos la paginación local
          this.applyPagination();
        },
        error: (error) => {
          console.error('Error al cargar destinos:', error);
          this.destinations = [];
          this.pagedDestinations = [];
          this.totalCount = 0;
        },
      });
  }

  /**
   * Paginación LOCAL
   */
  private applyPagination(): void {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;

    this.pagedDestinations = this.destinations.slice(startIndex, endIndex);
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadDestinations();
  }

  clearSearch(): void {
    this.searchParams.query = '';
    this.searchParams.country = '';
    this.onSearch();
  }

  onImageError(event: any): void {
    event.target.src = this.defaultImage;
  }

  formatCoordinates(latitude: number, longitude: number): string {
    return `${latitude.toFixed(4)}, ${longitude.toFixed(4)}`;
  }

  formatPopulation(population?: number): string {
    if (!population) return 'N/A';
    return population.toLocaleString('es-ES');
  }

  openInMaps(destination: DestinoTuristicoDto): void {
    const url = `https://www.google.com/maps/search/?api=1&query=${destination.latitud},${destination.longitud}`;
    window.open(url, '_blank');
  }

  /**
   * Cambia de página (pag. local)
   */
  onPageChange(page: number): void {
    this.currentPage = page;
    this.applyPagination();
  }

  getDestinationImage(imageUrl: string): string {
    return imageUrl ? environment.apis.default.url + imageUrl : this.defaultImage;
  }
}
