import { AbstractControl, ValidatorFn, FormArray } from '@angular/forms';
import { ValidationError } from 'app/core/errorhandling/form-validation/model/validation-error';
import { TransactionType } from '../../../common/models/transaction-type';
import { TransactionFormConstants as Constants } from '../constants/transactionFormConstants';

// Validates transaction lines in transaction creation form.
export function transactionLinesValidator(transactionType: TransactionType): ValidatorFn {
  return (c: FormArray): { [key: string]: any } | null => {

    const lines = c.controls;

    if (!lines || lines.length == 0)
      return error('  At least one item is required.');

    let errorAggregate: string = '';
    lines.forEach((line, index) => {

      const id = line.get(Constants.Line_ProductId).value;
      if (isNaN(id) || id <= 0)
        errorAggregate += `  Line ${index+1} – Product ID is missing.\n`;

      const qty = line.get(Constants.Line_ProductQuantity).value;
      if (isNaN(qty) || qty <= 0)
        errorAggregate += `  Line ${index+1} – Product quantity needs to be minimum one.\n`;

      const numberInStock = line.get(Constants.Line_ProductStock).value;
      if (transactionType == TransactionType.Sale && qty > numberInStock)
        errorAggregate += `  Line ${index+1} – Cannot deduct ${qty} units of '${line.get(Constants.Line_ProductName).value}'. Current stock is ${numberInStock}.\n`;
    
    });

    if (errorAggregate)
    {
      return error(errorAggregate.slice(0, -1)); // Slice last '\n' control char
    }

    return null;

    function error(message: string): ValidationError {
      return {
        'transactionLinesValidator': {
          message: 'Transaction lines are not valid:\n' + message
        }
      }};
  }
}
