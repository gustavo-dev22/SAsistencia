import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoadingService } from '../../../core/services/loading.service';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

@Component({
  selector: 'app-global-loading',
  standalone: true,
  imports: [CommonModule, ProgressSpinnerModule],
  templateUrl: './global-loading.component.html'
})
export class GlobalLoadingComponent {
  loadingService = inject(LoadingService);
}