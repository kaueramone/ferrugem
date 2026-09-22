# Refinamento FPS — versão 0.3.0

Implementação iniciada em 22/09/2026 após aprovação do usuário. Este documento
registra o novo recorte; os resultados 0.2.0 não validam automaticamente estas
mudanças. A preparação, o Motor e as regressões da build final passaram; houve inspeção visual parcial, com playtest completo e escuta ainda pendentes.

## Controles e apresentação

| Controle | Ação |
| --- | --- |
| WASD / Shift | Andar / correr. |
| Ctrl esquerdo | Manter agachado; soltar só permite levantar se houver espaço. |
| Espaço | Pular. |
| Mouse | Olhar. O primeiro clique captura sem disparar ou aplicar o salto de posição do cursor. |
| Botão direito mantido | Mirar pela mira de ferro. |
| Botão esquerdo / R / G | Disparar / recarregar / lançar carga. |
| Esc | Liberar o cursor. |

A mira aproxima suavemente o campo de visão de 80 para 58 graus e reduz a
sensibilidade de 0,12 para 0,066 graus por unidade de deslocamento do mouse. São
parâmetros de teste, não valores medidos ou copiados de POLYGON. A câmera usa a
postura efetiva do personagem e permanece em primeira pessoa. O marcador central
é ocultado ao mirar; a mira de ferro do modelo passa a orientar o alinhamento.

Mãos, punhos e manga são geometria provisória original montada por código. A
recarga desloca a mão de apoio e abre o tambor; disparo, balanço e movimento da
arma têm animação procedural. Ainda não são uma animação final capturada nem arte
anatômica final. O corpo remoto agacha com ajuste do quadril e duas cadeias de
pernas, preservando os pés; não é encolhido uniformemente.

Disparo, recuo, som do tiro e indicação de acerto continuam ligados à confirmação
do servidor. Isso evita apresentar dano não confirmado, mas adiciona atraso à
sensação de disparo quando a latência aumenta. A resposta da arma sob latência
precisa de playtest; não declaramos equivalência à sensação de POLYGON. Os efeitos
remotos acompanham a sequência replicada, sem repetir tiros antigos ao entrar.

## Movimento e campo de teste

O controlador acrescenta aceleração/parada, salto, gravidade, agachamento, degraus
e rampa definidos na geometria compartilhada. O campo continua pequeno; não é o
terreno final de 64 km². A rampa, plataforma, escada e passagem baixa são montadas
visual e logicamente a partir dos mesmos dados. Paredes continuam sendo sólidas.
O estado de mira reduz a velocidade de deslocamento.

A colisão permanece cinemática e específica do protótipo, sem física geral,
empurrão entre jogadores, veículos ou terreno destrutível. O servidor usa postura
e altura efetivas também ao avaliar tiro e ataque. A câmera suaviza a transição
vertical; sua imagem precisa ser conferida ao passar por teto baixo e degraus.

## Áudio original de validação

Seis efeitos são sintetizados por código em memória: disparo, recarga, passos,
impacto confirmado, antecipação do zombie e explosão. São sinais provisórios, não
gravações reais nem arquivos obtidos de pacotes. Nenhuma ferramenta generativa
externa ou serviço de áudio foi usado. A implementação recebeu assistência de IA.

Há um conjunto limitado de 24 vozes; efeitos remotos usam posição no mundo e
atenuação com distância máxima de 35 m. Sons locais de arma e passos ficam no
ouvinte. O áudio só é criado no cliente. Eventos de inicialização/reprodução no
log confirmam o caminho de código, mas não demonstram que alguém ouviu o resultado.
Volume, timbre, repetição e localização sonora precisam de escuta humana.

## Verificação proposta

```powershell
.\Testar-Ferrugem.bat -Smoke -Motor
```

O modo automatizado envia movimento, corrida, salto, agachamento e mira pelos
mesmos inputs de rede usados pelo jogador. Deve verificar estado confirmado no
servidor, observação pelos clientes e repetição após reconexão. Execute separado
dos modos `-Fps` e `-Combat`, que continuam sendo regressões próprias.

