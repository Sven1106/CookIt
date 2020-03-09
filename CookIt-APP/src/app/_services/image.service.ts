import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { ImageRequest } from 'src/app/_models/imageRequest';
import { AuthService } from 'src/app/_services/auth/auth.service';
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
