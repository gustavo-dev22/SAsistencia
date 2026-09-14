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
  {
    path: 'quiosco',
    loadComponent: () => import('./features/quiosco/pages/modo-quiosco/modo-quiosco.component').then(m => m.ModoQuioscoComponent)
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
      },
      { 
        path: 'turnos/catalogo', 
        loadComponent: () => import('./features/admin/pages/catalogo-turnos/catalogo-turnos.component').then(m => m.CatalogoTurnosComponent) 
      },
      { 
        path: 'turnos/asignacion', 
        loadComponent: () => import('./features/admin/pages/asignacion-turnos/asignacion-turnos.component').then(m => m.AsignacionTurnosComponent) 
      },
      { 
        path: 'marcaciones/en-vivo', 
        loadComponent: () => import('./features/admin/pages/marcaciones/registro-en-vivo/registro-en-vivo.component').then(m => m.RegistroEnVivoComponent) 
      },
      { 
        path: 'marcaciones/historial', 
        loadComponent: () => import('./features/admin/pages/marcaciones/historial-marcas/historial-marcas.component').then(m => m.HistorialMarcasComponent) 
      },
      { 
        path: 'incidencias/justificaciones', 
        loadComponent: () => import('./features/admin/pages/incidencias/justificaciones/justificaciones.component').then(m => m.JustificacionesComponent) 
      },
      { 
        path: 'configuracion/parametros', 
        loadComponent: () => import('./features/admin/pages/configuracion/parametros-globales/parametros-globales.component').then(m => m.ParametrosGlobalesComponent) 
      },
      { 
        path: 'configuracion/auditoria', 
        loadComponent: () => import('./features/admin/pages/configuracion/auditoria-marcas/auditoria-marcas.component').then(m => m.AuditoriaMarcasComponent) 
      },
      { 
        path: 'configuracion/calendario-laboral', 
        loadComponent: () => import('./features/admin/pages/configuracion/calendario-laboral/calendario-laboral.component').then(m => m.CalendarioLaboralComponent) 
      }
    ]
  },

  // Fallback
  { path: '**', redirectTo: '' }
];
