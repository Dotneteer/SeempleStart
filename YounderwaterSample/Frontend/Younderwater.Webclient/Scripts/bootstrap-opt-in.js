//$("*[data-toggle='tooltip']").tooltip({ template: '<div class="tooltip" role="tooltip"><div class="tooltip-inner"></div></div>' });
$("*[data-toggle='errorhint']").tooltip({
    template: '<div class="tooltip" role="tooltip"><div class="tooltip-inner errorhint"></div></div>',
    placement: 'left'
});
$("*[data-toggle='tooltip']").tooltip();