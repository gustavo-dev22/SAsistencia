export interface TipoJustificacion {
  id: number;
  nombre: string;
  descripcion: string | null;
  requiereDocumento: boolean;
  conGoceHaber: boolean;
}

export interface JustificacionItem {
  id: number;
  empleadoId: number;
  nombreCompleto: string;
  dni?: string;
  oficinaNombre?: string;
  oficinaSigla?: string;
  tipoJustificacionId: number;
  tipoJustificacionNombre: string;
  conGoceHaber: boolean;
  marcacionId?: number;
  fechaInicio: string;
  fechaFin: string;
  motivo: string;
  rutaArchivo?: string;
  nombreArchivoOriginal?: string;
  estado: 'PENDIENTE' | 'APROBADA' | 'RECHAZADA';
  observacionesAprobacion?: string;
  fechaRegistro: string;
}

export interface PaginatedResult<T> {
  items: T[];
  totalRegistros: number;
  paginaActual: number;
  totalPaginas: number;
}