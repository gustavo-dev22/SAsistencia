import { Component, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';

// PrimeNG
import { ButtonModule } from 'primeng/button';
import { AvatarModule } from 'primeng/avatar';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    ButtonModule,
    AvatarModule,
    TooltipModule,
    ConfirmDialog
  ],
  providers: [ConfirmationService],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.css'
})
export class MainLayoutComponent {
  // Datos del usuario, rol y menús desde AuthService
  usuario = computed(() => this.authService.currentUser());
  rol = computed(() => this.authService.userRole());
  menus = computed(() => this.authService.userMenus());

  // Iniciales del usuario para el Avatar
  iniciales = computed(() => {
    const nombre = this.usuario()?.nombreCompleto || '';
    return nombre
      .split(' ')
      .slice(0, 2)
      .map(n => n[0])
      .join('')
      .toUpperCase();
  });

  constructor(private authService: AuthService, private confirmationService: ConfirmationService) {}

  cerrarSesion(event: Event): void {
    this.confirmationService.confirm({
      target: event.target as EventTarget,
      message: '¿Está seguro de que desea salir del sistema de asistencias?',
      header: 'Cerrar Sesión',
      icon: 'pi pi-exclamation-triangle text-amber-400',
      acceptLabel: 'Sí, Salir',
      rejectLabel: 'Cancelar',
      acceptButtonStyleClass: 'p-button-danger p-button-sm',
      rejectButtonStyleClass: 'p-button-text p-button-secondary p-button-sm',
      accept: () => {
        this.authService.logout();
      }
    });
  }
}