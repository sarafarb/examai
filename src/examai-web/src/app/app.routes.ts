import { Routes } from '@angular/router';
import { RegisterComponent } from './features/auth/pages/register/register.component';
import { LoginComponent } from './features/auth/pages/login/login.component';
import { VerifyEmailSentComponent } from './features/auth/pages/verify-email-sent/verify-email-sent.component';
import { VerifyEmailComponent } from './features/auth/pages/verify-email/verify-email.component';
import { ForgotPasswordComponent } from './features/auth/pages/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './features/auth/pages/reset-password/reset-password.component';

export const routes: Routes = [
  { path: 'auth/register', component: RegisterComponent },
  { path: 'auth/login', component: LoginComponent },
  { path: 'auth/verify-email-sent', component: VerifyEmailSentComponent },
  { path: 'auth/verify-email', component: VerifyEmailComponent },
  { path: 'auth/forgot-password', component: ForgotPasswordComponent },
  { path: 'auth/reset-password', component: ResetPasswordComponent },
  { path: '', redirectTo: 'auth/login', pathMatch: 'full' }
];