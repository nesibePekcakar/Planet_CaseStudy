$(document).ready(function () {
    // Archive button
    $(document).on("click", ".archiveTaskBtn", function (e) {
        e.preventDefault();
        if (!confirm("Bu görevi arşivlemek istediğinizden emin misiniz?")) return;

        var taskId = $(this).data("id");

        $.ajax({
            url: '/Task/AjaxArchive',
            type: 'POST',
            data: { id: taskId },
            success: function (response) {
                if (response.success) {
                    $("#taskRow_" + taskId).fadeOut();
                    updateTaskList();
                } else {
                    alert("Arşivleme başarısız: " + response.message);
                }
            },
            error: function () {
                alert("Bir hata oluştu.");
            }
        });
    });

    // Load form into modal
    $(document).on("click", ".open-modal", function (e) {
        e.preventDefault();
        var url = $(this).attr("href");

        $("#taskModal .modal-content").html('<div class="text-center p-5"><div class="spinner-border"></div></div>');
        $("#taskModal").modal("show");

        $.get(url, function (html) {
            $("#taskModal .modal-content").html(html);
        });
    });

    // Submit AJAX forms inside modals
    $(document).on("submit", "form.ajaxForm", function (e) {
        e.preventDefault();

        var $form = $(this);
        var formData = $form.serialize();

        $.ajax({
            url: $form.attr("action"),
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    updateTaskList();
                } else if (response.errors) {
                    alert("Hata:\n" + response.errors.join("\n"));
                } else {
                    $("#taskModal .modal-content").html(response); // in case of validation error returning View()
                }
            },
            error: function () {
                alert("Sunucu hatası.");
            }
        });
    });
});

// Function to refresh the task list
function updateTaskList() {
    $.get('/Task/PartialTaskList', function (partialHtml) {
        $('#taskListContainer').html(partialHtml);
        $('#taskModal').modal('hide');
    });
}
