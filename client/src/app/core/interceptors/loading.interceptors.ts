import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BusyService } from '../services/busy.service';
import { Injectable } from '@angular/core';
import { delay, finalize } from 'rxjs/operators';

@Injectable()

export class LoadingInterceptor implements HttpInterceptor {

    constructor(private busyService: BusyService) {}

    intercept(req: HttpRequest<any>, next: HttpHandler):
    Observable<HttpEvent<any>> {
        
        if (req.method === 'POST' && req.url.includes('orders')) {
            return next.handle(req);
        }
        if (req.method === 'DELETE') {
            return next.handle(req);
        }
        if (req.url.includes('emailexists')) {
            // turn off loading spinner for checking email exists
            return next.handle(req);
        }
        this.busyService.busy();
        return next.handle(req).pipe(
            
            finalize(() => {
                this.busyService.idle();
            })
        );
    }
}