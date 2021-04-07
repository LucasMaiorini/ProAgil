import { Evento } from './Evento';

export class Lote {
  id: number;
  nome: string;
  preco: number;
  dataInicio?: Date;
  dataFim?: Date;
  quantidade: number;
  eventoId: number;
  evento: Evento;
 
  constructor(id: number) {
    this.id = id
  }
}