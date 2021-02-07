import { PartnerAddress } from "./partner-address";

export interface PartnerCreate {
  name: string;

  address: PartnerAddress;
}
