import { DynamicLayoutComponent } from '@abp/ng.core';
import { LoaderBarComponent } from '@abp/ng.theme.shared';
import { Component, OnInit } from '@angular/core';
import { ReplaceableComponentsService } from '@abp/ng.core'; 
import { eAccountComponents } from '@abp/ng.account'; 
import { CustomPersonalSettingsComponent } from './user-profiles/custom-personal-settings/custom-personal-settings'; 

@Component({
  selector: 'app-root',
  template: `
    <abp-loader-bar />
    <abp-dynamic-layout />
  `,
  imports: [LoaderBarComponent, DynamicLayoutComponent],
})

export class AppComponent implements OnInit {
  
  constructor(private replaceableComponents: ReplaceableComponentsService) {} 

  ngOnInit() {
    this.replaceableComponents.add({
      component: CustomPersonalSettingsComponent,
      key: eAccountComponents.PersonalSettings, // Reemplaza la pestaña "Ajustes personales"
    });
  }
}
