import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { PasswordStateMatcher } from 'src/app/_shared/passwordStateMatcher';

@Component({
  selector: 'app-signUp',
  templateUrl: './signUp.component.html',
  styleUrls: ['../home.page.scss', './signUp.component.scss']
})
export class SignUpComponent implements OnInit {
  signUpForm: FormGroup;
  title: string = "Opret bruger";
  @Output() signUpFormEmitter = new EventEmitter<FormGroup>();
  @Output() changeSubPageEmitter = new EventEmitter<number>();
  @Output() changeTitleEmitter = new EventEmitter<string>();
  @Input() signedUp: boolean; // TODO Why doesn't it work?
  matcher = new PasswordStateMatcher();

  constructor(
    private formBuilder: FormBuilder
  ) {
    this.createSignUpForm();
  }

  ngOnInit() {
    this.changeTitleEmitter.emit(this.title);
  }
  createSignUpForm() {
    this.signUpForm = this.formBuilder.group({
      name: new FormControl('', Validators.required),
      email: new FormControl('', [
        Validators.required,
        Validators.pattern('^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$')
      ]),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(4)
      ]),
      confirmPassword: new FormControl('')
    },
      { validator: this.passwordMatchValidator });
  }
  passwordMatchValidator(formGroup: FormGroup) {
    let password = formGroup.get('password').value;
    let confirmPassword = formGroup.get('confirmPassword').value;
    return password === confirmPassword ? null : { mismatch: true };
  }
}
