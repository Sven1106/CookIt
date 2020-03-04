// import { Injectable } from '@angular/core';
// import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
// import { Observable, throwError } from 'rxjs';
// import { catchError } from 'rxjs/operators';

// @Injectable()
// export class ErrorInterceptor implements HttpInterceptor {
//     intercept(reg: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
//         return next.handle(reg).pipe(
//             catchError(error => {
//                 if (error instanceof HttpErrorResponse) {
//                     if (error.status === 401 ||error.status === 400) {
//                         return throwError(error.statusText);
//                     }
//                     const applicationError = error.headers.get('Application-Error');
//                     if (applicationError) {
//                         return throwError(applicationError);
//                     }
//                     return throwError('Server Error');
//                 }
//             })
//         );
//     }
// }


// export const ErrorInterceptorProvider = {
//     provide: HTTP_INTERCEPTORS,
//     useClass: ErrorInterceptor,
//     multi: true
// };
