var Core;
(function (Core) {
    (function (MessageBox) {
        function confirm(title, message, onSuccess) {
            bootbox.dialog({
                title: title,
                message: message
            });
        }
        MessageBox.confirm = confirm;
    })(Core.MessageBox || (Core.MessageBox = {}));
    var MessageBox = Core.MessageBox;
})(Core || (Core = {}));
//# sourceMappingURL=core.utility.js.map
