import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-breadcrumb',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.scss']
})
export class BreadcrumbComponent implements OnInit {
  breadcrumbs: string[] = ['ראשי'];

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe(() => {
      this.buildBreadcrumbs(this.router.url);
    });
    this.buildBreadcrumbs(this.router.url);
  }

  private buildBreadcrumbs(url: string): void {
    const parts = url.split('/').filter(p => p);
    this.breadcrumbs = ['ראשי'];
    parts.forEach(part => {
      const translations: { [key: string]: string } = {
        'dashboard': 'דשבורד', 'my-exams': 'המבחנים שלי', 'analytics': 'ניתוח כיתה', 'billing': 'חיוב'
      };
      if (translations[part]) this.breadcrumbs.push(translations[part]);
    });
  }
}