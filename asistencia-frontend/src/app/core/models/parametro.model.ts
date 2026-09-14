export interface ParametroGlobal {
  id: number;
  clave: string;
  valor: string;
  tipoDato: 'STRING' | 'INT' | 'BOOL' | 'DECIMAL' | 'TIME';
  categoria: 'QUIOSCO' | 'POLITICAS' | 'SEGURIDAD' | 'SASI' | string;
  etiqueta: string;
  descripcion?: string;
  fechaModificacion: string;
  usuarioModificador?: string;
}