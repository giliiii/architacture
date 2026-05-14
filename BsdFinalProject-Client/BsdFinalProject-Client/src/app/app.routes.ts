import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login';
import { Home as UserHome } from './components/user/home/home';
import { Home as ManagerHome } from './components/manager/home/home';
import { RegisterComponent}  from './components/register/register';
import { DonorManage } from './components/manager/donor-manage/donor-manage';
import { GiftManage } from './components/manager/gift-manage/gift-manage';
import { CardManage } from './components/manager/card-manage/card-manage';
import { Basket } from './components/user/basket/basket';

export const routes: Routes = [
  { path: '', component: LoginComponent },
  { path: 'login', component: LoginComponent },
  { path: 'user/home', component: UserHome },
  { path: 'user/basket', component: Basket },
  { path: 'manager/home', component: ManagerHome },
  { path: 'register', component: RegisterComponent },
  { path: 'manager/donors', component: DonorManage },
  { path: 'manager/gifts', component: GiftManage },
  { path: 'manager/cards', component: CardManage }, 
   { path: '**', redirectTo: '' },
    // אתה יכול להוסיף עוד נתיבים לפי הצורך
  
];
