import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AlertService } from '../../_services/alert.service';
import { AuthService } from '../../_services/auth/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.page.html',
  styleUrls: ['./home.page.scss'],
})
export class HomePage implements OnInit {


  currentSubPage: number;
  title: string;

  constructor(
    private router: Router,
    private authService: AuthService,
    private alertService: AlertService,
    private ref: ChangeDetectorRef

  ) {
    this.changeSubPage(0);
  }

  ionViewWillEnter() {
    this.signOut();

  }
  ngOnInit() {

  }
  changeSubPage(index: number) {
    this.currentSubPage = index;
  }
  changeTitle(newTitle: string) {
    this.title = newTitle;
  }

  signIn(signInForm: FormGroup) {
    this.authService.login(signInForm.value)
      .subscribe(
        {
          next: (result: any) => {
            // console.log("next");
            this.router.navigate(['/user']);
          },
          error: (error: any) => {
            // console.log(error);
            if (error instanceof HttpErrorResponse) {
              switch (error.status) {
                case 400:
                  this.alertService.error('E-mail eller Password er forkert');
                  break;
                case 0:
                  this.alertService.error('Ingen forbindelse til serveren');
                  break;
                default:
                  const applicationError = error.headers.get('Application-Error');
                  if (applicationError) {
                    this.alertService.error(applicationError);
                  }
                  else {
                    this.alertService.error('Der opstod en fejl');
                  }
                  break;
              }
            }
          },
          complete: () => {
            // console.log("complete");
          }
        }
      );
  }

  signUp(signUpForm: FormGroup) {
    this.authService.register(signUpForm.value)
      .subscribe(
        {
          next: (result: any) => {
            // console.log("next"); 
            this.router.navigate(['/user']);
          },
          error: (error: any) => {
            // console.log(error);
            if (error instanceof HttpErrorResponse) {
              switch (error.status) {
                case 400:
                  this.alertService.error('E-mail eksisterer allerede');
                  break;
                case 0:
                  this.alertService.error('Ingen forbindelse til serveren');
                  break;
                default:
                  const applicationError = error.headers.get('Application-Error');
                  if (applicationError) {
                    this.alertService.error(applicationError);
                  }
                  else {
                    this.alertService.error('Der opstod en fejl');
                  }
                  break;
              }
            }
          },
          complete: () => {
            // console.log("complete");
          }
        }
      );
  }

  forgottenPassword(forgottenPasswordForm: FormGroup) {
    setTimeout(() => {
      this.alertService.success('Du vil inden for kort tid, modtage en e-mail med dit nye password');
    }, 1000);
  }
  signOut() {
    this.authService.removeDecodedToken();
  }
}
