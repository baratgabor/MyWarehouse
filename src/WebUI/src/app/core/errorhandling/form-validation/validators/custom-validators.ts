import { FormArray, FormControl, FormGroup, ValidatorFn } from '@angular/forms';
import { ValidationError } from '../model/validation-error';

export class CustomValidators {

  // Watch out not to name these exactly like built-in validators.
  // Otherwise the error display component might pick up a default error message.
  static requiredField(customMessage: string = ''): ValidatorFn {
    return (c: FormControl): ValidationError | null => {

      if (this.isEmpty(c.value))
        return this.makeError('requiredField', customMessage ?? 'This field is required.');

      return null;
    };
  }

  static requiredMinNumber(min: number): ValidatorFn {
    return (c: FormControl): ValidationError | null => {

      if (this.isEmpty(c.value))
        return this.makeError('requiredMinNumber', `You must provide a number of at least ${min}.`);
      else if (!this.isNumber(c.value))
        return this.makeError('requiredMinNumber', `Value must be a number, and minimum ${min}.`);
      else if (c.value < min)
        return this.makeError('requiredMinNumber', `The minimum accepted value is ${min}.`);

      return null;
    };
  }

  static requiredMaxNumber(max: number): ValidatorFn {
    return (c: FormControl): ValidationError | null => {

      if (this.isEmpty(c.value))
        return this.makeError('requiredMaxNumber', `You must provide a number (maximum ${max}).`);
      else if (!this.isNumber(c.value))
        return this.makeError('requiredMaxNumber', `Value must be a number (maximum ${max}).`);
      else if (c.value < max)
        return this.makeError('requiredMaxNumber', `The maximum accepted value is ${max}.`);

      return null;
    };
  }

  static requiredPositiveNumber(c: FormControl): ValidatorFn {
    return (c: FormControl): ValidationError | null => {

      if (this.isEmpty(c.value))
        return this.makeError('requiredPositiveNumber', 'You must provide a positive number.');
      else if (!this.isNumber(c.value))
        return this.makeError('requiredPositiveNumber', 'Value must be a positive number.');
      else if (!(c.value > 0))
        return this.makeError('requiredPositiveNumber', `The number must be positive.`);

      return null;
    };
  }

  static requiredWholeNumber(c: FormControl): ValidatorFn {
    return (c: FormControl): ValidationError | null => {

      if (this.isEmpty(c.value))
        return this.makeError('requiredWholeNumber', 'You must provide a whole number.');
      else if (!this.isNumber(c.value))
        return this.makeError('requiredWholeNumber', 'Value must be a whole number.');
      else if (!Number.isInteger(c.value))
        return this.makeError('requiredWholeNumber', `The number must be whole, not fractional.`);

      return null;
    };
  }

 static telephoneNumber(c: FormControl): ValidationError | null {
   const isValidPhoneNumber = /^\d{3,3}-\d{3,3}-\d{3,3}$/.test(c.value);
   const message = {
     'telephoneNumber': {
       'message': 'The phone number must be valid (XXX-XXX-XXX, where X is a digit)'
     }
   };
   return isValidPhoneNumber ? null : message;
 }

  // Template for later use.
  // static uniqueName(c: FormControl): Promise<ValidationError> {
  //   const message = {
  //     'uniqueName': {
  //       'message': 'The name is not unique'
  //     }
  //   };
  //
  //   return new Promise(resolve => {
  //     setTimeout(() => {
  //       resolve(c.value === 'Existing' ? message : null);
  //     }, 1000);
  //   });
  // }

  private static makeError(validatorName: string, message: string): ValidationError {

    let error: ValidationError = {
      [validatorName]: {
        message: message
      }
    }

    return error;
  }

  private static isEmpty(value: any): boolean {
    return value == null || value === ''
  }

  private static isNumber(value: any): boolean {
    return (typeof value === 'number' && !isNaN(value))
      || ((typeof value === 'string') && value.trim() != '' && !isNaN(Number(value)))
  }

  private static isString(value: any): boolean {
    return (typeof value == 'string' || value instanceof String);
  }
}