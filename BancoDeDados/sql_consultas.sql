select * from convenio;

select * from pessoa;

select * from paciente;

select * from endereco;

delete from paciente where id > 0;
delete from endereco where id > 0;
delete from pessoa where id > 0;

select * 
from pessoa p
join endereco e;

select *
from paciente
where id = @id_paciente;

UPDATE pessoa
SET nome = @nome,
fl_status = @fl_status
where id = @id;

UPDATE paciente
set fl_status = @fl_status,
paciente_risco = @paciente_risco,
fl_obito = @fl_obito,
id_convenio = @id_convenio
where id_pessoa = @id;

UPDATE endereco
set CEP = @cep,
rua = @rua,
numero = @numero,
bairro = @bairro,
cidade = @cidade,
uf = @uf
where id_pessoa = @id_pessoa;

select  
	pe.id as Id_Pessoa, 
	pe.fl_status as 'Status', 
	pe.nome as Nome, 
	pe.cgccpf as CPF, 
	pa.numero_prontuario as N_Prontuário, 
	pa.paciente_risco as Risco, 
    pa.fl_obito as 'Obito?',
    c.id as Id_Convenio,
	c.nome as Convenio,
    e.UF,
    e.Cidade,
    e.CEP,
    e.Bairro,
    e.Rua,
    e.Numero
from paciente pa
	inner join pessoa pe on pa.id_pessoa = pe.id
    inner join convenio c on pa.id_convenio = c.id
    inner join endereco e on pe.id = e.id_pessoa;
    
