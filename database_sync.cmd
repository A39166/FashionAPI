dotnet ef dbcontext scaffold 'Server=localhost;Database=ecmweb;Uid=root;Allow Zero Datetime=true;convert zero datetime=True;old guids=True;'Pomelo.EntityFrameworkCore.MySql -c DBContext -o Databases/FashionDB -f -v --no-build --no-onconfiguring --no-pluralize