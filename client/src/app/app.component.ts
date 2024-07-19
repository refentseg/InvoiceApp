import { CommonModule, NgClass } from '@angular/common';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { Component, ViewChild } from '@angular/core';
import { Event, NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { InvoicesComponent } from './invoices/invoices.component';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenav, MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { BrowserAnimationsModule, provideAnimations } from '@angular/platform-browser/animations';
import {MatListModule} from '@angular/material/list';
import { BreakpointObserver } from '@angular/cdk/layout';
import { LucideAngularModule, Archive } from 'lucide-angular';
import { NavItemComponent } from './components/nav-item/nav-item.component';
import { filter, Subject, Subscription, takeUntil } from 'rxjs';
import { AuthGuardService } from './guards/auth-guard.service';
import { JwtInterceptorService } from './auth/jwt-interceptor.service';
import { provideToastr, ToastrModule } from 'ngx-toastr';
import { AuthService } from './auth/auth.service';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { User } from './models/user';

interface DashLink {
  icon: string;
  path: string;
  title: string;
  active: boolean;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,CommonModule,
    HttpClientModule,
    MatIconModule,
    MatButtonModule,
    MatToolbarModule,
    MatSidenavModule,
    MatListModule,
    NavItemComponent,
    NgClass, RouterLink,
    LucideAngularModule,LoginComponent,RegisterComponent],
  templateUrl: './app.component.html',
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptorService, multi: true }],
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'InvoicingApp';

  dashlinks: DashLink[] = [
    { icon: 'house', path: '/', title: 'Dashboard',active:false },
    { icon: 'archive', path: '/invoices', title: 'Invoices',active:false  },
    { icon: 'user', path: '/customers', title: 'Customers',active:false  },
  ];
  private userSubscription?: Subscription;
  user: User | null = null;

  dashlinksWithActive: DashLink[] = [];

  @ViewChild(MatSidenav)
  sidenav!: MatSidenav;
  isMobile= true;
  isLoginMode = true;

  toggleMode() {
    this.isLoginMode = !this.isLoginMode;
  }
  constructor(private observer: BreakpointObserver,private router: Router,private authService: AuthService) {}

  ngOnInit() {
    this.observer.observe(['(max-width: 800px)']).subscribe((screenSize) => {
      if(screenSize.matches){
        this.isMobile = true;
      } else {
        this.isMobile = false;
      }
    });
    this.updateActiveLink(this.router.url);

    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
    ).subscribe((event: Event) => {
      if (event instanceof NavigationEnd) {
        this.updateActiveLink(event.urlAfterRedirects);
      }
    });

    this.userSubscription = this.authService.user$?.subscribe(user => {
      this.user = user;
    });

    if (!this.user) {
      this.loadCurrentUser();
    }
  }
  isCollapsed = true;
  expanded = true;

  updateActiveLink(currentPath: string) {
    this.dashlinksWithActive = this.dashlinks.map(link => ({
      ...link,
      active: currentPath.startsWith(link.path) && (link.path !== '/' || currentPath === '/')
    }));
  }
  loadCurrentUser() {
    this.authService.getCurrentUser().subscribe({
      next: (user) => {
        localStorage.setItem('user', JSON.stringify(user));
        this.authService['_user'].next(user);
      },
      error: (error) => {
        console.error('Error fetching current user:', error);
      }
    });
  }


  toggleMenu() {
    if(this.isMobile){
      this.sidenav.toggle();
      this.isCollapsed = false;
      this.expanded = true; // On mobile
    } else {
      this.sidenav.open(); //On desktop/tablet
      this.isCollapsed = !this.isCollapsed;
      this.expanded = !this.expanded;
    }
  }
  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  logout() {
    this.authService.logout();
  }
}
