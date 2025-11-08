# Exemplo de landpage para adesão de usuário integrada ao Keycloak e Google 

Este é um exemplo de como implementar uma _landpage_ para adesão de usuários em ASP.NET Core
integrado ao Keycloak e Google.

O objetivo aqui é ter uma página customizada onde o usuário apenas escolha se inscrever
clicando em um botão do "Google" e nada mais. Após sua adesão ele já terá um usuário
válido cadastrado no Keycloak, onde ele poderá fazer login usando sua conta do Google.

## Iniciando no desenvolvimento

Primeiro você precisará provisioar os serviços de pré-requisitos:
```sh
docker compose up -d
```

- Keycloak estará disponível em http://localhost:8081
  - As credenciais de desenvolvimento estão disponíveis em`docker-compose.yml`
  - Crie um realm `signup-keycloak-google`
    - Crie um client `signuplangpage.client.langpage` com "Client authentication" e "Service accounts roles" habilitados
      - Atribua a _role_ "manage-users" ao cliente em "Service accounts roles"
      - Use as credenciais para configurar o cliente Keycloak da API
    - Adicione o provedor de login social Google a configuração do realm
      - Alias: `google`, Display name: `Google`

```
http://localhost:8081/realms/signup-keycloak-google/protocol/openid-connect/auth?client_id=frontend-app&redirect_uri=http://localhost:3000&response_type=code&scope=openid
```

Agora você vai precisar configurar os segredos de sua aplicação:
```sh
# Shell Script
./eng/secrets-set.sh "<chave>" "<valor>"
```

```powershell
# PowerShell
.\eng\secrets-set.ps1 "<chave" "<valor>"
```

- `Authentication:Google:ClientId` com o _client id_ da aplicação Google
- `Authentication:Google:ClientSecret` com o segredo da aplicação Google
- `Keycloak:Credentials:Secret` com o segredo da aplicação no Keycloak

Por fim, você pode desenvolver sua aplicação:
```sh
dotnet restore

# Se for desenvolver sem TDD
./eng/dev-watch.sh

# Se for desenvolver com TDD
./eng/dev-watch-test.sh

# Isso é tudo que você precisa para começar a codificar com
# a aplicação disponível em http://localhost:8080
```
