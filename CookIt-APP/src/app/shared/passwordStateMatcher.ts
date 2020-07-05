import { Injectable } from '@angular/core';
import { FormGroupDirective, NgForm, FormControl } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core';
@Injectable({ providedIn: 'root' })
export class PasswordStateMatcher implements ErrorStateMatcher {
    isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
        const invalidCtrl = !!(control && control.invalid && control.parent.dirty);
        const invalidParent = !!(control && control.parent && control.parent.invalid && control.parent.dirty);
        return control.parent.errors && control.parent.errors && control.touched && (invalidCtrl || invalidParent);
    }
}