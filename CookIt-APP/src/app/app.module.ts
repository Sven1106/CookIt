import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { RouteReuseStrategy } from '@angular/router';
import { JwtModule } from '@auth0/angular-jwt';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule, IonicRouteStrategy } from '@ionic/angular';
import { IonicStorageModule } from '@ionic/storage';
import { SplashScreen } from '@ionic-native/splash-screen/ngx';
import { StatusBar } from '@ionic-native/status-bar/ngx';
import { Network } from '@ionic-native/network/ngx';
import { NativePageTransitions } from '@ionic-native/native-page-transitions/ngx';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthService } from '@core/services/auth/auth.service';
import { AlertService } from '@core/services/alert/alert.service';
import { NetworkService } from '@core/services/network/network.service';
import { RecipeService } from '@core/services/recipe/recipe.service';
import { environment } from '@environments/environment';
import { PasswordStateMatcher } from '@shared/passwordStateMatcher';
import { SharedModule } from '@shared/shared.module';

export function tokenGetter() {

  return localStorage.getItem('tokenForJwtModuleHttpRequest');
}

export function extractHostname(url) {
  let hostname;
  //find & remove protocol (http, ftp, etc.) and get hostname

  if (url.indexOf('//') > -1) {
    hostname = url.split('/')[2];
  }
  else {
    hostname = url.split('/')[0];
  }

  //find & remove '?'
  hostname = hostname.split('?')[0];
  return hostname;
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
        whitelistedDomains: [extractHostname(environment.apiDomain)], // auto sends JWT to these domains
        blacklistedRoutes: [extractHostname(environment.apiDomain) + '/api/auth'] // doesnt auto sends JWT to these domains
      }
    }),
    SharedModule

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
