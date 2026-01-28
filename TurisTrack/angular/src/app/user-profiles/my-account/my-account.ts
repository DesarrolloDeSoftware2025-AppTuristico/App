import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserProfileService } from '../../proxy/user-profiles';
import { ConfirmationService, ToasterService } from '@abp/ng.theme.shared';
import { AuthService } from '@abp/ng.core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-my-account',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-account.html',
  styleUrls: ['./my-account.scss']
})
export class MyAccountComponent {

  constructor(
    private userProfileService: UserProfileService,
    private confirmation: ConfirmationService,
    private toaster: ToasterService,
    private authService: AuthService,
    private router: Router
  ) {}

  eliminarCuenta() {
    this.confirmation.warn(
      'Esta acción es irreversible. ¿Estás seguro de que quieres eliminar tu cuenta?',
      'Eliminar Cuenta Permanentemente'
    ).subscribe((status) => {
      if (status === 'confirm') {
        this.procesarEliminacion();
      }
    });
  }

  private procesarEliminacion() {
    this.userProfileService.deleteMyAccount().subscribe({
      next: () => {
        this.toaster.success('Tu cuenta ha sido eliminada correctamente.');
        // Importante: Cerrar sesión localmente y redirigir al home
        this.authService.logout().subscribe(() => {
             this.router.navigate(['/']);
        });
      },
      error: (err) => {
        this.toaster.error('Ocurrió un error al intentar eliminar la cuenta.');
        console.error(err);
      }
    });
  }
}