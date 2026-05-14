
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { RippleModule } from 'primeng/ripple';
import { SelectItem, MessageService } from 'primeng/api';
import { DataViewModule } from 'primeng/dataview';
import { SelectButtonModule } from 'primeng/selectbutton';
import { DialogModule } from 'primeng/dialog';
import { GiftModel } from '../../../Models/gift';
import { GiftService } from '../../../Services/gift-service';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core';
import { categoryModel } from '../../../Models/category';
import { CategoryService } from '../../../Services/category-service';
import { ToastModule } from 'primeng/toast';
import { HttpClient,HttpHeaders } from '@angular/common/http'
import { jwtDecode } from 'jwt-decode';
import { DonorService } from '../../../Services/donor-service';
import { DonorModel } from '../../../Models/donor';
import { error, log } from 'console';
import { PanelModule } from 'primeng/panel';
import { BadgeModule } from 'primeng/badge';
import { FileUploadModule } from 'primeng/fileupload';
import { ProgressBarModule } from 'primeng/progressbar';
import { PrimeNG } from 'primeng/config';

interface UploadEvent {
    originalEvent: Event;
    files: File[];
}

// import { ProductService } from '@/service/productservice';
// import { Product } from '@/domain/product';
// import { Product } from '@/domain/product';
@Component({
  selector: 'app-gift-manage',
  imports: [DataViewModule, SelectButtonModule, TagModule, ButtonModule, FormsModule, CommonModule, CurrencyPipe, DialogModule, SelectModule, InputTextModule,ToastModule,PanelModule,BadgeModule,FileUploadModule,ProgressBarModule],
  providers: [MessageService],
  templateUrl: './gift-manage.html',
  styleUrl: './gift-manage.scss',
})
export class GiftManage {

constructor(private cdr: ChangeDetectorRef, private messageService: MessageService) { }

    gifts:GiftModel[]=[];
    giftSrv:GiftService=inject(GiftService) ;
    donorSrv:DonorService=inject(DonorService);
    donors:DonorModel[]=[];                
    cSrv:CategoryService=inject(CategoryService) ;
    id:Number=0;
    name:string="";
    description:string="";
    cost:Number=0;
    picture:string="";
    categoryId:Number=0;
    donorId:Number=0;
    winnerName:string="";
    layout: 'list' | 'grid' = 'list';
    options: SelectItem[] = [
    { label: 'List', value: 'list' },
    { label: 'Grid', value: 'grid' }
    ];
    displayDialog: boolean = false;
    newGiftName:string="";
    newGiftDescription:string="";
    newGiftCost:Number=0;
    newGiftPicture:string="";
    newGiftCategoryId:Number=0;
    categories: categoryModel[] = [];   
    categoryOptions: SelectItem[] = [];
    selectedCategory: Number = 0;
    filteredGifts:GiftModel[]=[];
    displayDialog2: boolean = false;
    newGiftid!: number;
    newGiftname!: string;
    newGiftdescription?: string;
    newGiftcost!: number;
    newGiftpicture?: string;
    newGiftcategoryId!: categoryModel;
    newGiftdonorId!: DonorModel;
    selectedDonor: DonorModel | null = null;
    searchByGiftName: string = "";
    donorsName:string[]=[];
    files: any[] = [];
    totalSize: number = 0;
    totalSizePercent: number = 0;

    // messageService: MessageService = inject(MessageService);

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
  
