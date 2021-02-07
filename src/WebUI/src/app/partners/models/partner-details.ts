import { PartnerAddress } from "./partner-address";

export interface PartnerDetails {
  id: number;
  name: string;

  createdAt: Date;
  lastModifiedAt: Date;

  address: PartnerAddress;
}
