import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { NotFoundComponent } from './core/not-found/not-found.component';
import { ServerErrorComponent } from './core/server-error/server-error.component';
import { TestErrorComponent } from './core/test-error/test-error.component';
import { HomeComponent } from './home/home.component';
import { ProductDetailsComponent } from './shop/product-details/product-details.component';
import { ShopComponent } from './shop/shop.component';

const routes: Routes = [
  {path: '', component: HomeComponent, data: { breadcrumb: 'Home' } },

  { path: 'test-error', component: TestErrorComponent, data: { breadcrumb: 'Test Errors' } },
  { path: 'server-error', component: ServerErrorComponent, data: { breadcrumb: 'Server Error' } },
  { path: 'not-found', component: NotFoundComponent, data: { breadcrumb: 'Not Found' } },
  
  // lazy loading only when shop is activated,we load the shop.moodule
  {path: 'shop', 
    loadChildren: () => import('./shop/shop.module')
    .then(mod => mod.ShopModule), data: {breadcrumb: 'Shop'}},

  // lazy loading only when basket is activated,we load the shop.moodule
  {path: 'basket', 
    loadChildren: () => import('./basket/basket.module')
    .then(mod => mod.BasketModule), data: {breadcrumb: 'Basket'}},

  // lazy loading only when checkout is activated,we load the shop.moodule
  {path: 'checkout', 
    loadChildren: () => import('./checkout/checkout.module')
    .then(mod => mod.CheckoutModule), data: {breadcrumb: 'Checkout'}},
    
  // lazy loading only when account is activated,we load the shop.moodule
  {path: 'account', 
    loadChildren: () => import('./account/account.module')
    .then(mod => mod.AccountModule), data: {breadcrumb: {skip: true}}},

   {path: '**', redirectTo: '', pathMatch: 'full'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
