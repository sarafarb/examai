import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { AuthService } from '../../../core/auth/services/auth.service';
import { TokenStorageService } from '../../../core/auth/services/token-storage.service';
import * as AuthActions from './auth.actions';
import { catchError, map, mergeMap, tap } from 'rxjs/operators';
import { of } from 'rxjs';
import { Router } from '@angular/router';

@Injectable()
export class AuthEffects {
  register$ = createEffect(() => this.actions$.pipe(
    ofType(AuthActions.registerStart),
    mergeMap(action => this.authService.register(action.payload).pipe(
      map(() => AuthActions.registerSuccess()),
      catchError(err => of(AuthActions.registerFailure({ error: err.error?.error || 'שגיאת רישום לא ידועה' })))
    ))
  ));

  registerSuccess$ = createEffect(() => this.actions$.pipe(
    ofType(AuthActions.registerSuccess),
    tap(() => this.router.navigate(['/auth/verify-email-sent']))
  ), { dispatch: false });

  loginSuccess$ = createEffect(() => this.actions$.pipe(
    ofType(AuthActions.loginSuccess),
    tap(action => this.tokenStorage.setTokens(action.accessToken, action.refreshToken))
  ), { dispatch: false });

  logout$ = createEffect(() => this.actions$.pipe(
    ofType(AuthActions.logout),
    tap(() => {
      this.tokenStorage.clearTokens();
      this.router.navigate(['/auth/login']);
    })
  ), { dispatch: false });

  constructor(
    private actions$: Actions,
    private authService: AuthService,
    private tokenStorage: TokenStorageService,
    private router: Router
  ) {}
}