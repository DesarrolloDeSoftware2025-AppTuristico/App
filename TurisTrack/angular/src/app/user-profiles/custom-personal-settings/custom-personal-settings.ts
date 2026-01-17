import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ProfileService, UpdateProfileDto } from '../../proxy/volo/abp/account';
import { ToasterService, ConfirmationService } from '@abp/ng.theme.shared';
import { UserProfileService } from '../../proxy/user-profiles';
import { AuthService } from '@abp/ng.core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-custom-personal-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './custom-personal-settings.html'
})
export class CustomPersonalSettingsComponent implements OnInit {
  form: FormGroup;
  inProgress = false;

  constructor(
    private fb: FormBuilder,
    private profileService: ProfileService,
    private toaster: ToasterService,
    private userProfileService: UserProfileService,
    private confirmation: ConfirmationService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.buildForm();
    this.loadData();
  }

  buildForm() {
    this.form = this.fb.group({
      userName: [{ value: '', disabled: true }],
      name: ['', Validators.required],
      surname: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: [''],
      // 1. AGREGAMOS EL CAMPO PREFERENCIAS
      preferencias: [''] 
    });
  }

  loadData() {
    this.profileService.get().subscribe(res => {
      // Cargamos los datos estándar
      this.form.patchValue(res);

      // 2. CARGAMOS LAS PREFERENCIAS (ExtraProperties)
      // Si usas ExtraProperties en el backend, suelen venir aquí.
      // Si tu propiedad es mayúscula en C# ("Preferencias"), aquí búscala igual.
      const prefs = res.extraProperties && res.extraProperties['Preferencias'];
      if (prefs) {
        this.form.get('preferencias')?.setValue(prefs);
      }

      // IMPORTANTE: Marcamos el formulario como "Pristine" (intacto) 
      // para que el botón de guardar aparezca deshabilitado al inicio.
      this.form.markAsPristine(); 
    });
  }

  save() {
    // 3. VALIDAMOS SI EL FORMULARIO FUE TOCADO
    // Si es invalido, está en progreso o NO se ha modificado (pristine), no hacemos nada.
    if (this.form.invalid || this.inProgress || this.form.pristine) return;

    this.inProgress = true;

    // Obtenemos los valores crudos
    const formValue = this.form.getRawValue();

    // Preparamos el DTO
    const dto: UpdateProfileDto = {
      ...formValue,
      extraProperties: {
        // 4. MAPEA TUS CAMPOS EXTRAS AL DTO
        // Asegúrate que el Key 'Preferencias' coincida con lo que espera tu Backend
        'Preferencias': formValue.preferencias 
      }
    };

    this.profileService.update(dto).subscribe({
      next: () => {
        this.toaster.success('Perfil actualizado correctamente');
        this.inProgress = false;
        
        // 5. REINICIAMOS EL ESTADO DE CAMBIOS
        // Esto deshabilita el botón nuevamente hasta que el usuario escriba algo nuevo.
        this.form.markAsPristine(); 
      },
      error: (err) => {
        this.toaster.error(err.message || 'Error al guardar');
        this.inProgress = false;
      }
    });
  }

  deleteAccount() {
    this.confirmation.warn(
      'Esta acción es irreversible. ¿Eliminar cuenta?',
      'Zona de Peligro'
    ).subscribe((status) => {
      if (status === 'confirm') {
        this.userProfileService.deleteMyAccount().subscribe(() => {
            this.authService.logout().subscribe(() => {
                this.router.navigate(['/']);
            });
        });
      }
    });
  }
}