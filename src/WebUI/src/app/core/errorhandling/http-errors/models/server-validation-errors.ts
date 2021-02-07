export interface ServerValidationErrors {
    
    title: string;

    // Key = propertyName, and value of string[] is list of errors pertaining to given property.
    errors: Map<string, string[]>;
}