Documentação Desafio Magalu
Escolhi a linguagem ASP.NET Core devido à sua facilidade de uso e rapidez no desenvolvimento e testes.

O projeto é dividido em duas partes: a Web API e o projeto de testes.

Na Web API, como um dos requisitos principais era a simplicidade, optei por usar o Dapper como ORM, já que os acessos ao banco seriam poucos e todas as consultas seriam simples.

Criei a pasta Data, onde centralizei a classe DatabaseService, responsável por todos os métodos de acesso ao banco de dados.

Para o controller, segui a convenção padrão e criei dois métodos:

POST: Responsável por receber o arquivo.
GET: Responsável por devolver a consulta.
No método POST (upload), implementei algumas validações diretamente no controller para evitar o recebimento de arquivos vazios ou com extensão inválida. Também defini um limite máximo de 2MB por arquivo para upload.

Quanto ao armazenamento do arquivo, optei por usar MemoryStream, em vez de copiá-lo para o servidor onde a API estiver publicada. Essa escolha se deve ao fato de que os arquivos são pequenos e essa abordagem evita consumo desnecessário de armazenamento.

Processamento do Arquivo
Para processar o arquivo, criei a pasta Mapping, onde implementei uma interface e sua implementação para as validações de cada propriedade extraída. O uso dessa abordagem facilita a testabilidade do código posteriormente.

Além das entidades base do desafio (Usuário, Pedido e Produto), criei uma nova entidade chamada ValidaImportacao, que armazena o estado de cada objeto — ou seja, se foi recuperado com sucesso do arquivo ou não. Dentro das entidades principais, há uma referência a essa nova entidade, sendo os dados populados nela dentro da classe de Mapping.

Também desenvolvi um repositório, que é responsável pela leitura do arquivo. Após a leitura de cada linha, a classe de validação é acionada. Com o retorno dessa validação, é possível determinar se os dados foram salvos no banco ou se a linha foi descartada.

Inserção dos Registros
Para evitar duplicação de dados, implementei uma validação que impede a inserção de pedidos repetidos ou produtos duplicados para o mesmo usuário. Isso previne a repetição de registros caso um usuário faça upload do mesmo arquivo mais de uma vez por engano.

Percebi que alguns pedidos possuíam ID igual a zero. Como não havia nenhuma especificação na documentação informando que esse valor era inválido, considerei esses pedidos como normais e segui o fluxo de inserção.

A estrutura das tabelas foi criada de forma simples. Poderia ter adicionado uma tabela extra para armazenar informações como nome do arquivo, data de upload, usuário logado e quantidade de registros bem-sucedidos e rejeitados, mas não sabia se isso fazia parte da solução esperada.

Retorno Pós-Upload
Após o upload, a API retorna um arquivo ZIP chamado usuarios.zip, contendo três arquivos JSON:

processados.json: Lista dos registros importados com sucesso.
rejeitados.json: Lista dos registros rejeitados e os motivos da rejeição.
detalhamento.json: Contém uma única linha informando quantos registros foram importados e quantos foram rejeitados.

Consulta de Dados (GET)
O método GET do controller Magalu foi implementado seguindo os filtros especificados na documentação:

Número do pedido (opcional).
Data inicial e final (opcionais).
Para seguir o formato esperado de retorno (Usuário → Pedidos → Produtos), implementei esse agrupamento diretamente no banco de dados usando SQL/Dapper. A query agrupa os produtos e valores em uma única coluna separada por ,, e, no retorno da query, faço a separação e a criação do objeto no formato correto.

Adicionei validações para garantir a integridade da consulta, como:

Verificação da correção das datas.
Checagem se o pedido filtrado existe no banco de dados.
Caso alguma dessas validações falhe, a API retorna um BadRequest(). Se a consulta for bem-sucedida, o retorno é um arquivo ZIP chamado Consulta.zip, contendo um arquivo JSON chamado processados.json.

Organização do Código
Na implementação da Web API, segui os princípios SOLID e dividi as responsabilidades de forma equilibrada, sempre respeitando a simplicidade exigida no desafio.

Documentação Projeto de Testes
Desenvolvi um projeto de testes utilizando xUnit e Moq. Os testes foram aplicados tanto no controller quanto nas interfaces de validação, garantindo a correta normalização dos dados extraídos do arquivo de texto para os objetos.
