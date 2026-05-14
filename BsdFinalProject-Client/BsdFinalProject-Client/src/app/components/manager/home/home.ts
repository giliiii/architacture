import { Component,OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MenubarModule } from 'primeng/menubar';
import { MenuItem } from 'primeng/api';
import path from 'path';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {  DonorManage  } from '../donor-manage/donor-manage';
import { GiftManage} from '../gift-manage/gift-manage';
import { CardManage } from '../card-manage/card-manage';


const routes: Routes = [
  { path: '', redirectTo: '/manager/home', pathMatch: 'full' },
  { path: 'manager/donors', component: DonorManage },
  { path: 'manager/gifts', component: GiftManage },
  { path: 'manager/cards', component: CardManage },

];

@Component({
  selector: 'app-manager-home',
  standalone: true,
  imports: [CommonModule, MenubarModule],
  templateUrl: './home.html',
  styleUrls: ['./home.scss'],
})
export class Home implements OnInit {
items: MenuItem[] = [];

 ngOnInit() {
        this.items = [
            {
                label: 'ניהול תורמים',
                icon: 'pi pi-home',
                routerLink: '/manager/donors'

            },
            {
                label: 'ניהול מתנות',
                icon: 'pi pi-star',
                routerLink: '/manager/gifts'

            },
            {
                label: 'ניהול רכישות',
                icon: 'pi pi-envelope',
                routerLink: '/manager/cards'
            }
        ];
}
}