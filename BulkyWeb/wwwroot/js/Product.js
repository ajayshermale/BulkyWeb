var dataTable;

$(document).ready(Function())
{
    loadDataTable();
}

function loadDataTable() {

    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/product/getAll' },
        "columns": [
            { data: "title", "width": "5%" },
            { data: "description", "width": "15%" },
            { data: "isbn", "width": "5%" },
            { data: "author", "width": "5%" },
            { data: "listPrice", "width": "1%" },
            { data: "price", "width": "1%" },
            { data: "price50", "width": "1%" },
            { data: "price100", "width": "1%" },
            { data: "category.name", "width": "5%" },
            {
                data: "imageUrl", "width": "15%",
                "render": function (data) {
                    return `
                    <div class="card border-0 p-3 shadow border-top border-5 rounded">
                    <img src="${data}" class="card-img-top rounded" />
                    </div>
                    `
                }
            },
            {
                data: "id",
                "render": function (data) {
                    return `
                    <div class="w-75 btn-group" role="group">
                   <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2" r"><i class="bi bi-pencil-square"></i>Edit</a>
                   <a onClick="Delete('/admin/product/DeleteAPI?id=${data}')" class="btn btn-danger mx-2" r"><i class="bi bi-trash-fill"></i>Delete</a>
                   </div>
                   `

                }
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


//"id": 5,
//    "title": "Book Hist2",
//        "description": "<p>sssssss</p>",
//            "isbn": "ssssss",
//                "author": "Ajay",
//                    "listPrice": 100,
//                        "price": 90,
//                            "price50": 80,
//                                "price100": 70,
//                                    "categoryId": 1,
//                                        "category": {
//    "id": 1,
//        "name": "Hist",
//            "dispalyOrder": 1
//},
//"imageUrl": "\\image\\product\\c7d48ab8-507d-4eb8-bd89-c458c4741f88.jpeg"
//    },
//function loadDataTable() {
//    dataTable = $('#tblData').DataTable({
//        "ajax":
//        {
//            "url": "/admin/product/getAll",
//            "type": "Get",
//            "dataType": "JSON",
//            "dataSrc": function (json) {
//                // Settings.
//                jsonObj = $.parseJSON(json.data)

//                // Data
//                return jsonObj.data;
//            }
//        },
//        "Columns": [
//            /*{ data: "id" },*/
//            { data: "title"}
//                ]
//    });
//}