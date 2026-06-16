import { createAction, props } from '@ngrx/store';
import { User } from '../../../core/models/auth.models';

export const registerStart = createAction('[Auth] Register Start', props<{ payload: any }>());
export const registerSuccess = createAction('[Auth] Register Success');
export const registerFailure = createAction('[Auth] Register Failure', props<{ error: string }>());

export const loginSuccess = createAction('[Auth] Login Success', props<{ user: User; accessToken: string; refreshToken: string }>());
export const logout = createAction('[Auth] Logout');