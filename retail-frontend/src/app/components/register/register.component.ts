import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  auth = inject(AuthService);
  router = inject(Router);

  model = { firstName: '', lastName: '', email: '', password: '', phoneNumber: '', address: '' };
  loading = false;
  error = '';
  success = '';

  register(): void {
    this.loading = true;
    this.error = '';
    this.auth.register(this.model).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/products']);
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Registration failed. Please try again.';
      }
    });
  }
}
