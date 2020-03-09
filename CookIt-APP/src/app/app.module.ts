import {
  IonicModule,
  IonicRouteStrategy
} from '@ionic/angular';
import { IonicStorageModule } from '@ionic/storage';
import { SplashScreen } from '@ionic-native/splash-screen/ngx';
import { StatusBar } from '@ionic-native/status-bar/ngx';
import { Network } from '@ionic-native/network/ngx';
import { NativePageTransitions } from '@ionic-native/native-page-transitions/ngx';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http'
import { BrowserModule } from '@angular/platform-browser';
import { RouteReuseStrategy } from '@angular/router';
import { JwtModule } from '@auth0/angular-jwt';
import {
  FormsModule,
  ReactiveFormsModule
} from '@angular/forms';
import {
  MatInputModule, MatFormFieldModule, MatButtonModule,
  MatSnackBarModule, MatIconModule, MatAutocompleteModule,
  MatChipsModule, MatSliderModule, MatProgressBarModule, MatDialogModule
} from '@angular/material';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { PasswordStateMatcher } from 'src/app/_shared/passwordStateMatcher';
import { AuthService } from './_services/auth/auth.service';
import { AlertService } from './_services/alert.service';
import { NetworkService } from './_services/network.service';
import { RecipeService } from './_services/recipe.service';

export function tokenGetter() {

  return localStorage.getItem('tokenForJwtModuleHttpRequest');
}
@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserModule,
    IonicModule.forRoot(),
    IonicStorageModule.forRoot(),
    AppRoutingModule,
    HttpClientModule,
    JwtModule.forRoot({
      config: {
        tokenGetter,
        whitelistedDomains: ['localhost:49615'], // auto sends JWT to these domains
        blacklistedRoutes: ['localhost:49615/api/auth'] // doesnt auto sends JWT to these domains
      }
    }),
    MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    MatSnackBarModule,
    MatIconModule,
    MatAutocompleteModule,
    MatChipsModule,
    MatSliderModule,
    MatProgressBarModule,
    MatDialogModule

  ],
  providers: [
    StatusBar,
    SplashScreen,
    { provide: RouteReuseStrategy, useClass: IonicRouteStrategy },
    NetworkService,
    Network,
    PasswordStateMatcher,
    AuthService,
    AlertService,
    RecipeService,
    NativePageTransitions
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
