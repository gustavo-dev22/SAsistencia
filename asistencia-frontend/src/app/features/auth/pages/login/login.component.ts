import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';

// PrimeNG
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    InputTextModule,
    PasswordModule,
    ButtonModule,
    CheckboxModule,
    ToastModule
  ],
  providers: [MessageService],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  cargando = signal<boolean>(false);
  errorMensaje = signal<string | null>(null);

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private messageService: MessageService
  ) {
    this.loginForm = this.fb.group({
      usuario: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(4)]],
      recordarSesion: [false]
    });
  }

  ngOnInit(): void {
    // Si hay un usuario recordado previamente, se auto-completa el formulario
    const recordado = this.authService.getUsuarioRecordado();
    if (recordado) {
      this.loginForm.patchValue({
        usuario: recordado,
        recordarSesion: true
      });
    }
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      this.errorMensaje.set('Complete los campos obligatorios para ingresar.');
      return;
    }

    this.cargando.set(true);
    this.errorMensaje.set(null);

    const { usuario, password, recordarSesion } = this.loginForm.value;

    // Pasamos el valor de recordarSesion a la función login
    this.authService.login(usuario, password, recordarSesion).subscribe({
      next: (res) => {
        this.cargando.set(false);

        this.messageService.add({
          severity: 'success',
          summary: 'Bienvenido',
          detail: `Acceso concedido: ${res.usuario.nombreCompleto}`
        });

        setTimeout(() => {
          if (res.rolActivo.nombreRol === 'TERMINAL_ASISTENCIA') {
            this.router.navigate(['/quiosco']);
          } else {
            this.router.navigate(['/admin/dashboard']);
          }
        }, 500);
      },
      error: (err) => {
        this.cargando.set(false);
        const detalle = err.error?.message || 'Error de autenticación.';
        this.errorMensaje.set(detalle);

        this.messageService.add({
          severity: 'error',
          summary: 'Acceso Denegado',
          detail: detalle
        });
      }
    });
  }

  campoInvalido(campo: string): boolean {
    const control = this.loginForm.get(campo);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }
}