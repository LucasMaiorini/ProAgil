using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProAgil.WebAPI.Dtos
{
    public class EventoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Deve ter entre 3 e 100 caracteres")]
        public string Local { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string DataEvento { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Tema { get; set; }

        [Range(2, 120000, ErrorMessage = "Quantidade de pessoas deve ser entre 2 e 120000")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public object QtdPessoas { get; set; }

        public string ImagemURL { get; set; }

        public string Telefone { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Email { get; set; }
        
        public List<LoteDto> Lotes { get; set; }
        public List<RedeSocialDto> RedesSociais { get; set; }
        public List<PalestranteDto> Palestrantes { get; set; }
    }
}