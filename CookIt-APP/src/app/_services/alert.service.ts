import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig, MatSnackBarRef } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class AlertService {

  allSnackBarRefs: MatSnackBarRef<any>[] = [];
  constructor(private snackbar: MatSnackBar) { }

  success(message: string, durationInMs: number) {
    let snackBarRef = this.snackbar.open(message, null, {
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      panelClass: ['snackBarSuccess'],
      duration: durationInMs
    });
    this.allSnackBarRefs.push(snackBarRef);
  }

  error(message: string) {
    let snackBarRef = this.snackbar.open(message, 'luk', {
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
  dismissAll() {
    this.snackbar.dismiss()
    this.allSnackBarRefs.forEach(snackbarRef => {
      snackbarRef.dismiss();
    });
  }

}
