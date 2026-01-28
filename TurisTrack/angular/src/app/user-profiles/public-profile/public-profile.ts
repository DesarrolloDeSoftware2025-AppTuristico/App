import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { UserProfileService, PublicUserProfileDto } from '../../proxy/user-profiles'; 

@Component({
  selector: 'app-public-profile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './public-profile.html',
  styleUrls: ['./public-profile.scss']
})
export class PublicProfileComponent implements OnInit {
  userProfile: PublicUserProfileDto | null = null;
  loading = true;
  error = false;

  constructor(
    private route: ActivatedRoute,
    private userProfileService: UserProfileService
  ) {}

  get iniciales(): string {
    if (!this.userProfile) return '';
    const nombre = this.userProfile.name?.charAt(0) || '';
    const apellido = this.userProfile.surname?.charAt(0) || '';
    return (nombre + apellido).toUpperCase();
  }
  
  ngOnInit(): void {
    // Obtenemos el ID de la URL
    const userId = this.route.snapshot.params['id'];

    if (userId) {
      this.cargarPerfil(userId);
    } else {
      this.error = true;
      this.loading = false;
    }
  }

  cargarPerfil(id: string) {
    this.userProfileService.getPublicProfile(id).subscribe({
      next: (res) => {
        this.userProfile = res;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.error = true;
        this.loading = false;
      }
    });
  }
}