import { PartnerAddress } from "./partner-address";

export interface PartnerUpdate {
  id: number;
  name: string;

  address: PartnerAddress;
}
