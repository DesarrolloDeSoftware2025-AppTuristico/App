import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // Necesario para [(ngModel)]
import { UserProfileService, PublicUserProfileDto } from '../../proxy/user-profiles';
import { RouterModule } from '@angular/router'; // Para poder hacer click e ir al perfil

@Component({
  selector: 'app-user-search',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule], // Importar módulos clave
  templateUrl: './user-search.html'
})
export class UserSearchComponent {
  searchText = '';
  users: PublicUserProfileDto[] = [];
  loading = false;
  hasSearched = false; // Para saber si mostrar "No se encontraron resultados"

  constructor(private userProfileService: UserProfileService) {}

  buscar() {
    if (!this.searchText.trim()) return;

    this.loading = true;
    this.hasSearched = true;

    this.userProfileService.searchUsers(this.searchText).subscribe({
      next: (res) => {
        this.users = res;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      }
    });
  }
  
  // Helper para iniciales (copiado del otro componente para que se vea bonito)
  getIniciales(user: PublicUserProfileDto): string {
    const n = user.name?.charAt(0) || '';
    const s = user.surname?.charAt(0) || '';
    return (n + s).toUpperCase();
  }
}