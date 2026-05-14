
import { Observable } from 'rxjs';
import { GiftModel } from '../Models/gift';
import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';



@Injectable({
  providedIn: 'root',
})
export class GiftService {
     BASE_URL = 'https://localhost:7097/api/Gifts'; 
      http: HttpClient = inject(HttpClient);

  constructor() {}

    private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    });
  }

  getAllGifts(): Observable<GiftModel[]> {
    return this.http.get<GiftModel[]>(this.BASE_URL);
  }

  getGiftById(id: number): Observable<GiftModel> {
    return this.http.get<GiftModel>(`${this.BASE_URL}/${id}`);
  }

  createGift(gift: GiftModel,headers?:HttpHeaders): Observable<GiftModel> {
     const finalHeaders = headers || this.getHeaders();
     const options = { headers: finalHeaders };
    return this.http.post<GiftModel>(this.BASE_URL, gift,options);
  }



  updateGift(gift: GiftModel, headers?: HttpHeaders): Observable<GiftModel> {
     const finalHeaders = headers || this.getHeaders();
    const options = { headers: finalHeaders };
    return this.http.put<GiftModel>(`${this.BASE_URL}/${gift.id}`, gift, options);
  }

  deleteGift(id: number,headers?: HttpHeaders): Observable<boolean> {
    const finalHeaders = headers || this.getHeaders();
    const options = { headers: finalHeaders };
    return this.http.delete<boolean>(`${this.BASE_URL}/${id}`, options);
  }

  getGiftsByCategory(categoryId: number): Observable<GiftModel[]> {
    return this.http.get<GiftModel[]>(
      `${this.BASE_URL}/category/${categoryId}`
    );
  }

  getGiftsByCost(price1: number, price2: number): Observable<GiftModel[]> {
    return this.http.get<GiftModel[]>(
      `${this.BASE_URL}/cost/${price1}/${price2}`
    );
  }
}
