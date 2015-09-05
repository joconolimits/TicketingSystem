$(function () {
    //$.extend(true, $.fn.dataTable.defaults, {
    //    "searching": true,
    //    "ordering": false
    //});
    $.fn.dataTable.ext.type.detect.unshift(
         function (d) {
             return d === 'Pending' || d === 'WaitReply' || d === 'Closed' ?
                 'status' :
                 null;
         }
     );

    $.fn.dataTable.ext.type.order['status-pre'] = function (d) {
        switch (d) {
            case 'Pending': return 1;
            case 'WaitReply': return 2;
            case 'Closed': return 3;
        }
        return 0;
    };



    $('table').DataTable({
        "order": [[2, "asc"]]
    }
 );
})
                  