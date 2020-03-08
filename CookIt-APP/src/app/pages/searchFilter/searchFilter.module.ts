import { IonicModule } from '@ionic/angular';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatSliderModule } from '@angular/material';


import { SearchFilterPageRoutingModule } from './searchFilter-routing.module';

import { SearchFilterPage } from './searchFilter.page';

import { SharedFormsModule } from 'src/app/_shared/sharedForms.module';


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    SearchFilterPageRoutingModule,
    SharedFormsModule
  ],
  declarations: [SearchFilterPage]
})
export class SearchFilterPageModule { }
