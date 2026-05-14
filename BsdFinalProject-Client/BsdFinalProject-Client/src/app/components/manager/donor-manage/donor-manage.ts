import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { RippleModule } from 'primeng/ripple';
import { MessageService } from 'primeng/api';
import { DonorModel } from '../../../Models/donor';
import { DonorService } from '../../../Services/donor-service';
import { CommonModule } from '@angular/common';
import { NgZone } from '@angular/core';;
import { SelectItem } from 'primeng/api';
import { HttpClient } from '@angular/common/http';
import { DialogModule } from 'primeng/dialog';
import { jwtDecode } from 'jwt-decode';
import { HttpHeaders } from '@angular/common/http'
import { GiftModel } from '../../../Models/gift';

// // import { Product } from '@/domain/product';


@Component({
  selector: 'app-donor-manage',
  standalone: true,
  imports: [SelectModule, TableModule, TagModule, ToastModule, ButtonModule, InputTextModule, RippleModule, FormsModule,CommonModule,DialogModule],
  providers: [MessageService],
  templateUrl: './donor-manage.html',
  styleUrls: ['./donor-manage.scss'],
})
export class DonorManage implements OnInit {
     donorSrv: DonorService = inject(DonorService );
    changeRef: ChangeDetectorRef = inject(ChangeDetectorRef);
    messageService: MessageService = inject(MessageService);
     id:Number=0;
     name: string = "";
     email: string = "";
     donors:DonorModel[]=[];
     clonedDonors: {[key: string]: DonorModel} = {};
     cdr: ChangeDetectorRef = inject(ChangeDetectorRef);
    //  ngZone: NgZone = inject(NgZone);
     displayDialog: boolean = false;
     newDonorName: string = "";
     newDonorEmail: string = "";
     editingDonor: DonorModel | null = null;
     displayGiftsDialog: boolean = false;
     donorGifts: GiftModel[] = [];
     

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

    ngOnInit():void {
     try {
         const token= localStorage.getItem('token');
         console.log('token:', token);
         const headers = this.getHeaders();             
         this.donorSrv.getDonors(headers).subscribe({       
          next: (response: DonorModel[]) => {
            setTimeout(() => {
              this.donors = response;
              console.log(this.donors);
              this.changeRef.markForCheck();
            }, 0);
           },
          error: (err) => {
            console.log('Login error:', err);
            this.messageService.add({ severity: 'error', summary: 'Error', detail:  'אינך מורשה לצפות ברשימת התורמים' });
      }  
        })
       } catch {
         console.log('הבקשה נכשלה');
         this.messageService.add({ severity: 'error', summary: 'Error', detail: 'הבקשה נכשלה' });
       }
    }

    deleteDonor(donorId: number) {
      const headers = this.getHeaders();
      this.donorSrv.deleteDonor(donorId, headers).subscribe({
        next: (response) => {
          console.log('תורם נמחק בהצלחה:', response);
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'תורם נמחק בהצלחה' });
          this.ngOnInit();
        }
        ,
        error: (err) => {
          console.log('שגיאה במחיקת תורם:', err);
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'שגיאה במחיקת תורם' });
        } 
      });
    }

openAddDonorDialog() {
  this.newDonorName = "";
  this.newDonorEmail = "";
  this.displayDialog = true;
}

closeDialog() {
  this.displayDialog = false;
}

//צפיה במתנות של תורם מסוים
openGiftsDialog(donor: DonorModel) {
  this.editingDonor = donor;
  this.displayDialog = true;
}

  // לוגיקה להצגת המתנות של התורם
  viewDonorGifts(donor: DonorModel) {  
   if (!donor || !donor.id) return;
   const headers = this.getHeaders();
    this.donorSrv.getDonorGifts(donor.id,headers).subscribe({
    next: (gifts) => {
      console.log(gifts)
      this.donorGifts = gifts || [];
      console.log(this.donorGifts);
      // פתיחת הדיאלוג אחרי שהנתונים התקבלו
      this.displayGiftsDialog = true;
      setTimeout(() => this.cdr.markForCheck(), 0);
    },
    error: (err) => {
      console.error('שגיאה בקבלת מתנות לתורם', err);
      alert('שגיאה בקבלת מתנות לתורם: ' + (err.error?.message || err.message));
    }
  });
}



addNewDonor() {
  if (!this.newDonorName.trim() || !this.newDonorEmail.trim()) {
      this.messageService.add({ severity: 'error', summary: 'Error', detail: 'אנא מלא את כל השדות' });
  return;
  }

  const newDonor: DonorModel = new DonorModel();
  newDonor.Name = this.newDonorName;
  newDonor.EMail = this.newDonorEmail;

  const headers = this.getHeaders();
  this.donorSrv.createDonor(newDonor, headers).subscribe({
    next: (response) => {
      console.log('תורם חדש נוצר בהצלחה:', response);
      this.messageService.add({ severity: 'success', summary: 'Success', detail: 'תורם חדש נוצר בהצלחה' });
      this.closeDialog();
      this.ngOnInit();
    },
    error: (err) => {
      console.log('שגיאה בהוספת תורם:', err);
      this.messageService.add({ severity: 'error', summary: 'Error', detail: 'שגיאה בהוספת תורם' });
      this.closeDialog();
    }
  });
}



    onRowEditInit(donor: DonorModel) {
        this.clonedDonors[String(donor.id)] = { ...donor };
    }

    onRowEditSave(donor: DonorModel) {
        if (donor.Name  && donor.EMail) {
                 try {      
                     const headers = this.getHeaders();
                     this.donorSrv.updateDonor(donor, headers).subscribe({       
                     next: (response: DonorModel) => {
                         console.log(response)
                         try {
                         this.changeRef.detectChanges();
                         this.messageService.add({ severity: 'success', summary: 'Success', detail: 'תורם עודכן בהצלחה' });
                         this.ngOnInit();
                         } catch {}
                     },
                    error: (err) => {
                       console.log('Error updating donor:', err);
                       this.messageService.add({ severity: 'error', summary: 'Error', detail: 'פרטים לא חוקיים' });
                     }  
                   })
                 } catch (error) {
                     this.messageService.add({ severity: 'error', summary: 'Error', detail:'לא ניתן לעדכן תורם' });
                 }
            delete this.clonedDonors[String(donor.id)];
        } else {
            this.messageService.add({ severity: 'error', summary: 'Error', detail: 'פרטים לא חוקיים' });
        }
    }

    onRowEditCancel(donor: DonorModel, index: number) {
        this.donors[index] = this.clonedDonors[String(donor.id)];
        delete this.clonedDonors[String(donor.id)];
    }

    getSeverity(status: string): string | undefined {
        switch (status) {
            case 'INSTOCK':
                return 'success';
            case 'LOWSTOCK':
                return 'warn';
            case 'OUTOFSTOCK':
                return 'danger';
            default:
                return undefined;
        }
    }

}
