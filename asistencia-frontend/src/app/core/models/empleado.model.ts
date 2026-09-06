export interface EmpleadoItem {
  id: number;
  usuarioIdSasi: string;
  nombreCompleto: string;
  email: string;
  dni: string | null;
  codigoQr: string;
  turnoId: number | null;
  turnoNombre: string;
  oficinaId: number | null;
  oficinaNombre?: string;
  oficinaSigla?: string;
  cargoId: number | null;        // <-- NUEVO
  cargoNombre?: string;          // <-- NUEVO
  habilitadoParaMarcar: boolean;
}