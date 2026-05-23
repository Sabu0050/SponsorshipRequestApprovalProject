import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export type QueryParams = object;

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private readonly httpClient: HttpClient) {
  }

  get<T>(path: string, params?: QueryParams): Observable<T> {
    return this.httpClient.get<T>(this.buildUrl(path), {
      params: this.buildParams(params)
    });
  }

  post<TResponse, TRequest = unknown>(path: string, body: TRequest): Observable<TResponse> {
    return this.httpClient.post<TResponse>(this.buildUrl(path), body);
  }

  private buildUrl(path: string): string {
    return `${this.baseUrl}/${path.replace(/^\/+/, '')}`;
  }

  private buildParams(params?: QueryParams): HttpParams {
    let httpParams = new HttpParams();

    Object.entries(params ?? {}).forEach(([key, value]) => {
      if (value !== null && value !== undefined && value !== '') {
        httpParams = httpParams.set(key, String(value));
      }
    });

    return httpParams;
  }
}
