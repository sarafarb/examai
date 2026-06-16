import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { Subscription, timer } from 'rxjs';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-verify-email-sent',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './verify-email-sent.component.html'
})
export class VerifyEmailSentComponent implements OnInit, OnDestroy {
  countdown = 0;
  isResending = false;
  message: string | null = null;
  private timerSub?: Subscription;

  constructor(private authService: AuthService) {}

  ngOnInit(): void {}

  resendEmail(): void {
    this.isResending = true;
    this.message = null;

    // כאן ניתן להחליף בכתובת המייל האמיתית מתוך ה-Store או ה-State במידת הצורך
    this.authService.resendVerification('user@exam.com').subscribe({
      next: () => {
        this.isResending = false;
        this.message = 'קוד אימות חדש נשלח לתיבת הדואר שלך.';
        this.startCountdown();
      },
      error: () => {
        this.isResending = false;
        this.message = 'שגיאה בשליחת הקוד. אנא נסה שוב מאוחר יותר.';
      }
    });
  }

  startCountdown(): void {
    this.countdown = 60;
    this.timerSub = timer(0, 1000)
      .pipe(take(61))
      .subscribe({
        next: (val) => this.countdown = 60 - val,
        complete: () => this.countdown = 0
      });
  }

  ngOnDestroy(): void {
    this.timerSub?.unsubscribe();
  }
}