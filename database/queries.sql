-- todos los examenes con las veces solicitadas 
 select t.CodeTest, t.NameTest, count(st.SampleId) as vecesSoli
from Test t
left join SampleTest st on st.TestId = t.IdTest
group by t.CodeTest, t.NameTest, t.IdTest
order by vecesSoli


-- pacientes con muestras rechazadas
use labSamples2;
select p.DocumentNumber, p.FullName, count(*) as muestras_Rechazadas
from Patient p
join Samples s on s.PatientId = p.IdPatient
where s.Status = 'Rechazada'
group by p.IdPatient,  p.DocumentNumber, p.FullName

