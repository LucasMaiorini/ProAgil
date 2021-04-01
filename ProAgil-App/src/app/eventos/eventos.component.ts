import { Component, OnInit } from '@angular/core';
import { EventoService } from '../services/Evento.service';
import { Evento } from '../models/Evento';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { defineLocale } from 'ngx-bootstrap/chronos';
import { ptBrLocale } from 'ngx-bootstrap/locale';
import { ToastrService } from 'ngx-toastr';
import { BsModalService } from 'ngx-bootstrap/modal';

defineLocale('pt-br', ptBrLocale);

@Component({
  selector: 'app-eventos',
  templateUrl: './eventos.component.html',
  styleUrls: ['./eventos.component.css'],
})
export class EventosComponent implements OnInit {
  titulo = 'Eventos';
  // tslint:disable-next-line: variable-name
  _filtroLista = '';
  eventosFiltrados: Evento[] = [];
  eventos: Evento[] = [];
  evento!: Evento;
  modoSalvar = 'post';
  dataEvento!: string;
  file!: File[];
  imagemLargura = 70;
  imagemMargem = 2;
  mostrarImg = false;
  registerForm!: FormGroup;
  bodyDeletarEvento = '';
  fileNameToUpdate!: string;
  dataAtual!: string;

  constructor(
    private eventoService: EventoService,
    private fb: FormBuilder,
    private modalService: BsModalService,
    private localeService: BsLocaleService,
    private toastr: ToastrService
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

  getEventos(): void {
    this.dataAtual = new Date().getMilliseconds().toString();

    // tslint:disable-next-line: deprecation
    this.eventoService.getAllEvento().subscribe(
      (eventosResponse: Evento[]) => {
        this.eventos = eventosResponse;
        this.eventosFiltrados = this.eventos;
      },
      (error) => {
        this.toastr.error(`Erro ao tentar Carregar eventos: ${error}`);
      }
    );
  }
  novoEvento(template: any): void {
    this.modoSalvar = 'post';
    this.openModal(template);
  }

  editarEvento(eventoParam: Evento, template: any): void {
    this.modoSalvar = 'put';
    this.openModal(template);
    this.evento = Object.assign({}, eventoParam);
    this.fileNameToUpdate = eventoParam.imagemURL.toString();
    this.evento.imagemURL = '';
    this.registerForm.patchValue(this.evento);
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
        this.toastr.success('Deletado com sucesso', 'Evento');
      },
      (error: any) => {
        this.toastr.error(`Erro ao deletar: ${error}`, 'Evento');
        console.log(error);
      }
    );
  }

  salvarAlteracao(template: any): void {
    if (this.registerForm.valid) {
      if (this.modoSalvar === 'post') {
        // Object.assign copies the values to the variable. Not changes the pointer
        this.evento = Object.assign({}, this.registerForm.value);
        this.uploadImagem();
        this.eventoService.postEvento(this.evento).subscribe(
          (novoEvento: Evento) => {
            template.hide();
            this.getEventos();
            this.toastr.success('Inserido com sucesso', 'Evento');
          },
          (error: any) => {
            this.toastr.error(`Erro ao inserir: ${error}`);
          }
        );
      } else {
        // Object.assign copies the values to the variable. Not changes the pointer
        this.evento = Object.assign(
          { id: this.evento.id },
          this.registerForm.value
        );
        this.uploadImagem();
        this.eventoService.putEvento(this.evento).subscribe(
          () => {
            template.hide();
            this.getEventos();
            this.toastr.success('Editado com sucesso');
          },
          (error: any) => {
            this.toastr.error(`Erro ao editar: ${error}`, 'Evento');
          }
        );
      }
    }
  }

  onFileChange(event: any): void {
    const reader = new FileReader();
    if (event.target.files && event.target.files.length) {
      this.file = event.target.files;
      console.log(this.file);
    }
  }

  uploadImagem(): void {
    if (this.modoSalvar === 'post') {
      const nomeArquivo = this.evento.imagemURL.split('\\', 3);
      this.evento.imagemURL = nomeArquivo[2];
      this.eventoService.postUpload(this.file, nomeArquivo[2]).subscribe(() => {
        this.dataAtual = new Date().getMilliseconds().toString();
        this.getEventos();
      });
    } else {
      this.evento.imagemURL = this.fileNameToUpdate;
      this.eventoService
        .postUpload(this.file, this.fileNameToUpdate)
        .subscribe(() => {
          this.dataAtual = new Date().getMilliseconds().toString();
          this.getEventos();
        });
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
      local: ['', [Validators.required, Validators.minLength(3)]],
      dataEvento: ['', Validators.required],
      imagemURL: ['', Validators.required],
      qtdPessoas: ['', [Validators.required, Validators.max(120000)]],
      telefone: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
    });
  }
}
