import { ValidationErrors } from "@angular/forms";

// Defines a stronger typed validation error model, compatible with the Angular default.
export interface ValidationError extends ValidationErrors {
    [validatorName: string]: {
        message: string
    }
}