export class RedeSocial {
  id: number;
  nome: string;
  URL: string;
  eventoId?: number;
  palestranteId?: number;

  constructor(id: number) {
    this.id = id;
  }
}
