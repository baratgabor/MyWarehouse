import { Component, Input } from '@angular/core';
import { AbstractControlDirective, AbstractControl } from '@angular/forms';

// This component is used to automatically list all known validation errors
// pertaining to a given form control.

@Component({
  selector: 'app-show-validation-errors',
  template: `
    <span class="is-invalid"></span> <!-- Bootstrap's .invalid-feedback relies on .is-invalid sibling. https://github.com/angular/angular/issues/18877 -->
    <div *ngIf="showList()" class="invalid-feedback" [ngClass]="extraCssClasses">
      <div *ngFor="let message of listOfErrors()" style="white-space: pre;">{{message}}</div>
    </div>
  `,
})
export class ShowValidationErrorsComponent {

 private static readonly errorMessages = {
    'required': () => 'This field is required.',
    'minlength': (params) => `Value must be at least' ${params.requiredLength} characters long.`,
    'maxlength': (params) => `Value must be shorter than ${params.requiredLength} characters.`,
    'pattern': (params) => 'The required pattern is: ' + params.requiredPattern
 };

 @Input() private control: AbstractControlDirective | AbstractControl;
 @Input() private shouldShow: boolean = true;
 @Input() extraCssClasses: string = '';

  showList(): boolean {
    return this.control &&
      this.control.errors &&
      this.shouldShow;
  }

  listOfErrors(): string[] {

    let errors: string[] = [];

    for (let field of Object.keys(this.control.errors)) {

      // Supports two scenario:
      // 1) If we have a defined message for a built-in validator.
      // 2) If the error object contains a 'message' property.
      if (field in ShowValidationErrorsComponent.errorMessages) {
        errors.push(ShowValidationErrorsComponent.errorMessages[field](this.control.errors[field]));
      }
      else if ('message' in this.control.errors[field]) {
        errors.push(this.control.errors[field].message);
      }
    }

    return errors;
  }
}