Projeto de geração e entrega de arquivos PDF a partir de informações persistidas por outros módulos do ecossistema.

## Objetivo da arquitetura

O projeto PDF é uma API .NET 8 criada para gerar arquivos PDF a partir de dados já persistidos em dois contextos de dados: o banco RAG, que guarda documentos importados, e o banco MLNet, que guarda modelos de treinamento. A ideia central é expor um endpoint HTTP que consulta o repositório correspondente, monta o conteúdo textual e gera um PDF na pasta Downloads do usuário com nome fixo, como mostra o fluxo do controlador em ImprimirDocumentosController.cs e dos handlers em ImprimirDocumentosHandler.cs e ImprimirModelosHandler.cs.

Em termos de arquitetura, o projeto segue uma arquitetura em camadas, bem visível na composição de startup da API em Program.cs:

- Camada de API/Apresentação: controllers, endpoints, Swagger e middleware.
- Camada de Aplicação: rotas/handlers, requests/responses e a lógica de geração do PDF.
- Camada de Domínio: entidades e contratos de repositórios, como as interfaces de acesso aos dados.
- Camada de Infraestrutura: implementações de acesso a SQL Server via contextos específicos para RAG e MLNet, em RagContexto.cs e MlNetContexto.cs.

Os padrões arquiteturais mais evidentes são:

- Injeção de dependência, com registro de serviços no arquivo InjecaoDependenciaConfiguracao.cs.
- CQRS-ish/Handler-based, porque o projeto separa Request e Handler para cada operação, com registro em CqrsConfiguracao.cs. Isso facilita o fluxo “request → handler → repositório → resposta”.
- Repository pattern, porque as consultas a SQL ficam encapsuladas nos contextos de infraestrutura, e o domínio depende de interfaces de repositório.
- Middleware pipeline, com autenticação por chave de API e configuração de middlewares em ConfiguracaoMiddleware.cs e ApiKeyMiddleware.cs.
- Resposta padronizada por contrato de operação, com ResultadoOperacao, para sinalizar sucesso ou erro de forma consistente.

Em resumo, o projeto é um gerador de PDFs a partir de bases de documentos e modelos de ML, usando uma API ASP.NET Core com separação em camadas, DI, CQRS-style handlers, middleware de segurança e o padrão repositório para abstrair acesso ao SQL Server.

## Fluxo

1. A API apenas entrega o endpoint e recebe o request.
2. A aplicação chama a rota de handler com o contrato do domínio.
3. O handler dispara o repositório de orientação de origem definido na infraestrutura.
4. A infraestrutura usa o contexto RAG ou MLNet para consultar a base correta.

## Regras de manutenção

1. A API não conhece SQL nem a origem do dado diretamente.
2. A aplicação usa contratos da camada de domínio e não acoplamentos de infraestrutura.
3. A infraestrutura deve manter o acesso ao banco RAG/MLNet em contextos com nomes de origem explícitos.
4. Novas integrações devem sempre criar entidades, contratos e implementações com namespace de origem identificável.
