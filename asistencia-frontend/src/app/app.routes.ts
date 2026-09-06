import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/pages/login/login.component';
import { MainLayoutComponent } from './shared/components/layout/main-layout/main-layout.component';
import { DashboardComponent } from './features/admin/pages/dashboard/dashboard.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', component: LoginComponent },
  { path: 'login', component: LoginComponent },
  // Layout compartido para todas las páginas administrativas
  {
    path: 'admin',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: DashboardComponent },
      // Conforme construyamos los siguientes módulos, los agregaremos aquí:
      // { path: 'personal/empleados', component: EmpleadosComponent },
      // { path: 'turnos/catalogo', component: TurnosComponent },
      // { path: 'marcaciones/en-vivo', component: MarcacionesEnVivoComponent },
    ]
  },
  { path: '**', redirectTo: '' }
];
