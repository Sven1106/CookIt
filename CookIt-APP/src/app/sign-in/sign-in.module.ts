import { RecipeComponent } from './../recipe/recipe.component';
import { SharedFormsModule } from '../_shared/sharedForms.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { SignInPageRoutingModule } from './sign-in-routing.module';
import { SignInPage } from './sign-in.page';

@NgModule({
  imports: [
    CommonModule,
    IonicModule,
    SignInPageRoutingModule,
    SharedFormsModule,
  ],
  providers: [],
  declarations: [SignInPage, RecipeComponent]
})
export class SignInPageModule { }
