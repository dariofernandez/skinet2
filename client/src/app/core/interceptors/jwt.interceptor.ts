import {HttpInterceptor, HttpRequest, HttpHandler, HttpEvent} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()

export class JwtInterceptor implements HttpInterceptor {
    // intercept(req: HttpRequest<any>, next: HttpHandler):
    //  Observable<HttpEvent<any>> {
    //     throw new Error('Method not implemented.');
    // }

    intercept(req: HttpRequest<any>, next: HttpHandler):
              Observable<HttpEvent<any>> {

        const token = localStorage.getItem('token');

        if (token) {
            // if we have a token, clone the request and set the header
            req = req.clone({
                setHeaders: {
                    Authorization: `Bearer ${token}`
                }
            });
        }
        return next.handle(req);
    }

}