       ngOnInit() {
        // console.log("Initializing");
        //     this.initializeCategories();
        //     try {
        //          this.giftSrv.getAllGifts().subscribe({       
        //          next: (response: GiftModel[]) => {
        //            this.gifts=response;
        //            console.log("gifts",this.gifts)
        //           //  this.cdr.detectChanges(); 
        //           },
        //          error: (err) => {
        //            console.log('Login error:', err);
        //      }  
        //        })
        //       } catch {
        //         alert('הבקשה נכשלה');
        //       }
                    try {
                  // thisheaders = this.getHeaders();  
                    const headers=this.getHeaders();
                  // Load all gifts
                    this.giftSrv.getAllGifts().subscribe({       
                    next: (response: GiftModel[]) => {
                    // Convert gifts to proper format
                      this.gifts = response.map((gift: any) => {
                        const converted = new GiftModel();
                        converted.id = gift.id;
                        converted.name = gift.name;
                        converted.description = gift.description;
                        converted.cost = gift.cost;
                        converted.picture = gift.picture;
                        converted.categoryId = gift.categoryId;
                        converted.donorId = gift.donorId;
                        converted.winnerName = gift.winnerName;
                        return converted;
                    })
                    this.filteredGifts = this.gifts;
                    console.log("✓ Gifts loaded:", this.gifts)
                    setTimeout(() => {
                      this.cdr.detectChanges();
                    }, 100); 
                    // Load all donors
                    this.donorSrv.getDonors(headers).subscribe({
                      next: (response: DonorModel[]) => {
                        this.donors = response || [];
                        //הכנסת שמות התורמים למערך donorsName
                        this.donorsName = this.donors.map(donor => donor.Name);
                        //
                        alert('Donors loaded: ' + JSON.stringify(this.donors));
                        console.log('✓ Donors loaded:', this.donors);
                        setTimeout(() => {
                          this.cdr.detectChanges();
                        }, 110); 
                      },
                      error: (err) => {
                      console.log('✗ Load donors error:', err);
                      }
                    })
                    console.log("✓ Donors loaded:", this.donors);
                    // Log the structure of first donor
                    if (this.donors.length > 0) {
                      console.log("First donor structure:", {
                        Id: this.donors[0].id,
                        Name: this.donors[0].Name,
                        Email: this.donors[0].EMail
                      });
                    }},
                    error: () => {
                      console.log('✗ Load donors error:');
                    }  
                  });
                    
                    this.cSrv.getAllCategories().subscribe({
                      next: (response: categoryModel[]) => {
                        this.categories = response || [];
                        console.log('✓ Categories loaded:', this.categories);
                        setTimeout(() => {
                          this.cdr.detectChanges();
                        }, 0); 
                      },
                      error: (err) => {
                        console.log('✗ Load categories error:', err);
                      }
                    });
                    this.donorSrv.getDonors(headers).subscribe({
                      next: (response: DonorModel[]) => {
                        this.donors = response || [];
                        console.log('✓ Donors loaded:', this.donors);
                        setTimeout(() => {
                          this.cdr.detectChanges();
                        }, 0);
                      },
                      error: (err) => {
                        console.log('✗ Load donors error:', err);
                      }
                    });
                  
            }           
            catch (error) {
              console.log('✗ Initialization error:', error);
            }
          };

      initializeCategories() {
        console.log("Fetching categories");
        this.cSrv.getAllCategories().subscribe( {
          next: (categories: categoryModel[]) => {
            this.categories = categories;
            this.categoryOptions = this.categories.map(cat => ({
              label: cat.name,
              value: cat.id
            }));
         setTimeout(() => {
           this.cdr.detectChanges();  
           }, 0);
          },
          error: (err) => {
            console.log('Error fetching categories:', err);
          }
        });
      }
      
      
      closeDialog() {
        this.displayDialog = false;
        this.resetForm();
      }

      resetForm() {
        this.newGiftName = "";
        this.newGiftDescription = "";
        this.newGiftCost = 0;
        this.newGiftPicture = "";
        this.newGiftCategoryId = 0;
        this.selectedCategory = 0;
      }

       deleteGift (id:number){
          const headers = this.getHeaders(); 
         console.log("headers",headers) 
         this.giftSrv.deleteGift(id, headers).subscribe({
          next: (response: boolean) => {
            console.log("Deleted gift with id:",id);
            this.gifts = this.gifts.filter(gift => gift.id !== id);
            this.ngOnInit();
          },
          error: (err) => {
            console.log('Delete error:', err);
             this.messageService.add({ severity: 'error', summary: 'Error', detail: 'מחיקת המתנה נכשלה' });
          }  
        })  
      }

