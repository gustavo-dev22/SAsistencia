export interface FeriadoItem {
  id: number;
  anio: number;
  fecha: string;
  fechaLegible: string;
  diaSemana: string;
  nombre: string;
  tipo: 'FERIADO_NACIONAL' | 'NO_LABORABLE_COMPENSABLE' | 'FIESTA_INSTITUCIONAL' | string;
  aplicaSectorPublico: boolean;
  esCompensable: boolean;
  normaLegal?: string;
  fechaCompensacionLimite?: string;
  activo: boolean;
}

export interface GuardarFeriadoPayload {
  id?: number | null;
  fecha: string;
  nombre: string;
  tipo: string;
  aplicaSectorPublico: boolean;
  esCompensable: boolean;
  normaLegal?: string;
  fechaCompensacionLimite?: string | null;
}