import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';
import { LoginService } from '../../Services/login-service';
import { ChangeDetectorRef } from '@angular/core';
import {jwtDecode, JwtPayload} from 'jwt-decode';
import { FormsModule } from '@angular/forms';
import { AutoComplete } from 'primeng/autocomplete';
import { FloatLabel } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { FloatLabelModule } from 'primeng/floatlabel';
import { ButtonModule } from 'primeng/button';
import e from 'express';


@Component({
  selector: 'app-login', 
  imports: [CommonModule, FormsModule, FloatLabel, InputTextModule,FloatLabelModule,ButtonModule],
  templateUrl: './login.html',
  styleUrls: ['./login.scss'],
 
})
export class LoginComponent {
   loginSrv: LoginService = inject(LoginService);
   ref = inject(ChangeDetectorRef)
   router= inject(Router);
   email: string = "";
   password: string = ""; 
   role: string = "";
   isRegistering: boolean = false;
   fullName: string = "";
   phone: string = "";
   address: string = "";


  goToRegister() {
    alert("navigating to register");
// Navigate to the register component
    this.router.navigate(['/register']);
  }


login() {
  try {
    this.loginSrv.login({ EMail: this.email, Password: this.password }).subscribe({
      next: (response: any) => {
        const token =
          typeof response === 'string'
            ? response
            : response?.token ?? '';

        localStorage.setItem('token', token);

        const decoded: any = jwtDecode(token);

        this.role =
          decoded?.role ??
          decoded?.Role ??
          decoded?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ??
          '';

        console.log(this.role);

        if (this.role === 'User') {
          this.router.navigate(['user/home']);
        } else if (this.role === 'Manager') {
          this.router.navigate(['manager/home']);
        } else {
          alert('משתמש לא מזוהה עבור להרשמה');
        }
      },
     error: (err) => {
        console.log('Login error:', err);
        alert(err?.error?.message || 'שגיאת התחברות');
      }
    });
  } catch {
    alert('XXXX');
  }
}

}


