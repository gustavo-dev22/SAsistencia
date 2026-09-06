export interface OficinaItem {
  id: number;
  nombre: string;
  sigla: string;
  idOficinaPadre: number | null;
  nombreOficinaPadre: string;
  activo: boolean;
  totalEmpleados: number;
}

export interface CargoItem {
  id: number;
  nombre: string;
  descripcion: string | null;
  exoneradoMarcacion: boolean;
  activo: boolean;
  totalEmpleados: number;
}