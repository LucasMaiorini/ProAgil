import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Evento } from '../models/Evento';
import { EventoService } from '../services/Evento.service';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { defineLocale } from 'ngx-bootstrap/chronos';
import { ptBrLocale } from 'ngx-bootstrap/locale';

defineLocale('pt-br', ptBrLocale);
@Component({
  selector: 'app-eventos',
  templateUrl: './eventos.component.html',
  styleUrls: ['./eventos.component.css'],
})
export class EventosComponent implements OnInit {
  // tslint:disable-next-line: variable-name
  _filtroLista = '';

  eventosFiltrados: Evento[] = [];
  eventos: Evento[] = [];
  evento!: Evento;
  modoSalvar = '';

  imagemLargura = 70;
  imagemMargem = 2;
  mostrarImg = false;
  modalRef!: BsModalRef;
  registerForm!: FormGroup;
  bodyDeletarEvento = '';

  constructor(
    private eventoService: EventoService,
    private modalService: BsModalService,
    private fb: FormBuilder,
    private localeService: BsLocaleService
  ) {
    this.localeService.use('pt-br');
  }

  ngOnInit(): void {
    this.validation();
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

  editarEvento(evento: Evento, template: any): void {
    this.modoSalvar = 'put';
    this.openModal(template);
    this.evento = evento;
    this.registerForm.patchValue(evento);
  }

  novoEvento(template: any): void {
    this.modoSalvar = 'post';
    this.openModal(template);
  }

  openModal(template: any): void {
    this.registerForm.reset();
    template.show();
  }

  excluirEvento(evento: Evento, confirm: any): void {
    this.openModal(confirm);
    this.evento = evento;
    this.bodyDeletarEvento = `Tem certeza que deseja excluir o Evento: ${evento.tema} ?`;
  }

  confirmeDelete(template: any): void {
    this.eventoService.deleteEvento(this.evento.id).subscribe(
      () => {
        template.hide();
        this.getEventos();
      },
      (error: any) => {
        console.log(error);
      }
    );
  }
  salvarAlteracao(template: any): void {
    if (this.registerForm.valid) {
      if (this.modoSalvar === 'post') {
        this.evento = Object.assign({}, this.registerForm.value);
        this.eventoService.postEvento(this.evento).subscribe(
          (novoEvento: Evento) => {
            template.hide();
            this.getEventos();
          },
          (error: any) => {
            console.log(error);
          }
        );
      }
      if (this.modoSalvar === 'put') {
        this.evento = Object.assign(
          { id: this.evento.id },
          this.registerForm.value
        );
        this.eventoService.putEvento(this.evento).subscribe(
          () => {
            template.hide();
            this.getEventos();
          },
          (error: any) => {
            console.log(error);
          }
        );
      }
    }
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

  validation(): void {
    this.registerForm = this.fb.group({
      tema: [
        '',
        [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(50),
        ],
      ],
      local: ['', Validators.required],
      dataEvento: ['', Validators.required],
      imagemURL: ['', Validators.required],
      qtdPessoas: ['', [Validators.required, Validators.max(120000)]],
      telefone: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
    });
  }
}
