import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { GlobalLoadingComponent } from './shared/components/global-loading/global-loading.component';

@Component({
  imports: [RouterOutlet, GlobalLoadingComponent],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('asistencia-frontend');
}
