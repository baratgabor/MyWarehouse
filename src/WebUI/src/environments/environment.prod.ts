export const environment = {
  production: true,
  // baseHost: 'https://testwarehouse.azurewebsites.net',
  // apiHost: 'https://testwarehouse.azurewebsites.net/api',
  baseHost: `https://${new URL(window.location.origin).hostname}:5001`,
  apiHost: `https://${new URL(window.location.origin).hostname}:5001`,
};
