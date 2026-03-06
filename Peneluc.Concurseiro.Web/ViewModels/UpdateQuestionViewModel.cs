﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Peneluc.Concurseiro.Web.ViewModels
{
    public class UpdateQuestionViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo Enunciado é obrigatório.")]
        [MinLength(10, ErrorMessage = "O enunciado deve ter no mínimo 10 caracteres.")]
        public string Statement { get; set; }

        public string Explanation { get; set; }

        [Required(ErrorMessage = "O campo Dificuldade é obrigatório.")]
        public string Difficulty { get; set; }

        [Required(ErrorMessage = "O campo SubjectId é obrigatório.")]
        public Guid SubjectId { get; set; }

        [Required(ErrorMessage = "O campo ExamSourceId é obrigatório.")]
        public Guid ExamSourceId { get; set; }

        [Range(1900, 2030, ErrorMessage = "O ano do exame deve ser um ano válido.")]
        public int? ExamYear { get; set; }

        public List<UpdateAnswerOptionViewModel> AnswerOptions { get; set; } = new();
    }
}