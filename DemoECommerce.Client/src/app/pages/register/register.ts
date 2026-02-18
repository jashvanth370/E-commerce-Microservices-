import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';
import { AppUserDTO } from '../../models/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  fb = inject(FormBuilder);
  router = inject(Router);
  authService = inject(AuthService);

  registerForm = this.fb.group({
    name: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]],
  });

  onSubmit() {
    if (this.registerForm.valid) {
      if (this.registerForm.value.password !== this.registerForm.value.confirmPassword) {
        alert("Passwords don't match");
        return;
      }

      const user: AppUserDTO = {
        name: this.registerForm.value.name!,
        email: this.registerForm.value.email!,
        password: this.registerForm.value.password!,
        role: 'User'
      };

      this.authService.register(user).subscribe({
        next: (res) => {
          if (res.flag) {
            alert(res.message);
            this.router.navigate(['/login']);
          } else {
            alert(res.message);
          }
        },
        error: (err) => {
          console.error(err);
          alert('Registration failed');
        },
      });
    }
  }
}
