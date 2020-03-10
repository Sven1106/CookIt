import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { RecipeService } from 'src/app/_services/recipe.service';
import { AlertService } from 'src/app/_services/alert.service';
import { AuthService } from 'src/app/_services/auth/auth.service';


@Component({
  selector: 'app-home',
  templateUrl: './home.page.html',
  styleUrls: ['./home.page.scss'],
})
export class HomePage implements OnInit {

  signingIn: boolean;
  signingUp: boolean;
  currentSubPage: number;
  title: string;

  constructor(
    private router: Router,
    private authService: AuthService,
    private alertService: AlertService,
    private recipeService: RecipeService
  ) {

  }

  ionViewWillEnter() {
    this.signingIn = false;
    this.signingUp = false;

    this.signOut();
    this.changeSubPage(0);
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
    this.signingIn = true;
    console.log(signInForm);
    this.authService.login(signInForm.value)
      .subscribe(
        {
          next: (result: any) => {
            // console.log("next");
            this.router.navigate(['/user']);
          },
          error: (error: any) => {
            // console.error(error);
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
    this.signingIn = false;
  }

  signUp(signUpForm: FormGroup) {
    this.signingUp = true;
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
    this.signingUp = false;

  }

  forgottenPassword(forgottenPasswordForm: FormGroup) {
    setTimeout(() => {
      this.alertService.success('Du vil inden for kort tid, modtage en e-mail med dit nye password', 3000);
    }, 1000);
  }
  signOut() {
    this.authService.removeDecodedToken();
    this.recipeService.removeKitchenCupboardInStorage();
  }
}
