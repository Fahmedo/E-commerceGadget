var dataTable;


$(document).ready(function () {
    loadDataTable();
});
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Technician/Getall"
        },
        "columns": [
            { "data": "companyName", "width": "15%" },
            { "data": "streetAddress", "width": "15%" },
            { "data": "city", "width": "15%" },
            { "data": "state", "width": "15%" },
            { "data": "emailAddress", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },


            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="/Admin/Technician/Upsert?id=${data}"
                        class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> &nbsp; Edit</a>

                        <a onClick=Delete('/Admin/Technician/Delete/${data}')
                        class="btn btn-danger mx-2">   <i class="bi bi-trash-fill"></i> &nbsp; Delete </a>

                         <a href="/Admin/Technician/Details?id=${data}"
                        class="btn btn-primary mx-2"> <i class="bi bi-ticket-detailed"></i> &nbsp; Details</a>
                    </div>

                        `




                },
                "width": "15%"
            }


        ]


    });

}


function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message)
                    }
                    else {
                        toastr.error(data.message)
                    }
                }
            })
        }
    })
}