/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { KitchenCupboardComponent } from './kitchenCupboard.component';

describe('KitchenCupboardComponent', () => {
  let component: KitchenCupboardComponent;
  let fixture: ComponentFixture<KitchenCupboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ KitchenCupboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(KitchenCupboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
