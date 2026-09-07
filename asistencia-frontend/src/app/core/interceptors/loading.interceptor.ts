import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { finalize } from 'rxjs';
import { LoadingService } from '../services/loading.service';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const loadingService = inject(LoadingService);

  // Puedes personalizar el mensaje según la acción
  let texto = 'Cargando información...';
  if (req.method === 'POST') texto = 'Guardando o sincronizando datos...';
  if (req.method === 'PUT')  texto = 'Actualizando cambios...';
  if (req.method === 'DELETE') texto = 'Eliminando registro...';

  loadingService.mostrar(texto);

  return next(req).pipe(
    finalize(() => {
      loadingService.ocultar();
    })
  );
};