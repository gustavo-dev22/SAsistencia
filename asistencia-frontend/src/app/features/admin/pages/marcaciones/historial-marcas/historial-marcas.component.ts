import { Component, OnInit, signal, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HistorialMarcacionesService } from '../../../../../core/services/historial-marcaciones.service';
import { OrganizacionService } from '../../../../../core/services/organizacion.service';

// PrimeNG
import { TableModule, TableLazyLoadEvent } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { MessageService } from 'primeng/api';
import { FiltroHistorial, ItemHistorial } from '../../../../../core/models/marcacion.model';
import { OficinaItem } from '../../../../../core/models/organizacion.model';

@Component({
  selector: 'app-historial-marcas',
  standalone: true,
  imports: [CommonModule, FormsModule, TableModule, ToastModule, TooltipModule],
  providers: [MessageService],
  templateUrl: './historial-marcas.component.html'
})
export class HistorialMarcasComponent implements OnInit {
  private historialService = inject(HistorialMarcacionesService);
  private orgService = inject(OrganizacionService);
  private messageService = inject(MessageService);
  private cdr = inject(ChangeDetectorRef);

  marcaciones = signal<ItemHistorial[]>([]);
  oficinas = signal<OficinaItem[]>([]);
  totalRegistros = signal<number>(0);
  cargando = signal<boolean>(false);

  // Filtros
  fechaInicio = signal<string>(this.obtenerFechaPrimerDiaDelMes());
  fechaFin = signal<string>(this.obtenerFechaHoy());
  busqueda = signal<string>('');
  oficinaSeleccionada = signal<number | null>(null);
  tipoSeleccionado = signal<string>('');
  puntualidadSeleccionada = signal<string>('');

  // Paginación
  pagina = 1;
  filasPorPagina = 15;

  ngOnInit(): void {
    this.cargarOficinas();
    this.consultar(1);
  }

  obtenerFechaHoy(): string {
    return new Date().toISOString().split('T')[0];
  }

  obtenerFechaPrimerDiaDelMes(): string {
    const d = new Date();
    d.setDate(1);
    return d.toISOString().split('T')[0];
  }

  cargarOficinas(): void {
    this.orgService.listarOficinas().subscribe({
      next: (data) => this.oficinas.set(data.filter(o => o.activo))
    });
  }

  consultar(pagina: number = 1): void {
    this.cargando.set(true);
    this.pagina = pagina;

    const payload: FiltroHistorial = {
      fechaInicio: this.fechaInicio(),
      fechaFin: this.fechaFin(),
      empleadoId: null,
      oficinaId: this.oficinaSeleccionada() || null,
      tipoMarcacion: this.tipoSeleccionado() || null,
      estadoPuntualidad: this.puntualidadSeleccionada() || null,
      busqueda: this.busqueda().trim() || undefined,
      pagina: this.pagina,
      registrosPorPagina: this.filasPorPagina
    };

    this.historialService.consultarHistorial(payload).subscribe({
      next: (res) => {
        this.marcaciones.set(res.items);
        this.totalRegistros.set(res.totalRegistros);
        this.cargando.set(false);
        this.cdr.markForCheck();
      },
      error: () => {
        this.cargando.set(false);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Fallo al consultar el historial.' });
        this.cdr.markForCheck();
      }
    });
  }

  onLazyLoad(event: TableLazyLoadEvent): void {
    const primeraFila = event.first ?? 0;
    const filas = event.rows ?? this.filasPorPagina;
    this.filasPorPagina = filas;
    const nuevaPagina = Math.floor(primeraFila / filas) + 1;
    this.consultar(nuevaPagina);
  }

  limpiarFiltros(): void {
    this.fechaInicio.set(this.obtenerFechaPrimerDiaDelMes());
    this.fechaFin.set(this.obtenerFechaHoy());
    this.busqueda.set('');
    this.oficinaSeleccionada.set(null);
    this.tipoSeleccionado.set('');
    this.puntualidadSeleccionada.set('');
    this.consultar(1);
  }

  exportarCsv(): void {
    if (this.marcaciones().length === 0) {
      this.messageService.add({ severity: 'warn', summary: 'Atención', detail: 'No hay datos para exportar.' });
      return;
    }

    const encabezados = ['ID,Fecha,Hora,DNI,Colaborador,Area,Cargo,Turno,Tipo,Puntualidad,TardanzaMin,Metodo,IP'];
    const filas = this.marcaciones().map(m => 
      `"${m.id}","${m.fecha}","${m.hora}","${m.dni || ''}","${m.nombreCompleto}","${m.oficinaSigla || ''}","${m.cargoNombre || ''}","${m.turnoNombre || ''}","${m.tipoMarcacion}","${m.estadoPuntualidad}","${m.minutosTardanza}","${m.metodoRegistro}","${m.ipTerminal || ''}"`
    );

    const csvContent = '\uFEFF' + [encabezados, ...filas].join('\n');
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.setAttribute('href', url);
    link.setAttribute('download', `Historial_Marcaciones_${this.fechaInicio()}_al_${this.fechaFin()}.csv`);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }
}