
import { Observable } from 'rxjs';
import { CardModel } from '../Models/card';
import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
@Injectable({
  providedIn: 'root',
})
export class CardService {
    BASE_URL = 'https://localhost:7097/api/Cards'; 
    http: HttpClient = inject(HttpClient);

    constructor() {}

    private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    });
  }

  
    getCardById(id: number): Observable<any> {
        return this.http.get<any>(`${this.BASE_URL}/byId/${id}`);
    }

    getMyCards(): Observable<any[]> {
        return this.http.get<any[]>(`${this.BASE_URL}/myCards`);
    }

    createNewCards(baskets: any[]): Observable<any[]> {
        return this.http.post<any[]>(`${this.BASE_URL}/createCards`, baskets);
    }

    getPopularPurchases(): Observable<CardModel[]> {
        return this.http.get<CardModel[]>(`${this.BASE_URL}/popular-purchases`);
    }

    getAllCardsWithBuyers(): Observable<CardModel[]> {
        return this.http.get<CardModel[]>(`${this.BASE_URL}/withBuyers`);
    }

    getAllPurchasesOrderedByCost(): Observable<CardModel[]> {
        return this.http.get<CardModel[]>(`${this.BASE_URL}/by-cost`);
    }

  
}
