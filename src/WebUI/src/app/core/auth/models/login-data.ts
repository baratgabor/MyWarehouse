
export interface AuthenticationSuccessData {
  accessToken: string;
  tokenType: string;
  expiresIn: number;
  username: string;
  email: string;
  isExternalLogin: string;
  externalAuthenticationProvider: string;
}
