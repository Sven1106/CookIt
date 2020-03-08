import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { Storage } from '@ionic/storage';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiDomain + 'api/auth/';
  jwtHelperService = new JwtHelperService();
  decodedToken: any;
  constructor(
    private httpClient: HttpClient,
    private storage: Storage
  ) { }

  login(model: any) {
    return this.httpClient.post(this.baseUrl + 'login', model)
      .pipe(
        map((response: any) => {
          let user = response;
          if (user) {
            this.setDecodedToken(user.token);
          }
        })
      );
  }

  register(model: any) {
    return this.httpClient.post(this.baseUrl + 'register', model)
      .pipe(
        map((response: any) => {
          let user = response;
          if (user) {
            this.setDecodedToken(user.token);
          }
        })
      );
  }

  async isLoggedIn() {
    let token = '';
    await this.storage.get('token').then((value) => {
      token = value;
    });
    if (token == null) {
      return false;
    }
    return !this.jwtHelperService.isTokenExpired(token);
  }

  async getDecodedToken() {
    let token = '';
    await this.storage.get('token').then((value) => {
      token = value;
    });
    if (token) {
      this.decodedToken = this.jwtHelperService.decodeToken(token);
    }
  }

  async setDecodedToken(token: string) {
    await this.storage.set('token', token);
    this.decodedToken = this.jwtHelperService.decodeToken(token);
  }

  async removeDecodedToken() {
    await this.storage.remove('token');
    this.decodedToken = null;
  }
}
