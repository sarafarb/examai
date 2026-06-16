import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators,ReactiveFormsModule, AbstractControl } from '@angular/forms';
import {CommonModule} from '@angular/common';
import { Store } from '@ngrx/store';
import { registerStart } from '../../store/auth.actions';
import { selectAuthLoading, selectAuthError } from '../../store/auth.selectors';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule]

//   styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  showPassword = false;
  passwordStrength = 0; // 0 = חלש, 1 = בינוני, 2 = חזק
  passwordStrengthText = 'חלש';

  // מאתחלים את ה-Observables בצורה בטוחה ישירות מהסטור
  loading$: Observable<boolean>;
  serverError$: Observable<string | null>;

  constructor(private fb: FormBuilder, private store: Store) {
    // השיוך נעשה מיד בתוך הקונסטרקטור ברגע שה-store מוזרק בהצלחה
    this.loading$ = this.store.select(selectAuthLoading);
    this.serverError$ = this.store.select(selectAuthError);
  }

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordsMatchValidator });

    this.registerForm.get('password')?.valueChanges.subscribe(value => {
      this.calculatePasswordStrength(value);
    });
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  calculatePasswordStrength(password: string): void {
    if (!password) {
      this.passwordStrength = 0;
      this.passwordStrengthText = 'חלש';
      return;
    }

    let score = 0;
    if (password.length >= 8) score++;
    if (/[A-Z]/.test(password) && /[a-z]/.test(password)) score++;
    if (/[0-9]/.test(password) || /[^A-Za-z0-9]/.test(password)) score++;

    this.passwordStrength = score;
    if (score === 1) this.passwordStrengthText = 'חלש';
    else if (score === 2) this.passwordStrengthText = 'בינוני';
    else if (score === 3) this.passwordStrengthText = 'חזק ביותר';
  }

  passwordsMatchValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      return { passwordsMismatch: true };
    }
    return null;
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }
    this.store.dispatch(registerStart({ payload: this.registerForm.value }));
  }
}