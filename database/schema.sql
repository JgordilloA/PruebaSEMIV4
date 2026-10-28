Create database labSamples;

use labSamples;


create table Patient (
IdPatient int identity not null,
DocumentNumber int not null,
FullName Nvarchar(100),
CreatedAt date
);