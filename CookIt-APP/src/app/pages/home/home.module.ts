import { ForgottenPasswordComponent } from './forgottenPassword/forgottenPassword.component';
import { SignUpComponent } from './signUp/signUp.component';
import { SignInComponent } from './../home/SignIn/SignIn.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { HomePageRoutingModule } from './home-routing.module';
import { HomePage } from './home.page';
import { SharedFormsModule } from '../../_shared/sharedForms.module';

@NgModule({
  declarations: [
    HomePage,
    SignInComponent,
    SignUpComponent,
    ForgottenPasswordComponent
  ],
  imports: [
    CommonModule,
    IonicModule,
    HomePageRoutingModule,
    SharedFormsModule,

  ],
  providers: [
  ]
})
export class SignInPageModule { }
