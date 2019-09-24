$(document).ready(function () {
    InitCustomTable("Vols", ControllerPath, "Vous devez sélectionner un vol", "Êtes-vous sûr de vouloir archiver ce vol ?");

    // Le format des dates est forcée au format français mais il faudrait gérer ça autrement pour une application internationale
    // Datetime picker
    var dateDebutValue = $('#dateDebut').val();
    var dateDebutPicker = $('#dateDebut').datetimepicker({
        format: "DD/MM/YYYY hh:mm:ss"
    });
    dateDebutPicker.val(dateDebutValue);

    // Datetime picker
    var dateFinValue = $('#dateFin').val();
    var dateFinPicker = $('#dateFin').datetimepicker({
        format: "DD/MM/YYYY hh:mm:ss"
    });
    dateFinPicker.val(dateFinValue);
});