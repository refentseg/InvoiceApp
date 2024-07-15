import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
import { FormControl } from '@angular/forms';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { LucideAngularModule,Archive,User, House } from 'lucide-angular';
import { AuthGuardService } from './guards/auth-guard.service';
import { provideToastr, ToastrModule } from 'ngx-toastr';
import { provideAnimations } from '@angular/platform-browser/animations';

export const appConfig: ApplicationConfig = {
  providers: [
    provideAnimations(),
    provideToastr(),
    provideRouter(routes),provideHttpClient(), provideAnimationsAsync(),importProvidersFrom(LucideAngularModule.pick({ Archive, User,House })),
  ]
};
