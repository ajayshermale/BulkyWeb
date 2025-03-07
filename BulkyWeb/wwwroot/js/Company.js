﻿var datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dateTable = $('#tblDataCompany').DataTable({

        "ajax": { url: '/admin/company/getAll' },
        "columns": [
            { "data": "name", "width": "5%" },
            { "data": "streetAddress", "width": "5%" },
            { "data": "city", "width": "5%" },
            { "data": "state", "width": "5%" },
            { "data": "postalCode", "width": "5%" },
            { "data": "phoneNumber", "width": "5%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role = "group" >
                            <a href="/admin/company/upsert?id=${data}" class="btn btn-primary mx-2" r"><i class="bi bi - pencil - square"></i>Edit</a>
                            <a onClick = "Delete('/admin/company/DeleteAPI?id=${data}')" class="btn btn-danger mx-2" r"><i class="bi bi - trash - fill"></i>Delete</a>
                            </div>`
                },
                "width": "25%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'delete',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }

            })
        }
    });
}