var dataTable;

$(document).ready(function () {
    loadDataTable("GetWishList")
});

function loadDataTable(url) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/inquiry/" + url
        },
        "columns": [
            { "data": "productName", "width": "20%" },
            { "data": "unitPrice", "width": "15%" },
            { "data": "stockStatus", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="#" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="fas fa-edit"></i>
                            </a>
                        </div>
                    `;
                },
                "width": "5%"
            }
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a  href="#" /* href="/Inquiry/Details/${data}" */class="btn btn-success text-white" style="cursor:pointer">
                                <i class="fas fa-edit"></i>
                            </a>
                        </div>
                    `;
                },
                "width": "5%"
            }
        ]
    });
} 