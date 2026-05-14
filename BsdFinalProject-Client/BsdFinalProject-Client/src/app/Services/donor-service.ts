import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {DonorModel} from '../Models/donor'
import { HttpClient, HttpParams } from '@angular/common/http';
import { inject } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { GiftModel } from '../Models/gift';

@Injectable({
  providedIn: 'root',
})
export class DonorService {
  BASE_URL = 'https://localhost:7097/api/Donors';
  http: HttpClient = inject(HttpClient);
  
  constructor() { }

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    });
  }

  createDonor(item:DonorModel, headers?: HttpHeaders): Observable<DonorModel> {
    const finalHeaders = headers || this.getHeaders();
    const options = { headers: finalHeaders };
    return this.http.post<DonorModel>(`${this.BASE_URL}`, item, options);
  }

  updateDonor(item: DonorModel, headers?: HttpHeaders): Observable<DonorModel> {
    const finalHeaders = headers || this.getHeaders();
    const options = { headers: finalHeaders };
    return this.http.put<DonorModel>(`${this.BASE_URL}`, item, options);
  }

  getDonors(headers?: HttpHeaders): Observable<DonorModel[]> {
    const finalHeaders = headers || this.getHeaders();
    const options = {
      headers: finalHeaders  
    };
    return this.http.get<DonorModel[]>(this.BASE_URL, options);
  }

  getOneDonor(id: number): Observable<DonorModel> {
    const options = { headers: this.getHeaders() };
    return this.http.get<DonorModel>(`${this.BASE_URL}/${id}`, options);
  }

  deleteDonor(id: number, headers?: HttpHeaders): Observable<DonorModel> {
    const finalHeaders = headers || this.getHeaders();
    const options = { headers: finalHeaders };
    return this.http.delete<DonorModel>(`${this.BASE_URL}/${id}`, options);
  }

  getDonorGifts(donorId: Number, headers?:HttpHeaders): Observable<GiftModel[]> {
    const finalHeaders = headers || this.getHeaders();
    const options = { headers: finalHeaders };
    return this.http.get<GiftModel[]>(`${this.BASE_URL}/${donorId}/gifts`,options);
  }

  
}
