export interface PartnerListing {
  id: number;
  name: string;

  // Entire address, formatted.
  address: string;

  country: string;
  zipCode: string;
  city: string;
  street: string;
}
