import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IProduct } from './models/product';
import { IPagination } from './models/pagination';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

export class AppComponent implements OnInit {
  title = 'SkiNet2';
  products: any[];

  constructor(private http: HttpClient ) { }

  ngOnInit(): void {

    // make a connection request to the api
    //  you can check this from browser, inspect element, console
    this.http.get('https://localhost:44315/api/products?pageSize=3').
    subscribe((response: IPagination) => {
      console.log(response);
      this.products = response.data;
    }, error => {
      console.log(error);
    });
  }
}
