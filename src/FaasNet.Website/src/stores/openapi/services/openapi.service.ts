import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environment';
import { Observable } from 'rxjs';

@Injectable()
export class OpenApiService {
  constructor(
    private http: HttpClient) { }


  getOperations(endpoint: string): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + '/openapi/operations';
    return this.http.post<any>(targetUrl, {
      endpoint: endpoint
    }, { headers: headers });
  }

  getOperation(endpoint: string, operationId: string): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + '/openapi/operations/' + operationId;
    return this.http.post<any>(targetUrl, {
      endpoint: endpoint
    }, { headers: headers });
  }
}
