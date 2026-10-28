Create database labSamples2;

use labSamples2;

create table Patient (
IdPatient int identity not null,
DocumentNumber int not null,
FullName Nvarchar(100),
CreatedAt datetime not null,
constraint PK_IdPatient PRIMARY KEY (IdPatient),
constraint UQ_DocumentNumber unique (DocumentNumber), --no permite documentos de paciente repetidos
);

create table Test(
IdTest int identity not null,
CodeTest nvarchar(20),
NameTest Nvarchar(100),
constraint PK_IdTest PRIMARY KEY (IdTest),
constraint UQ_CodeTest unique (CodeTest),
);

create table Samples(
IdSample int identity not null,
PatientId int not null,
SampleType Nvarchar(10)  not null check (SampleType in ('Sangre','Orina', 'Heces', 'Isopado')),
CollectedAt datetime not null,
Status Nvarchar(10) not null default 'Registrada' check (Status in ('Registrada','Recibida', 'Procesada', 'Rechazada')),
RejectionReason Nvarchar(200) null,
CreatedAt datetime not null,
constraint PK_IdSample PRIMARY KEY (IdSample),

);

create table SampleTest(
SampleId int not null references Samples(IdSample),
TestId int not null references Test(IdTest),
primary key (SampleId,TestId) -- no se puede repetir examenes en la misma muestra
);

insert into Test (CodeTest,NameTest) Values 
('HEM','HEMOGRAMA COMPLETO'),
('COL','COLESTEROL'),
('HIP','HIPERTENSION'),
('VIH','SIDA'),
('VPH','PAPILOMA');

select * from Test