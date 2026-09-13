export interface ResultadoMarcacion {
  exito: boolean;
  mensaje: string;
  nombreEmpleado?: string;
  areaEmpleado?: string;
  cargoEmpleado?: string;
  hora?: string;
  tipoMarcacion?: 'ENTRADA' | 'SALIDA';
  estadoPuntualidad?: 'PUNTUAL' | 'TOLERANCIA' | 'TARDANZA' | 'EXONERADO' | 'FUERA_TURNO';
  minutosTardanza: number;
}

export interface MarcacionEnVivo {
  id: number;
  empleadoId: number;
  nombreCompleto: string;
  dni?: string;
  oficinaNombre?: string;
  oficinaSigla?: string;
  cargoNombre?: string;
  hora: string;
  tipoMarcacion: 'ENTRADA' | 'SALIDA';
  estadoPuntualidad: 'PUNTUAL' | 'TOLERANCIA' | 'TARDANZA' | 'FUERA_TURNO' | 'EXONERADO';
  minutosTardanza: number;
  metodoRegistro: string;
  fechaHora: string;
}

export interface ResumenHoy {
  totalMarcas: number;
  totalPuntuales: number;
  totalTardanzas: number;
  totalExonerados: number;
  ultimasMarcaciones: MarcacionEnVivo[];
}

export interface FiltroHistorial {
  fechaInicio: string;
  fechaFin: string;
  empleadoId?: number | null;
  oficinaId?: number | null;
  tipoMarcacion?: string | null;
  estadoPuntualidad?: string | null;
  busqueda?: string;
  pagina: number;
  registrosPorPagina: number;
}

export interface ItemHistorial {
  id: number;
  empleadoId: number;
  nombreCompleto: string;
  dni?: string;
  oficinaNombre?: string;
  oficinaSigla?: string;
  cargoNombre?: string;
  turnoNombre?: string;
  fecha: string;
  hora: string;
  tipoMarcacion: string;
  estadoPuntualidad: string;
  minutosTardanza: number;
  metodoRegistro: string;
  ipTerminal?: string;
}

export interface PaginatedResult<T> {
  items: T[];
  totalRegistros: number;
  paginaActual: number;
  totalPaginas: number;
}