Para repetir com condições simuladas de rede:

```powershell
.\Testar-Ferrugem.bat -Smoke -Motor -LatencyMs 50 -PacketLoss 2
```

Esse perfil aplica 50 ms adicionais no envio e no recebimento dos clientes:
aproximadamente 100 ms extras de ida e volta, com perda configurada de 2% em cada
direção. Isso é configuração do simulador, não uma medição de RTT. Não adiciona
jitter, duplicação ou corrupção. O launcher exige a confirmação oficial de que o
simulador está ativo; não aceita apenas a presença do arquivo. O pacote condiciona esse recurso ao símbolo `NETCODE_DEBUG`: a configuração Development, sozinha, não o habilitou nesta versão. O processo de build deve definir explicitamente esse símbolo somente nos builds Development usados no teste.

A integração utiliza o formato do pacote Netcode em
`Runtime/Connection/NetworkSimulatorSettings.cs` e `DefaultDriverConstructor.cs`,
com o simulador do Transport em `Runtime/Pipelines/SimulatorUtility.cs`. Os
pacotes fixados no repositório são a referência dessas APIs.

No teste manual, conferir alinhamento da mira, retomada do cursor, recarga,
agachamento remoto, teto baixo, escada, rampa, salto, volumes de áudio e sons de
outros jogadores. A revisão não deve confundir mudança de FOV com acerto confirmado
nem assumir que som no log equivale a qualidade sonora aprovada.

## Facções aprovadas

- **Os Sem Cova — vermelho.**
- **Mato Sem Cachorro — verde.**
- **Última Gota — azul.**

Lore e planta foram atualizadas sem mudar a geometria: Mato Sem Cachorro fica na
posição A ao norte, Os Sem Cova em B a sudoeste e Última Gota em C a sudeste.
Nomes e letras acompanham as cores. As três equipes ainda são uma definição de
design: o protótipo não atribui facções usando o ID da conexão.

## Histórico e evidências de validação

Em 22/09/2026, a preparação no Editor terminou com saída 0. O registro
`Logs/prepare-motor.log` contém `MOTOR_SUITE_PASS count=19`: verificações isoladas
de aceleração, parada, velocidades, salto/pouso, ausência de salto duplo, postura,
teto, degraus, rampa, queda, limite de inclinação, colisão de raio, altura de cabeça
e impedimento de subir uma parede. Isso confirma essas verificações de lógica;
não substitui o smoke multiplayer nem a inspeção visual e sonora.

Ao iniciar o teste, o WSL voltou a apresentar `0x80070422`: `WslService` estava
parado e desativado. Foi restaurado somente esse serviço para inicialização manual
e execução ativa; Ubuntu-24.04 foi confirmado antes de retomar o `.bat`. Esse reparo
de ambiente não é resultado de teste do jogo. Builds e smoke da 0.3.0 continuam
pendentes neste ponto do registro.

A primeira rodada após o reparo do WSL terminou em falha, registrada em
`Logs/LinuxSmoke/20260922-064936-145/result.json` como timeout do cliente A.
A investigação também identificou que o simulador não havia sido compilado por
falta de `NETCODE_DEBUG`; por isso essa rodada não valida latência ou perda de
pacotes, mesmo tendo produzido eventos de movimento. O símbolo foi corrigido e os dois alvos recompilados para as rodadas seguintes.

A rodada Motor em `Logs/LinuxSmoke/20260922-065833-337` confirmou o simulador
ativo nos dois clientes, mas falhou na observação local de salto. Servidor e
clientes remotos registraram saltos; isso não basta para aprovar o requisito
local. O diagnóstico foi ajustado para observar ascensão e registrar altura, velocidade vertical e contato com o chão; a aprovação posterior está registrada na tabela da build final abaixo.

