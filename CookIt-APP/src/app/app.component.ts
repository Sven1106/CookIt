import { RecipeService } from 'src/app/_services/recipe/recipe.service';
import { Component } from '@angular/core';

import { Platform } from '@ionic/angular';
import { SplashScreen } from '@ionic-native/splash-screen/ngx';
import { StatusBar } from '@ionic-native/status-bar/ngx';
import { debounceTime } from 'rxjs/operators';
import { AlertService } from './_services/alert/alert.service';
import { NetworkService } from './_services/network/network.service';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss']
})
export class AppComponent {
  isConnected: boolean;
  constructor(
    private platform: Platform,
    private splashScreen: SplashScreen,
    private statusBar: StatusBar,
    private networkService: NetworkService,
    private alertService: AlertService,
    private recipeService: RecipeService
  ) {
    this.initializeApp();
  }

  initializeApp() {
    this.platform.ready().then(() => {
      this.statusBar.styleDefault();
      this.splashScreen.hide();
      this.networkSubscriber();
    });
  }
  networkSubscriber(): void {
    this.networkService
      .getNetworkStatus()
      .pipe(debounceTime(300))
      .subscribe((connected: boolean) => {
        this.isConnected = connected;
        if (this.isConnected === false) {
          this.alertService.error('Ingen internet');
        }
      });
  }
}
