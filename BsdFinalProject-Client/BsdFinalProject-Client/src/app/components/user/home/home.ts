

import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { RippleModule } from 'primeng/ripple';
import { SelectItem, MessageService } from 'primeng/api';
import { DataViewModule } from 'primeng/dataview';
import { SelectButtonModule } from 'primeng/selectbutton';
import { PanelModule } from 'primeng/panel';
import { GiftModel } from '../../../Models/gift';
import { GiftService } from '../../../Services/gift-service';
import { DonorService } from '../../../Services/donor-service';
import { DonorModel } from '../../../Models/donor';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { jwtDecode } from 'jwt-decode';
import { DialogModule } from 'primeng/dialog';
import { categoryModel } from '../../../Models/category';
import { CategoryService } from '../../../Services/category-service';
import { error, log } from 'console';
import { Drawer, DrawerModule } from 'primeng/drawer';


interface UploadEvent {
    originalEvent: Event;
    files: File[];
}
@Component({
  selector: 'app-user-home',
  imports: [DataViewModule, DialogModule, SelectButtonModule, TagModule, ButtonModule, FormsModule, CommonModule, SelectModule, InputTextModule, PanelModule,DrawerModule],
  providers: [MessageService],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home {
  constructor(private cdr: ChangeDetectorRef) { }
  visible1: boolean = false;
  visible2: boolean = false;
  visible3: boolean = false;
  visible4: boolean = false;
  gifts: GiftModel[] = [];
  filteredGifts: GiftModel[] = [];
  giftSrv: GiftService = inject(GiftService);
  donorSrv: DonorService = inject(DonorService);
  donors: DonorModel[] = [];
  donorsName: string[] = [];
  id: number = 0;
  name: string = "";
  description: string = "";
  cost: Number = 0;
  picture: string = "";
  categoryId: Number = 0;
  donorId: number = 0;
  winnerName: string = "";
  displayDialog: boolean = false;
  newGiftid!: number;
  newGiftname!: string;
  newGiftdescription?: string;
  newGiftcost!: number;
  newGiftpicture?: string;
  newGiftcategoryId!: number;
  newGiftdonorId!: number;
  headers: HttpHeaders = new HttpHeaders();
  categories: categoryModel[] = [];
  cSrv:CategoryService=inject(CategoryService) 
  layout: 'list' | 'grid' = 'list';
  selectedDonor: DonorModel | null = null;
  searchByGiftName: string = "";
  displayUpdateDialog: boolean = false;
  updateGiftName: string = "";
  updateGiftDescription: string = "";
  updateGiftCost: number = 0;
  updateGiftPicture: string = "";
  updateGiftCategoryId: Number = 0;
  categoryOptions: SelectItem[] = [];
  selectedCategory: number = 0;
  // messageService: MessageService = inject(MessageService);

  options: SelectItem[] = [
    { label: 'List', value: 'list' },
    { label: 'Grid', value: 'grid' }
  ];

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
                    try { 
                    const headers=this.getHeaders();
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
                    // this.cSrv.markForCheck();
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

  filterGifts() {
    console.log('Filtering gifts with:');
    console.log('searchByGiftName:', this.searchByGiftName);
    console.log('selectedDonor:', this.selectedDonor?.id);
    console.log('selectedDonor:', this.selectedDonor);
    console.log('All gifts:', this.gifts);

    this.filteredGifts = this.gifts.filter(gift => {

      // Filter by gift name
      const nameMatch = gift.name.toLowerCase().includes(this.searchByGiftName.toLowerCase());

      return nameMatch;
    });

    console.log('Filtered gifts result:', this.filteredGifts);
    this.cdr.markForCheck();
  }

  // Called when gift name search input changes
  onGiftNameChange() {
    this.filterGifts();
  }

  // Called when donor selection changes

  // Clear all filters
  clearFilters() {
    this.searchByGiftName = "";
    this.selectedDonor = null;
    this.filteredGifts = this.gifts;
    this.cdr.detectChanges();
  }

  // Helper function to get donor name by ID
  getDonorName(donorId: number): string {
    console.log(`getDonorName called with donorId: ${donorId} (type: ${typeof donorId})`);
    const donor = this.donors.find(d => {
      console.log(`Comparing: d.id=${d.id} (type: ${typeof d.id}) with donorId=${donorId}`);
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

}




