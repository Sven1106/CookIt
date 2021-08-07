
import { NgModule } from '@angular/core';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { HomePageRoutingModule } from './home-routing.module';
import { SharedModule } from '@shared/shared.module';
import { HomePage } from './pages/home/home.page';
import { SignInComponent } from './components/signIn/signIn.component';
import { SignUpComponent } from './components/signUp/signUp.component';
import { ForgottenPasswordComponent } from './components/forgottenPassword/forgottenPassword.component';
@NgModule({
  declarations: [
    HomePage,
    SignInComponent,
    SignUpComponent,
    ForgottenPasswordComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    HomePageRoutingModule,
    SharedModule
  ],
  providers: [
  ]
})
export class HomeModule { }
