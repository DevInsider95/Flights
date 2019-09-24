function ListBoxItemsToSecondListBox(ListBoxId, ListBoxSecondId) {
    var options = document.getElementById(ListBoxId).options;
    var optionsSecond = document.getElementById(ListBoxSecondId).options;

    for (var i = 0; i < options.length; i++) {
        if (options[i].selected) {
            optionsSecond.add(options[i]);
        }
    }
}

function InitListBoxTransfer(LeftListBoxId, RightListBoxId) {
    $('#to-right').click(function () {
        ListBoxItemsToSecondListBox(LeftListBoxId, RightListBoxId);
    });

    $('#to-left').click(function () {
        ListBoxItemsToSecondListBox(RightListBoxId, LeftListBoxId);
    });
}

function onSumbit(expressionName, ListBoxId) {
    var options = document.getElementById(ListBoxId).options;
    var div = document.getElementById('hiddenList');

    for (var i = 0; i < options.length; i++) {
        div.innerHTML += '<input type="hidden" name=\"' + expressionName + '[' + i + ']' + '\"value=\"' + options[i].value + '\"/>';
    }
}