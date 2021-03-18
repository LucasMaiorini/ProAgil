import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Evento } from '../models/Evento';

@Injectable({
  providedIn: 'root',
})
export class EventoService {
  baseURL = 'http://localhost:5000/api/evento';

  constructor(private http: HttpClient) {}

  getAllEvento(): Observable<Evento[]> {
    return this.http.get<Evento[]>(this.baseURL);
  }
  getEventoId(id: number): Observable<Evento> {
    return this.http.get<Evento>(`${this.baseURL}/${id}`);
  }
  getEventoTema(tema: string): Observable<Evento> {
    return this.http.get<Evento>(`${this.baseURL}/getByTema/${tema}`);
  }
}
