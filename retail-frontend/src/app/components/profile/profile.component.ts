import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { UserProfile } from '../../models/auth.model';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.component.html'
})
export class ProfileComponent implements OnInit {
  auth = inject(AuthService);
  http = inject(HttpClient);

  profile: UserProfile | null = null;
  loading = false;
  successMsg = '';
  errorMsg = '';
  pwdError = '';
  pwdSuccess = '';

  profileForm = { firstName: '', lastName: '', phoneNumber: '', address: '' };
  pwdForm = { currentPassword: '', newPassword: '' };

  ngOnInit(): void {
    this.loading = true;
    this.auth.getProfile().subscribe({
      next: (p) => {
        this.profile = p;
        this.profileForm = { firstName: p.firstName, lastName: p.lastName, phoneNumber: p.phoneNumber, address: p.address };
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }

  updateProfile(): void {
    this.errorMsg = '';
    this.http.put<UserProfile>('/api/auth/profile', this.profileForm).subscribe({
      next: (p) => {
        this.profile = p;
        this.successMsg = 'Profile updated successfully!';
        setTimeout(() => this.successMsg = '', 3000);
      },
      error: (err) => this.errorMsg = err.error?.message || 'Update failed.'
    });
  }

  changePassword(): void {
    this.pwdError = '';
    this.http.put('/api/auth/change-password', this.pwdForm).subscribe({
      next: () => {
        this.pwdSuccess = 'Password changed successfully!';
        this.pwdForm = { currentPassword: '', newPassword: '' };
        setTimeout(() => this.pwdSuccess = '', 3000);
      },
      error: (err) => this.pwdError = err.error?.message || 'Password change failed.'
    });
  }
}
