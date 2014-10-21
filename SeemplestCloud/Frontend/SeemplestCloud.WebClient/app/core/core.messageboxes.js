var Core;
(function (Core) {
    var MessageBox;
    (function (MessageBox) {
        function confirm(message, onSuccess) {
            bootbox.confirm(message, onSuccess);
        }
        MessageBox.confirm = confirm;
    })(MessageBox || (MessageBox = {}));
})(Core || (Core = {}));
//# sourceMappingURL=core.messageboxes.js.map
