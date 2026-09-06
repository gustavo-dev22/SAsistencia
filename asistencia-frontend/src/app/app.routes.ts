import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/pages/login/login.component';
import { MainLayoutComponent } from './shared/components/layout/main-layout/main-layout.component';
import { DashboardComponent } from './features/admin/pages/dashboard/dashboard.component';
import { authGuard } from './core/guards/auth.guard';
import { EmpleadosComponent } from './features/admin/pages/empleados/empleados.component';

export const routes: Routes = [
  // Rutas públicas con Lazy Loading
  { 
    path: '', 
    loadComponent: () => import('./features/auth/pages/login/login.component').then(m => m.LoginComponent) 
  },
  { 
    path: 'login', 
    loadComponent: () => import('./features/auth/pages/login/login.component').then(m => m.LoginComponent) 
  },

  // Layout Administrativo Compartido
  {
    path: 'admin',
    loadComponent: () => import('./shared/components/layout/main-layout/main-layout.component').then(m => m.MainLayoutComponent),
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { 
        path: 'dashboard', 
        loadComponent: () => import('./features/admin/pages/dashboard/dashboard.component').then(m => m.DashboardComponent) 
      },
      { 
        path: 'personal/empleados', 
        loadComponent: () => import('./features/admin/pages/empleados/empleados.component').then(m => m.EmpleadosComponent) 
      },
      { 
        path: 'personal/areas', 
        loadComponent: () => import('./features/admin/pages/areas-cargos/areas-cargos.component').then(m => m.AreasCargosComponent) 
      }
    ]
  },

  // Fallback
  { path: '**', redirectTo: '' }
];
