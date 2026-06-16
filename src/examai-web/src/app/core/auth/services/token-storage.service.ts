import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokenStorageService {
  private _accessToken: string | null = null;
  private readonly REFRESH_TOKEN_KEY = 'exam_ai_rt';

  setTokens(accessToken: string, refreshToken: string): void {
    this._accessToken = accessToken;
    sessionStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken);
  }

  getAccessToken(): string | null {
    return this._accessToken;
  }

  getRefreshToken(): string | null {
    return sessionStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  clearTokens(): void {
    this._accessToken = null;
    sessionStorage.removeItem(this.REFRESH_TOKEN_KEY);
  }
}