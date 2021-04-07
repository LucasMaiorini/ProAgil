import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { ToastrService } from 'ngx-toastr';
import { Evento } from 'src/app/models/Evento';
import { Lote } from 'src/app/models/Lote';
import { RedeSocial } from 'src/app/models/RedeSocial';
import { EventoService } from 'src/app/services/Evento.service';

@Component({
  selector: 'app-evento-edit',
  templateUrl: './eventoEdit.component.html',
  styleUrls: ['./eventoEdit.component.scss'],
})
export class EventoEditComponent implements OnInit {
  titulo = 'Editar Evento';
  registerForm: FormGroup;
  evento = new Evento();
  dataEvento: Date;
  imagemURL = 'assets/img/upload.png';
  file!: File[];
  imageNameToUpdate: string;
  dataAtual = '';

  constructor(
    private eventoService: EventoService,
    private fb: FormBuilder,
    private router: ActivatedRoute,
    private localeService: BsLocaleService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.validation();
    this.loadEvento();
  }

  validation(): void {
    this.registerForm = this.fb.group({
      id: [],
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
      imagemURL: [''],
      qtdPessoas: ['', [Validators.required, Validators.max(120000)]],
      telefone: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      lotes: this.fb.array([]),
      redesSociais: this.fb.array([]),
    });
  }

  createsLote(lote: any): FormGroup {
    return this.fb.group({
      id: [lote.id],
      nome: [lote.nome, Validators.required],
      quantidade: [lote.quantidade, Validators.required],
      preco: [lote.preco, Validators.required],
      dataInicio: [lote.dataInicio],
      dataFim: [lote.dataFim],
    });
  }

  createsRedesSociais(redeSocial: any): FormGroup {
    return this.fb.group({
      id: [redeSocial.id],
      nome: [redeSocial.nome, Validators.required],
      url: [redeSocial.URL, Validators.required],
    });
  }

  get lotes(): FormArray {
    return this.registerForm.get('lotes') as FormArray;
  }

  get redesSociais(): FormArray {
    return this.registerForm.get('redesSociais') as FormArray;
  }

  // addLotes() {
  //   this.lotes.push(this.createsLote({id: 0}));
  // }
  // addRedesSociais(): void {
  //   this.redesSociais.push(this.createsRedesSociais({id: 0}));
  // }
  addLotes(): void {
    this.lotes.push(this.createsLote(new Lote(0)));
  }

  addRedesSociais(): void {
    this.redesSociais.push(this.createsRedesSociais(new RedeSocial(0)));
  }

  removeLotes(id: number): void {
    this.lotes.removeAt(id);
  }

  removeRedesSociais(id: number): void {
    this.redesSociais.removeAt(id);
  }

  /*
  Function called when changes the image of an event.
   */
  onFileChange(event: any): void {
    const reader = new FileReader();
    // Convert event in FileList
    const filelist: FileList = (event.target as HTMLInputElement).files;
    // Change the current image to the new one
    reader.onload = (eventRead: any) => {
      this.imagemURL = eventRead.target.result;
    };
    this.file = event.target.files;
    reader.readAsDataURL(this.file[0]);
  }

  loadEvento(): any {
    // Gets the id in the route
    const id = +this.router.snapshot.paramMap.get('id');
    this.eventoService.getEventoId(id).subscribe((evento: Evento) => {
      this.evento = Object.assign({}, evento);
      this.imageNameToUpdate = evento.imagemURL.toString();
      this.imagemURL = `http://localhost:5000/resources/images/${this.evento.imagemURL}?ts=${this.dataAtual}`;
      // Cleans the imagemURL in order to avoid throwing error into console.
      this.evento.imagemURL = '';
      this.registerForm.patchValue(this.evento);
      this.evento.lotes.forEach((lote) => {
        this.lotes.push(this.createsLote(lote));
      });
      this.evento.redesSociais.forEach((redeSocial) => {
        this.redesSociais.push(this.createsRedesSociais(redeSocial));
      });
    });
  }

  saveEvento(): void {
    // Object.assign copies the values to the variable. Not changes the pointer
    this.evento = Object.assign(
      { id: this.evento.id },
      this.registerForm.value
    );
    this.evento.imagemURL = this.imageNameToUpdate;
    this.uploadImagem();
    this.eventoService.putEvento(this.evento).subscribe(
      () => {
        this.toastr.success('Editado com sucesso');
      },
      (error: any) => {
        this.toastr.error(`Erro ao editaro: ${error}`, 'Evento');
      }
    );
  }

  uploadImagem(): void {
    // Uploads only if the image was changed.
    if (this.registerForm.get('imagemURL').value !== '') {
      this.eventoService
        .postUpload(this.file, this.imageNameToUpdate)
        .subscribe(() => {
          this.dataAtual = new Date().getMilliseconds().toString();
          this.imagemURL = `http://localhost:5000/resources/images/${this.evento.imagemURL}?ts=${this.dataAtual}`;
        });
    }
  }
}
