import { Injectable,inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateBasketModel } from '../Models/createBasket';
import { BasketModel } from '../Models/basket';


@Injectable({
  providedIn: 'root',
})
export class BasketService {
  
   BASE_URL = 'https://localhost:7097/api/Cards'; 
    http: HttpClient = inject(HttpClient);

    constructor() {}

    private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    });
  }

    getAllMyBasket(headers: HttpHeaders): Observable<BasketModel[]> {
    return this.http.get<BasketModel[]>(`${this.BASE_URL}`, {
      headers: headers
    });
  }

   createNewBasket(createBasket: CreateBasketModel,headers?:HttpHeaders): Observable<CreateBasketModel> {
    const finalHeaders = headers || this.getHeaders();
    const options = { headers: finalHeaders };
    return this.http.post<CreateBasketModel>(`${this.BASE_URL}`, createBasket, options);
  }
  
  // 3. מחיקת סל אחד
  deleteOneBasket(id: number,headers?:HttpHeaders): Observable<BasketModel> {
    const finalHeaders = headers || this.getHeaders();
    const options = { headers: finalHeaders };
    return this.http.delete<BasketModel>(`${this.BASE_URL}/${id}`, options);
  }

  // 4. מחיקת כל הסלים של המשתמש
  deleteAllBasket(userId: number,headers?:HttpHeaders): Observable<any> {
    const finalHeaders = headers || this.getHeaders();
    const options = { headers: finalHeaders };
    return this.http.delete(`${this.BASE_URL}?id=${userId}`, options);
  }

}
