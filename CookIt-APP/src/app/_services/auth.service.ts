import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiDomain + 'api/auth/';
  jwtHelperService = new JwtHelperService();
  decodedToken: any;
  constructor(private httpClient: HttpClient) { }

  login(model: any) {
    return this.httpClient.post(this.baseUrl + 'login', model)
      .pipe(
        map((response: any) => {
          const user = response;
          if (user) {
            this.setDecodedToken(user.token);
          }
        })
      )
  }
  register(model: any) {
    return this.httpClient.post(this.baseUrl + 'register', model);
  }

  isLoggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelperService.isTokenExpired(token);
  }

  getDecodedToken() {
    const token = localStorage.getItem('token');
    if (token) {
      this.decodedToken = this.jwtHelperService.decodeToken(token);
    }
  }
  setDecodedToken(token: string) {
    localStorage.setItem('token', token);
    this.decodedToken = this.jwtHelperService.decodeToken(token);
  }
  
}
