import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../_services/auth/auth.service';

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate {
    constructor(private authService: AuthService, private router: Router) { }

    async canActivate(): Promise<boolean> {
        let isLoggedIn: boolean;
        await this.authService.isLoggedIn().then((value) => {
            isLoggedIn = value;
        });
        if (isLoggedIn) {
            return true;
        }
        this.router.navigate(['/home']);
        return false;
    }
}