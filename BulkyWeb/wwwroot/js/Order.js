var dataTable;

$(document).ready(Function())
{
    //debugger
    var url = window.location.toString();
   // debugger
    if (url.includes("inprocess")) {
        //debugger
        loadDataTable("inprocess");
    }
    else {
        if (url.includes("pending")) {
            loadDataTable("pending");
        }
        else {
            if (url.includes("completed")) {
                loadDataTable("completed");
            }
            else {
                if (url.includes("approved")) {
                    loadDataTable("approved");
                }
                else {
                    loadDataTable("all");
                }
            }
        }
    }
    //debugger
    //loadDataTable();
}

function loadDataTable(status)
{
   // debugger
    dataTable = $('#tblDataOrder').DataTable(
        {
            "ajax": { url: '/Admin/order/getall?status='+status },
            "columns": [
                { data: 'id' },
                { data: 'name' ,"width": "5%" },                
                { data: 'phoneNumber' },
                { data: 'applicationUser.email' },
                { data: 'orderStatus' },
                { data: 'orderTotal' },
                { data: 'orderDate' },
                {
                    data: "id",
                    "render": function (data) {
                        return `
                    <div class="w-75 btn-group" role="group">
                   <a href="/admin/order/details?id=${data}" class="btn btn-primary mx-2" r"><i class="bi bi-pencil-square"></i>Edit</a>                   
                   </div>
                   `

                    }
                }

            ]
        }

    );

}