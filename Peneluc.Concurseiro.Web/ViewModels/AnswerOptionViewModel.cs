using System;

namespace Peneluc.Concurseiro.Web.ViewModels
{
    public class AnswerOptionViewModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool IsCorrect { get; set; }
    }
}