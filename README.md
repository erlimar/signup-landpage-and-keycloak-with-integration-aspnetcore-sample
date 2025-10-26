# Exemplo de landpage para adesão de usuário integrada ao Keycloak e Google 

Este é um exemplo de como implementar uma _landpage_ para adesão de usuários em ASP.NET Core
integrado ao Keycloak e Google.

O objetivo aqui é ter uma página customizada onde o usuário apenas escolha se inscrever
clicando em um botão do "Google" e nada mais. Após sua adesão ele já terá um usuário
válido cadastrado no Keycloak, onde ele poderá fazer login usando sua conta do Google.

## Iniciando no desenvolvimento

```sh
dotnet restore
dotnet tool restore
./dev-watch.sh

# Isso é tudo que você precisa para começar a codificar com
# a aplicação disponível em http://localhost:8080
```
