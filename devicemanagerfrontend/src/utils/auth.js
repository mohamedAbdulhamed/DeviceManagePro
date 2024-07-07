import { TOKEN_KEY, REFRESH_TOKEN_KEY } from '../config/constants'

/**
 * set token.
 * @param {string} Token
 * @returns {void}
 */
export const setToken = (token) => {
  localStorage.setItem(TOKEN_KEY, token);
};

/**
 * set refreshToken.
 * @param {string} RefreshToken
 * @returns {void}
 */
export const setRefreshToken = (refreshToken) => {
  localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
};

/**
 * Gets the saved token.
 * @returns {string | null} - The token, or null if the token doesn't exist.
 */
export const getToken = () => {
  return localStorage.getItem(TOKEN_KEY);
};

/**
 * Gets the saved refreshToken.
 * @returns {string | null} - The refreshToken, or null if the refreshToken doesn't exist.
 */
export const getRefreshToken = () => {
  return localStorage.getItem(REFRESH_TOKEN_KEY);
};

/**
 * Removes the token.
 * @returns {void}
 */
export const removeToken = () => {
  localStorage.removeItem(TOKEN_KEY);
};

/**
 * Removes the refreshToken.
 * @returns {void}
 */
export const removeRefreshToken = () => {
  localStorage.removeItem(REFRESH_TOKEN_KEY);
};

/**
 * Checks if the user is authenticated.
 * @returns {boolean} - True if the user is authenticated, false otherwise.
 */
export const isAuthenticated = () => {
  const token = getToken();
  if (!token) return false;
  return true;
};