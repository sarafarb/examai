import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.scss']
})
export class NotificationComponent {
  @Input() unreadCount = 2;
  isOpen = false;
  notifications = ['המבחן במתמטיקה נסרק בהצלחה', 'תזכורת: נותרו לך 13 עמודים לסריקה'];

  toggleDropdown(): void {
    this.isOpen = !this.isOpen;
    if (this.isOpen) this.unreadCount = 0;
  }
}