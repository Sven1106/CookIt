import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';
import { Observable } from 'rxjs';
import { ImageRequest } from '@core/models/imageRequest';
import { AuthService } from '@core/services/auth/auth.service';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ImageService {
  baseUrl = environment.apiDomain + 'api/';
  constructor(
    private httpClient: HttpClient
  ) { }


  getImage(imageRequest: ImageRequest): Observable<string> {
    return this.httpClient.post(this.baseUrl + 'image/getImage', imageRequest, { responseType: 'text' });
  }

}
