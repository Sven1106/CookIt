import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-signIn',
  templateUrl: './signIn.component.html',
  styleUrls: ['../home.page.scss', './signIn.component.scss']
})
export class SignInComponent implements OnInit {

  signInForm: FormGroup;
  title: string = "";
  @Output() signInFormEmitter = new EventEmitter<FormGroup>();
  @Output() changeSubPageEmitter = new EventEmitter<number>();
  @Output() changeTitleEmitter = new EventEmitter<string>()
  @Input() signedIn: boolean; // TODO Why doesn't it work?
  constructor(
    private formBuilder: FormBuilder
  ) {
    this.createSignInForm();
  }
  ngOnInit() {
    this.changeTitleEmitter.emit(this.title);
  }
  createSignInForm() {
    this.signInForm = this.formBuilder.group({
      email: new FormControl('', [
        Validators.required,
        Validators.pattern('^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$') // correct way to check if email is valid
      ]),
      password: new FormControl('', Validators.required)
    });
  }
}
