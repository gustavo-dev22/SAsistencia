export interface TurnoItem {
  id: number;
  nombre: string;
  descripcion: string | null;
  horaEntrada: string;
  horaSalida: string;
  toleranciaEntradaMinutos: number;
  limiteTardanzaMinutos: number;
  minutosRefrigerio: number;
  esRotativo: boolean;
  activo: boolean;
  totalEmpleadosAsignados: number;
  diasSemana: string;
}
