using System;
using System.ComponentModel.DataAnnotations;

namespace Peneluc.Concurseiro.Web.ViewModels
{
    public class UpdateAnswerOptionViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "A descrição da alternativa é obrigatória.")]
        public string Description { get; set; }
        public bool IsCorrect { get; set; }
    }
}