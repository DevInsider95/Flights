var Table = null;

function InitCustomTable(TableId, ControllerPath, SelectItemErrorMessage, ConfirmDeleteMessage) {

    // Instance de la table
    Table = new DataTableCustom(TableId);

    // Redirection vers l'action d'ajout
    $('#btnAdd').click(function () {
        location.href = ControllerPath + "/Creer";
    });

    // Redirection vers l'action de modification
    $('#btnEdit').click(function () {
        if (Table.SelectedId != null)
            location.href = ControllerPath + "/Modifier/" + Table.SelectedId;
        else
            alert(SelectItemErrorMessage);
    });

    // Mise en place du double clic sur la table
    $("#" + Table.tableName + "> tbody > tr").dblclick(function (e) {
        if (Table.SelectedId != null)
            location.href = ControllerPath + "/Modifier/" + Table.SelectedId;
        else
            alert(SelectItemErrorMessage);
    });

    // Bouton de suppression
    $('#btnRemove').click(function () {
        if (Table.SelectedId != null) {
            if (confirm(ConfirmDeleteMessage)) {
                $.post(ControllerPath + "/Archiver/" + Table.SelectedId)
                    .done(function () {
                        Table.DataTable.fnDeleteRow($('#' + Table.SelectedId));
                    })
                    .fail(function () {
                        alert("Une erreur est survenue lors de la mise en archive");
                    });
                return true;
            }
        }
        else
            alert(SelectItemErrorMessage);
    });

    // Redirection pour l'annulation
    $('#btnCancel').click(function () {
        location.href = ControllerPath;
    });
}