# IdolChatAI
* Um app de mensagens com IA para conversar com personagens pré-criados.
---
## Proposta

Esse é um projeto para a matéria de Inteligência Artificial, 
seu objetivo é uma interface multiplataforma (mobile, web e desktop) 
onde o usuario possa escolher um contato para conversar.
É um projeto bem simples que utiliza de uma conexão com o **Groq** para responder as mensagens.

Ele possui os seguites objetivos inicais:
- interface:
  * Tela de contatos (escolher com quem irá conversar);
  * Tela de conversa (enviar e receber as mesnages, visualizar cards de mensagens);
- código:
  * Conexão via api com IA;
  * Envio de requisições e recebimento de respostas;
  * Json dos contatos disponiveis para conversa, assim como instruções de personalidade;
---
## Ferramentas e Recursos

Utilizamos:
* Unity para desenvolver o app de forma que possa ser utilizado multiplataforma;
* Groq para conectar com a IA devido a ser gratuita com um bom limite diário e rápida;
---
## Como usar?

1. Clonar repositorio [IdolChatAi](https://github.com/Gustavo-Gomide/IdolChatAi):
   * Abra o terminal na pasta desejada;
   * Digite: git clone https://github.com/Gustavo-Gomide/IdolChatAi.git
2. Configure a chave de API:
   * Vá para o site do [Groq](https://console.groq.com/keys) e crie sua chave.
   * Abra o arquivo [config.json](https://github.com/Gustavo-Gomide/IdolChatAi/blob/main/Assets/StreamingAssets/config.json) (Assets/StreamingAssets/config.json) em seu  editor de código.
   * Cole a chave de API no campo **apiKey**
3. Rodar:
   * Abra o a pasta clonada no unity e rode a aplicação dentro da engine.
   * Caso queira buildar:
     * Vá em "file";
     * Clique em "Build Profiles" e faça as configurações necessarias;

---
## Futuro do app

Ainda está em desenvolvimento e sujeito a melhorias, 
futuramente pode ser desenvolvido uma API para gerar conexão com o groq para permitir que pode ser passado a chave de api em tempo de execução
ou um campo para o usuario passar sua chave do groq para ser utilizada.

<p align="center">
  <img src="https://media.githubusercontent.com/media/Gustavo-Gomide/IdolChatAi/main/Assets/Images/dragon_metal.png" height="200" alt="logo Dragão">
</p>
