import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class AlertService {

  constructor(private snackbar: MatSnackBar) { }

  success(message: string) {
    let snackBarRef = this.snackbar.open(message, 'luk', {
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      panelClass: ['snackBarSuccess']
    });
  }

  error(message: string) {
    let snackBarRef =  this.snackbar.open(message, 'luk', {
        horizontalPosition: 'center',
        verticalPosition: 'bottom',
        panelClass: ['snackBarError']
      });
  }

  // warning(message: string) {
  //   alertify.warning(message);
  // }

  // message(message: string) {
  //   alertify.message(message);
  // }
}
