import { Observable } from 'rxjs';
import { LoginModel } from '../Models/login';
import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
@Injectable({
  providedIn: 'root',
})
export class LoginService {
  BASE_URL = 'https://localhost:7097/api/Users';
  http: HttpClient = inject(HttpClient);
  constructor() { }

  login(item:LoginModel): Observable<LoginModel> {
    return this.http.post<LoginModel>(`${this.BASE_URL}/login`, item);
  }

  register(item: any): Observable<any> {
    return this.http.post<any>(`${this.BASE_URL}/register`, item);
  }
}
