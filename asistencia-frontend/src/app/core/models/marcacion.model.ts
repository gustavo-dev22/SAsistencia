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