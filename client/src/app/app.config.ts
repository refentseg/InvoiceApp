import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptors, withInterceptorsFromDi } from '@angular/common/http';
import { FormControl, FormsModule } from '@angular/forms';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { LucideAngularModule,Archive,User, House, LogOut, Settings, Trash2, Printer, SquarePen } from 'lucide-angular';
import { AuthGuardService } from './guards/auth-guard.service';
import { provideToastr, ToastrModule } from 'ngx-toastr';
import { provideAnimations } from '@angular/platform-browser/animations';
import { JwtInterceptorService } from './auth/jwt-interceptor.service';
import { provideClientHydration } from '@angular/platform-browser';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';

export const appConfig: ApplicationConfig = {
  providers: [
    provideAnimations(),
    provideToastr(),
    provideRouter(routes),
    provideClientHydration(),
    provideHttpClient(withInterceptorsFromDi()),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptorService,
      multi: true
    },
     provideAnimationsAsync(),
     importProvidersFrom(LucideAngularModule.pick({ Archive, User,House,LogOut,Settings,Trash2,SquarePen,Printer })),
     provideCharts(withDefaultRegisterables())
  ]
};
