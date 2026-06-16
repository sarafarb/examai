import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VerifyEmailSentComponent } from './verify-email-sent.component';

describe('VerifyEmailSentComponent', () => {
  let component: VerifyEmailSentComponent;
  let fixture: ComponentFixture<VerifyEmailSentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VerifyEmailSentComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(VerifyEmailSentComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
