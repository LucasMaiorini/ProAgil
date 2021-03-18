import { Component, OnInit, TemplateRef } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Evento } from '../models/Evento';
import { EventoService } from '../services/Evento.service';

@Component({
  selector: 'app-eventos',
  templateUrl: './eventos.component.html',
  styleUrls: ['./eventos.component.css'],
})
export class EventosComponent implements OnInit {
  _filtroLista = '';

  eventosFiltrados: Evento[] = [];
  eventos: Evento[] = [];
  imagemLargura = 70;
  imagemMargem = 2;
  mostrarImg = false;
  modalRef!: BsModalRef;

  constructor(
    private eventoService: EventoService,
    private modalService: BsModalService
  ) {}

  ngOnInit(): void {
    this.getEventos();
  }

  get filtroLista(): string {
    return this._filtroLista;
  }

  set filtroLista(value: string) {
    this._filtroLista = value;
    this.eventosFiltrados = this.filtroLista
      ? this.filtrarEventos(this.filtroLista)
      : this.eventos;
  }

  openModal(template: TemplateRef<any>): void {
    this.modalRef = this.modalService.show(template);
  }

  filtrarEventos(filtro: string): Evento[] {
    filtro = filtro.toLocaleLowerCase();
    return this.eventos.filter(
      (evento) =>
        evento.tema.toLocaleLowerCase().includes(filtro) ||
        evento.local.toLocaleLowerCase().includes(filtro)
    );
  }

  alternarImg(): void {
    this.mostrarImg = !this.mostrarImg;
  }

  getEventos(): void {
    // tslint:disable-next-line: deprecation
    this.eventoService.getAllEvento().subscribe(
      (eventosResponse: Evento[]) => {
        this.eventos = eventosResponse;
        this.eventosFiltrados = this.eventos;
      },
      (error) => console.log(error)
    );
  }
}
