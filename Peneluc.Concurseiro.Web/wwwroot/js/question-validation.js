$(function () {
    $('form').on('submit', function (e) {
        // Executa a validação customizada apenas se a validação padrão (unobtrusive) passar
        if (!$(this).valid()) {
            return;
        }

        // Verifica se existe pelo menos um checkbox com name contendo "IsCorrect" marcado
        const isAnyCorrect = $('.form-check-input[name*="IsCorrect"]:checked').length > 0;
        const summaryDiv = $('#custom-validation-summary');

        if (!isAnyCorrect) {
            e.preventDefault();
            summaryDiv.text('Pelo menos uma alternativa deve ser marcada como correta.').show();
        } else {
            summaryDiv.hide();
        }
    });
});