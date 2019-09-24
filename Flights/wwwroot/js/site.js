// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code
class DataTableCustom {
    constructor(tableName) {

        // Identifiant null
        this.SelectedId = null;

        // Element
        this.Element = null;

        // ClassName
        this.ClassName = null;

        // Nom de la table
        this.tableName = tableName;

        // Table
        this.DataTable = $('#' + tableName).dataTable();

        // Variable de sauvegarde de la classe
        var _self = this;

        // Mise en place du clic sur les tables
        $("#" + tableName + "> tbody > tr").click(function (e) {
            $("#" + tableName + "> tbody > tr").removeClass("selected");
            $(e.target).closest("tr").addClass("selected");
            _self.SelectedId = $(e.target).closest("tr").attr("id");
            _self.Element = $(e.target);
            _self.ClassName = e.target.closest("tr").querySelector("td > i").getAttribute("class");
        });
    }
}

var ControllerPath = null;
var PersonneId = null;
var CurrentItemId = null;

function setControllerPath(URL)
{
    ControllerPath = URL;
}

function setPersonneId(ID) {
    PersonneId = ID;
}

function setCurrentItem(ID) {
    CurrentItemId = ID;
}

$(document).ready(function () {

    // Input Mask
    var inputmasks = $('[inputmask]');

    for (var i = 0; i < inputmasks.length; i++) {
        var mask = $(inputmasks[i]).attr("inputmask");
        $(inputmasks[i]).inputmask(mask);
    }

    // Language pour les tables
    $('.table-full-features').DataTable({
        "language": {
            "decimal": "",
            "emptyTable": "Aucune donnée disponible",
            "info": "Affichage _START_ à _END_ sur _TOTAL_ résultats",
            "infoEmpty": "Affichage 0 à 0 sur 0 résultats",
            "infoFiltered": "(Filtré sur _MAX_ résultats)",
            "infoPostFix": "",
            "thousands": ",",
            "lengthMenu": "Afficher _MENU_ résultats",
            "loadingRecords": "Chargement...",
            "processing": "En traitement...",
            "search": "Recherche:",
            "zeroRecords": "Aucun enregistrements correspondants trouvés",
            "paginate": {
                "first": "Première page",
                "last": "Dernire page",
                "next": "Page suivante",
                "previous": "Page précédente"
            },
            "aria": {
                "sortAscending": ": Activer pour trier la colonne par ordre croissant",
                "sortDescending": ": Activer pour trier la colonne par ordre décroissant"
            }
        }
    });
});
