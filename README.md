# Exemplo de landpage para adesão de usuário integrada ao Keycloak e Google 

Este é um exemplo de como implementar uma _landpage_ para adesão de usuários em ASP.NET Core
integrado ao Keycloak e Google.

O objetivo aqui é ter uma página customizada onde o usuário apenas escolha se inscrever
clicando em um botão do "Google" e nada mais. Após sua adesão ele já terá um usuário
válido cadastrado no Keycloak, onde ele poderá fazer login usando sua conta do Google.

## Iniciando no desenvolvimento

Primeiramente você vai precisar configurar o ID e segredo de sua aplicação Google:
```sh
./eng/secrets-set.sh Authentication:Google:ClientId "<valor>"
./eng/secrets-set.sh Authentication:Google:ClientSecret "<valor>"
```

```sh
dotnet restore
dotnet tool restore

# Se for só executar a aplicação
./eng/dev-watch.sh

# Se for desenvolver com TDD
./eng/dev-watch-test.sh

# Isso é tudo que você precisa para começar a codificar com
# a aplicação disponível em http://localhost:8080
```
