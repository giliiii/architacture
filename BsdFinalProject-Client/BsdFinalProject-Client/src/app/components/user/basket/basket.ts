
import { Component, OnInit } from '@angular/core';
import { BasketService } from '../../../Services/basket-service';
import { BasketModel } from '../../../Models/basket';
import { CreateBasketModel } from '../../../Models/createBasket';
import { HttpHeaders } from '@angular/common/http';
import {jwtDecode} from 'jwt-decode';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';  // ייבוא CommonModule
import { Router } from '@angular/router';
import { inject } from '@angular/core';
import { Drawer, DrawerModule } from 'primeng/drawer';
// import { AppComponent } from './app.component';

@Component({
  selector: 'app-basket',
  imports: [TableModule, ButtonModule, CommonModule, DrawerModule],
  templateUrl: './basket.html',
  styleUrl: './basket.scss',
})
export class Basket {
  baskets: BasketModel[] = [];
  router= inject(Router);
  constructor(private basketService: BasketService) { }

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    let userRole = '';
    if (token) {
      const decodedToken: any = jwtDecode(token);
      userRole = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || '';
    }
    return new HttpHeaders({
      'User-Role': userRole,
      'Authorization': token ? `Bearer ${token}` : ''
    });
  }
  
  ngOnInit(): void {
    this.getAllBaskets();
  }

   getAllBaskets() {
    const headers=this.getHeaders();
    const userId = 1; // זיהוי המשתמש (בפועל יהיה מ-Token או דרך אחר)
    this.basketService.getAllMyBasket(headers).subscribe(
      (data) => {
        this.baskets = data;
      },
      (error) => {
        console.error('Error fetching baskets:', error);
      }
    );
  }

  // פונקציה ליצירת סל חדש
  // createBasket() {
  //   const headers=this.getHeaders();
  //   const newBasket = new CreateBasketModel(101, 1); // לדוגמה, giftId = 101, userId = 1
  //   this.basketService.createNewBasket(newBasket,headers).subscribe(
  //     (data) => {
  //       console.log('New basket created:', data);
  //       this.getAllBaskets(); // עדכון הסלים אחרי יצירת סל חדש
  //     },
  //     (error) => {
  //       console.error('Error creating basket:', error);
  //     }
  //   );
  // }

  // פונקציה למחיקת סל
  deleteBasket(id: number) {
    const headers=this.getHeaders();
    this.basketService.deleteOneBasket(id,headers).subscribe(
      (data) => {
        console.log('Basket deleted:', data);
        this.getAllBaskets(); // עדכון הסלים אחרי מחיקה
      },
      (error) => {
        console.error('Error deleting basket:', error);
      }
    );
  }

  goToShop() {
   this.router.navigate(['/user/home']);
}
}

