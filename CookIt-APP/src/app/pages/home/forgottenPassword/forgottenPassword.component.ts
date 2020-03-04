import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-forgottenPassword',
  templateUrl: './forgottenPassword.component.html',
  styleUrls: ['./forgottenPassword.component.scss']
})
export class ForgottenPasswordComponent implements OnInit {

  forgottenPasswordForm: FormGroup;
  title: string = "Glemt password";
  @Output() forgottenPasswordFormEmitter = new EventEmitter<FormGroup>();
  @Output() changeTitleEmitter = new EventEmitter<string>()
  constructor(
    private formBuilder: FormBuilder
  ) {
    this.createForgottenPasswordForm();
  }
  ngOnInit() {
    this.changeTitleEmitter.emit(this.title);
  }
  createForgottenPasswordForm() {
    this.forgottenPasswordForm = this.formBuilder.group({
      email: new FormControl('', [Validators.required, Validators.pattern('^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$')])
    });
  }
}