      openUpdateGiftDialog(item:GiftModel) {
       this.id = item.id;
       this.newGiftName = item.name;
       this.newGiftDescription = item.description || "";
       this.newGiftCost = item.cost;
       this.newGiftPicture = item.picture || "";
       this.newGiftCategoryId = item.categoryId;
       this.selectedCategory = Number(item.categoryId);
       this.donorId = item.donorId;
       this.winnerName = item.winnerName || "";
       console.log("categoies",this.categories)
       this.displayDialog = true;
     }

     updateGiftDetails() {
        const updatedGift: GiftModel = {
          id: this.id,
          name: this.newGiftName,
          description: this.newGiftDescription,
          cost: this.newGiftCost,
          picture: this.newGiftPicture,
          categoryId: this.selectedCategory,
          
          donorId: this.donorId,
          winnerName: this.winnerName
        };
      console.log("categoryid",this.categoryId)
      try{
        const headers = this.getHeaders(); 
        console.log("headers",headers) 
        this.giftSrv.updateGift(updatedGift, headers).subscribe({
        next: (response: GiftModel) => {
          console.log("Updated gift:", response);
          this.gifts = this.gifts.map(gift => gift.id === response.id ? response : gift);
          this.closeDialog();
          this.ngOnInit();
          // this.cdr.detectChanges();
          // setTimeout(() => {
          //   this.closeDialog();
          // }, 100);
        },
        error: (err) => {
          console.log('Error updating gift:', err);
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'עדכון המתנה נכשל' });
        }
        })
      }catch (error) {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'עדכון המתנה נכשל' });
        }
      }
    
   filterGifts() {
      console.log('Filtering gifts with:');
      console.log('searchByGiftName:', this.searchByGiftName);
      console.log('selectedDonor:', this.selectedDonor?.id);
      console.log('selectedDonor:', this.selectedDonor);
      console.log('All gifts:', this.gifts);
      
      this.filteredGifts = this.gifts.filter(gift => {
        // Filter by donor
        let donorMatch = true;
        console.log(`Checking gift ${gift.name} with donorId ${gift.donorId} against selected donor Id ${this.selectedDonor?.id}`);
        if (this.selectedDonor && this.selectedDonor.id) {
          console.log(`Checking gift ${gift.name} with donorId ${gift.donorId} against selected donor Id ${this.selectedDonor.id}`);
          alert("aaa!!!selectedDonor.Id,"+ this.selectedDonor.id + "selectedDonor.Name,"+ this.selectedDonor.Name)
          donorMatch = gift.donorId === this.selectedDonor.id;
          console.log(`Checking gift ${gift.name}: donorId=${gift.donorId}, selectedDonor.Id=${this.selectedDonor.id}, match=${donorMatch}`);
        }
        // Filter by gift name
        const nameMatch = gift.name.toLowerCase().includes(this.searchByGiftName.toLowerCase());
        
        
        
        return nameMatch && donorMatch;
      });
      
      console.log('Filtered gifts result:', this.filteredGifts);
      this.cdr.markForCheck();
    }
    

    // Called when gift name search input changes
    onGiftNameChange() {
      this.filterGifts();
    }
    viewDonorGifts(donor: DonorModel | null = null) {
      console.log(donor);
      
    if (!donor || !donor.id) return;
    const headers = this.getHeaders(); 
    this.donorSrv.getDonorGifts(donor.id,headers).subscribe({
      next: (gifts) => {
        console.log(gifts);
      
        this.gifts = gifts || [];
        // פתיחת הדיאלוג אחרי שהנתונים התקבלו
        setTimeout(() => this.cdr.markForCheck(), 0);
      },
    error: (err) => {
      console.error('שגיאה בקבלת מתנות לתורם', err);
      alert('שגיאה בקבלת מתנות לתורם: ' + (err.error?.message || err.message));
    }
  });
}

    // Called when donor selection changes
    onDonorChange() {
      console.log('Donor changed:', this.selectedDonor);
      alert("donor   ,"+this.selectedDonor)
      this.viewDonorGifts(this.selectedDonor);
    }

    // Clear all filters
    clearFilters() {
      this.searchByGiftName = "";
      this.selectedDonor = null;
      this.filteredGifts = this.gifts;
      this.cdr.detectChanges();
    }



    getDonorName(donorId: number): string {
      console.log(`getDonorName called with donorId: ${donorId} (type: ${typeof donorId})`);
      const donor = this.donors.find(d => {
        console.log(`Comparing: d.Id=${d.id} (type: ${typeof d.id}) with donorId=${donorId}`);
        return d.id === donorId;
      });
      const result = donor ? donor.Name : 'Unknown';
      console.log(`getDonorName result: ${result}`);
      return result;
    }

    // Get image with fallback
    getImageUrl(picture: string | undefined): string {
      return picture || 'assets/no-image.png';
    }

    // Format currency for display
    formatCost(cost: number): string {
      return new Intl.NumberFormat('he-IL', {
        style: 'currency',
        currency: 'ILS'
      }).format(cost);
    }
    // Additional methods for editing gifts can be added here
    addNewGift() {
      if (!this.newGiftname.trim()  || !this.newGiftcost ||!this.newGiftcategoryId || !this.newGiftdonorId) {
    alert('אנא מלא את כל השדות');
    return;
  }
    // create payload shaped for the API (lowercase keys)
    //יצירת אובייקט מסוג donormodel עם הנתונים מהטופס
    const payload = new GiftModel();
    payload.name = this.newGiftname.trim();
    payload.description = this.newGiftdescription || '';
    payload.cost = this.newGiftcost;
    payload.picture = this.newGiftpicture || '';
    payload.categoryId = this.newGiftcategoryId.id;  
    payload.donorId = this.newGiftdonorId.id; 
     console.log('האם התורם חוקי', payload.donorId); 
    console.log('שולח יצירת מתנה עם הנתונים:', JSON.stringify(payload));
    const headers = this.getHeaders(); 
    this.giftSrv.createGift(payload,headers).subscribe({
      next: (response) => {
        console.log('מתנה חדשה נוצרה בהצלחה:', response);
        // normalize response and add to list
        const converted = new GiftModel();
        const r: any = response;
        converted.id = r.id || r.Id;
        converted.name = r.name || r.Name;
        converted.description = r.description || r.Description;
        converted.cost = r.cost || r.Cost;
        converted.picture = r.picture || r.Picture;
        converted.categoryId = r.categoryId || r.CategoryId;
        converted.donorId = r.donorId || r.DonorId;
        // עדכון ה-UI מיידית
        this.gifts = [...this.gifts, converted];
        // סגור הדיאלוג לאחר עיגון שינוי (הימנעות מבעיות בדיקה)
        setTimeout(() => {
          this.closeDialog2();
          this.cdr.markForCheck();
        }, 0);
      },
      error: (err) => {
        console.log('שגיאה בהוספת מתנה:', err);
        setTimeout(() => this.closeDialog(), 0);
        alert('שגיאה בהוספת מתנה: ' + (err.error?.message || err.message));
      }
    });
    }

    showAddGiftDialog() {
      this.displayDialog2 = true;
    }
    closeDialog2() {
      this.displayDialog2 = false;
    }

    editGift(gift: GiftModel) {
      // Logic to edit the selected gift
    }

  onSelectFiles(event: any) {
    const file = event.files[0]; // מקבל את הקובץ שנבחר
    if (file) {
      this.newGiftPicture = URL.createObjectURL(file); // יוצר URL זמני לתמונה
      console.log("Chosen picture URL:", this.newGiftPicture);
    }
  }

  // פונקציה שמפעילה את חלון הבחירה כאשר נלחץ על כפתור הבחירה
  choosePicture(fileUpload: any) {
    fileUpload.choose();
  }
  
}