A regressão de combate passou em **53 verificações**, com 50 ms adicionais por
direção e perda de 2% por direção, em
`Logs/LinuxSmoke/20260922-071927-977/result.json`. Ambos os clientes registraram
`AUDIO_READY` e os eventos de tiro e recarga; isso verifica execução desses
caminhos, não escuta humana. Não houve evento de áudio de explosão observado
nessa evidência. O aviso conhecido de alocação no encerramento do Unity permanece.
A regressão FPS também passou em **27 verificações**, registrada em `Logs/LinuxSmoke/20260922-072111-141/result.json`. Naquele momento, a nova rodada Motor ainda não havia sido concluída. A primeira tentativa de inspeção manual foi interrompida pelo usuário; uma nova inspeção foi autorizada para depois dos builds. Naquele momento, não havia inspeção visual concluída nem aceite sonoro; a inspeção parcial posterior está registrada abaixo.

### Rodada final do motor

O teste Motor passou em **60 verificações** em
`Logs/LinuxSmoke/20260922-081022-887/result.json`, com simulador ativo de 50 ms e
2% de perda por direção. Houve movimento, salto, pouso, agachamento e mira
confirmados pelo servidor e observados local e remotamente, incluindo reconexão.
O diagnóstico local registrou ascensão real: A com altura 0,121 m e velocidade
vertical 5,618 m/s; B com 0,259 m / 5,140 m/s e, após reconectar, 0,375 m / 4,703 m/s.
Ambos registraram inicialização de áudio e passos; a qualidade sonora não foi ouvida.

A tentativa imediatamente anterior (`Logs/LinuxSmoke/20260922-080234-192`)
falhou porque o cliente B não completou a conexão e o ciclo terminou em
`CYCLE_INCOMPLETE` após 45 s. A repetição usou a mesma build e hash, sem alterar
parâmetros ou critérios. A causa dessa falha transitória permanece sem explicação;
o sucesso posterior não apaga essa ocorrência. As regressões Combat e FPS sobre a build final foram concluídas posteriormente, conforme a tabela abaixo.

### Regressões sobre a mesma build final

| Teste | Resultado | Registro |
| --- | --- | --- |
| Motor, 50 ms / 2% por direção | PASS, 60 verificações | `Logs/LinuxSmoke/20260922-081022-887/result.json` |
| Combat, 50 ms / 2% por direção | PASS, 53 verificações | `Logs/LinuxSmoke/20260922-081128-406/result.json` |
| FPS, sem simulador | PASS, 27 verificações | `Logs/LinuxSmoke/20260922-081227-203/result.json` |

As três rodadas usaram os mesmos builds finais. O áudio registrou inicialização
nos dois clientes, passos no Motor e tiro/recarga no Combat. O aviso conhecido
`Leak Detected` no encerramento permaneceu nas três rodadas. A inspeção manual seguinte, registrada em `Logs/LinuxSmoke/20260922-081331-105`, teve o alcance parcial descrito abaixo. Estes testes não demonstram capacidade para 100 jogadores.

### Inspeção visual parcial da build final

Na sessão `Logs/LinuxSmoke/20260922-081331-105`, capturas reais do cliente
conectado mostraram HUD em PT-BR legível, mãos e revólver renderizados, coberturas,
escada e teto baixo visíveis. O clique de captura preservou a munição em 6/6.
Foi observado o aviso **VOCÊ MORREU** com a arma oculta; ao renascer, a arma,
vida 100 e proteção reapareceram. Não foi identificado bloqueio gráfico nessa
inspeção parcial.

Os infectados alcançaram o ponto de nascimento e provocaram mortes repetidas,
impedindo concluir a tentativa de disparo entre capturas. Esse comportamento
requer avaliação do posicionamento inicial e do ritmo do teste. A inspeção não
aprova tiro manual, alinhamento da mira, recarga, agachamento remoto, travessia
dos obstáculos nem escuta dos efeitos. Esses itens continuam no roteiro manual;
os eventos automáticos de rede e áudio não substituem esse aceite.
