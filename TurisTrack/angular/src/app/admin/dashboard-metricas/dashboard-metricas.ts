import { Component, OnInit } from '@angular/core';
import { AdminMetricasService, ApiMetricaResumenDto } from '../../proxy/metricas';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard-metricas',
  templateUrl: './dashboard-metricas.html',
  styleUrls: ['./dashboard-metricas.scss'],
  standalone: true,       
  imports: [CommonModule]
})
export class DashboardMetricas implements OnInit {
  metricas: ApiMetricaResumenDto | null = null;
  loading = false;

  constructor(private adminMetricasService: AdminMetricasService) {}

  ngOnInit(): void {
    this.cargarMetricas();
  }

  cargarMetricas() {
    this.loading = true;
    this.adminMetricasService.obtenerMetricasUso().subscribe({
      next: (res) => {
        console.log('Métricas cargadas', res);
        this.metricas = res;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error cargando métricas', err);
        this.loading = false;
      }
    });
  }
}