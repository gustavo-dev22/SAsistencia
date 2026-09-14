export interface ItemAuditoria {
  id: number;
  marcacionId?: number;
  empleadoId: number;
  nombreEmpleado: string;
  dniEmpleado?: string;
  oficinaSigla?: string;
  tipoOperacion: string;
  horaAnterior?: string;
  horaNueva: string;
  tipoMarcacion: string;
  motivoJustificacion: string;
  usuarioResponsable: string;
  ipResponsable: string;
  fechaRegistro: string;
}

export interface PaginatedResult<T> {
  items: T[];
  totalRegistros: number;
  paginaActual: number;
  totalPaginas: number;
}