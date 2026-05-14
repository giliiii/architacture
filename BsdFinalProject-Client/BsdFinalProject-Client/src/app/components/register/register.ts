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


@Component({
  selector: 'app-login', 
  imports: [CommonModule, FormsModule, FloatLabel, InputTextModule],
  templateUrl: './register.html',
  styleUrls: ['./register.scss'],
 
})
export class RegisterComponent {
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
  
    goToLogin() {
    this.router.navigate(['/login']);
   }
   register() {
    if (!this.email || !this.password || !this.fullName) {
      alert("יש למלא את כל השדות");
      return;
    }
    
    const registerData = {
      EMail: this.email,
      Password: this.password,
      FullName: this.fullName,
      Phone: this.phone,
      Address: this.address
    };
    
    this.loginSrv.register(registerData).subscribe({
      error: (error: any) => {
        alert("הרשמה נכשלה");
        console.error('Register error:', error);
      },
      next: (response: any) => {
        alert("הרשמה הצליחה! כעת בואו להתחבר");
        const token = typeof response === 'string' ? response : response.token;
        localStorage.setItem('token', token);
        this.isRegistering = false;
        this.email = "";
        this.password = "";
      }
    });
   }
  
   login() {
    alert("1התחברות במערכת")
    try{
      alert("2התחברות במערכת")
      console.log("Sending login with:", {EMail:this.email,Password:this.password})
      this.loginSrv.login({EMail:this.email,Password:this.password}).subscribe({
        error: (error) => {
          alert("התחברות נכשלה")
          console.error('Login error:', error);
          if (error.error && error.error.errors) {
            console.error('Validation errors:', error.error.errors);
          }
        },
        next: (response: any) => {
          alert("התחברות הצליחה")
          console.log('Login successful:', response);
          // Handle both string response and object response
          const token = typeof response === 'string' ? response : (response?.token || response);
          localStorage.setItem('token', token);
          this.ref.detectChanges();
          const decoded: any= jwtDecode(token);
          this.role = decoded.role || '';
          console.log('Decoded role:', this.role)
          if(this.role=="User")
            this.router.navigate(['user/home'])
          else if(this.role=="Manager")
            this.router.navigate(['manager/home'])
          else
           alert("משתמש לא מזוהה עבור להרשמה")
        },
        

        //this.router.navigate(['/home']);
      });
    } catch (error) {
      //הוספת הודעה למשתמש
      alert("משתמש לא מזוהה עבור להרשמה")
      console.error('Login error:', error);
    }

   }

}

