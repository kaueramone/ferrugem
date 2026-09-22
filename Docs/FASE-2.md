# Fase 2 — Protótipo jogável

> Estado atual: o [refinamento FPS 0.3.0](REFINAMENTO-FPS.md) acrescenta mira de ferro, postura, salto, rampa/escada, mãos e sons procedurais, com verificações automáticas aprovadas; inspeção visual parcial realizada, playtest completo e escuta pendentes. As limitações e resultados dos marcos 2A/2B abaixo descrevem as versões anteriores; a ausência histórica desses recursos não é a descrição da nova implementação.

Esta fase começa depois da conexão validada entre clientes Windows e servidor
Linux. Divisão de escopo acordada: primeiro movimentação FPS; depois combate e
infectados; por último veículo e destruição. Dois clientes não comprovam capacidade
para 100 jogadores.

## Marco 2A — FPS e presença no mundo

Marco validado: campo de testes compacto, movimento do personagem previsto no
cliente e confirmado pelo servidor, câmera FPS e corpo remoto visível entre dois
clientes. O corpo utiliza um civil gratuito de Quaternius; veja
[origens e seleção](ASSETS-PROTOTIPO.md). Nenhuma câmera de terceira pessoa a pé.

Controles do recorte: WASD para andar (3,5 m/s), Shift para correr (6 m/s), mouse
para olhar, clique na janela para capturar o mouse e Esc para soltá-lo. A corrida
ainda não consome estamina. Testar alternando o foco entre as duas janelas.

O campo tem 40 × 40 m e dois obstáculos retangulares. A simulação usa limites e
colisão horizontal simples compartilhados entre cliente e servidor; não é um
controlador final de terreno. Pulo, rampas, escadas, empurrão entre jogadores e
física geral não fazem parte deste marco.

Aceite:

- Dois clientes Windows entram no mesmo servidor e veem um ao outro.
- Cada cliente controla somente o próprio personagem; mover e olhar são independentes.
- Câmera na altura dos olhos, corpo próprio oculto para evitar ver a cabeça por dentro.
- Chão e obstáculos impedem atravessamento; desconectar remove o personagem remoto.
- Reentrar não duplica o personagem. Novo cliente recebe os personagens existentes.
- A build correta continua executável pelos lançadores. Resultados efetivos de
  testes devem ser registrados antes de declarar este marco concluído.

## Marco 2B — Combate e infectados (testes automáticos aprovados)

Uma arma civil, mira, disparo, recarga, munição, vida e respawn. O servidor valida
os acertos e determina as mortes. Zombies caminham lentamente, pressionam rotas
e distraem o jogador; não são chefes, corredores ou esponjas de dano.

- Um ataque corpo a corpo válido do zombie mata o jogador. Encostar em uma
  colisão não basta: alcance e momento do ataque devem ser claros e verificáveis.
- Cada morte causada por zombie cria exatamente um novo zombie no local da morte.
  O atacante continua existindo. O jogador morto retorna pelo respawn humano.
- Repetição de eventos, reconexão ou dano concorrente não podem duplicar a criação.
- Tiro na cabeça ou explosão mata o zombie; tiros no corpo não o matam.
- Mortes PvP não criam zombies neste recorte, salvo futura mudança explícita de regra.
- Poucos infectados no início, perseguição limitada e ataques perceptíveis.
  Quantidade, velocidade e alcance são valores de playtest; não relaxar as regras
  de morte sem revisar a decisão de design.

Aceite: ambos os clientes observam a mesma morte e criação única do infectado;
tiros no corpo não o eliminam; cabeça e explosão o eliminam; um cliente que entra
depois recebe o estado atual. Dano, respawn e transformação não estão no marco 2A.

## Marco 2C — Veículo e destruição (planejado)

Um veículo civil com entrada, saída e direção sincronizadas; terceira pessoa
permitida somente enquanto estiver no veículo. Depois: uma árvore derrubável,
parede modular e pequeno trecho de chão deformável com colisão atualizada.

Aceite: jogadores e zombie respeitam a geometria alterada, novo cliente vê a
destruição anterior e servidor mantém estado consistente. Medir o conjunto com
latência/perda e perfil de CPU antes de expandir mapa ou quantidade de entidades.

## Evidências do marco 2A

Em 21/09/2026, antes da mudança do launcher para servidor Linux por padrão,
`Testar-Ferrugem.bat -Smoke -Fps` passou em **36 verificações** com servidor Windows.
Evidência local: `Logs/Smoke/20260921-221609-833/result.json` e logs dos três
processos na mesma pasta. Além do contrato de conexão, o teste confirmou movimento
no servidor e nos clientes remotos, identidades distintas, remoção/recriação na
reconexão e ausência dos marcadores de modelo/animações ausentes.

Em seguida, `Testar-Servidor-Linux.bat -Smoke -Fps` passou em **27 verificações**
com servidor dedicado Linux e dois clientes Windows, saída 0. Evidência local:
`Logs/LinuxSmoke/20260921-221702-121/result.json` e logs na mesma pasta.

O uso normal agora é `Testar-Ferrugem.bat`: duplo clique inicia clientes Windows e
servidor Linux; `-Smoke -Fps` testa automaticamente esse mesmo caminho. Servidor
Windows permanece como diagnóstico opcional, sem exigência de testar ambos a cada
alteração. O teste local Linux não comprova capacidade ou operação de uma VPS.

O usuário confirmou o teste manual com "Boa, deu bom". A sessão manual Linux foi encerrada corretamente e passou em cinco verificações de ciclo de vida: `Logs/LinuxSmoke/20260921-222757-885/result.json`. Isso encerra o marco 2A; não representa validação de combate ou destruição.

## Próxima fase

Fase 3 adiciona três equipes, acampamentos e captura/pontuação KOTH. Testar a
fundação em área pequena antes de montar Santa Brasa ou produzir arte própria.

## Recorte implementado do marco 2B — aguardando playtest manual

A apresentação adiciona um revólver civil provisório de seis disparos, formado por
primitivas, e HUD com vida, munição, reserva, cargas, recarga e contagem de respawn.
Clique esquerdo dispara, R recarrega e G lança uma carga. O primeiro clique para
capturar o cursor não dispara; clicar no HUD não captura o mouse. O feedback de
acerto e o recuo visual só aparecem depois da confirmação do servidor.

Os infectados reaproveitam provisoriamente o civil de Quaternius com pele e roupa
dessaturadas. A antecipação do ataque mostra uma marca vermelha no chão e inclinação
do corpo. Não é uma animação final de ataque nem um modelo de zombie definitivo.
Barril e carga explosiva também usam primitivas e seus efeitos visuais respondem
ao estado replicado.

Os acertos usam as formas de teste do servidor. Não há compensação de latência,
física geral, ragdoll, mãos animadas ou som de arma neste recorte. A câmera continua
em primeira pessoa durante a morte; somente baixa em direção ao chão. Veículos e
terceira pessoa não foram adicionados.

Status: builds e verificações automáticas aprovados na versão 0.2.0; playtest manual de combate ainda pendente.

Parâmetros iniciais para playtest: seis tiros, 30 em reserva, duas cargas; recarga
em 2 s, intervalo de 0,4 s entre tiros, dano PvP de 34 no corpo e 100 na cabeça.
Carga com fusível de 1,5 s e raio de 4 m; zombies caminham a 0,9 m/s, antecipam
ataque por 0,8 s e precisam continuar próximos para acertar. Respawn em 4 s com
3 s de proteção. São valores provisórios; não comprovam balanceamento final.

### Limites de validação do combate

O modo `-Smoke -Combat` combina verificações isoladas das regras, disparo e recarga
reais enviados pelos clientes e um cenário injetado pelo servidor nas entidades
de jogo. Esse cenário usa as rotinas de produção de explosão e infecção para
verificar sua replicação, a criação única do infectado e o respawn. Ele não é uma
partida conduzida por bots nem valida sozinho perseguir, mirar com o mouse ou
lançar a carga pela tecla G. A perseguição normal e esses controles precisam de
playtest separado. A quantidade de verificações é a do `result.json` efetivo, registrada abaixo.

Zombies mortos deixam de ser apresentados, mas seus ghosts continuam retidos no
servidor neste recorte. Esse ciclo de vida precisa ser revisto antes de sessões
longas ou testes de carga. Os testes locais de dois clientes não demonstram
capacidade para 100 jogadores nem substituem medição em VPS.

## Refinamento FPS após a validação do combate

O usuário indicou POLYGON como referência de sensação FPS e cinco pacotes Synty
para direção visual. O [registro de referências](REFERENCIAS-VISUAIS-E-FPS.md)
separa fatos verificados, hipóteses de playtest e o recorte seguinte: movimento,
mira de ferro, apresentação da arma, áudio licenciado e testes de latência antes
de ampliar a arte. Este registro não implementa esses recursos nem substitui o
aceite pendente do combate atual.

## Evidências automáticas do marco 2B

Em 21/09/2026, a versão **0.2.0** passou no comando
`Testar-Ferrugem.bat -Smoke -Combat`, com servidor dedicado Linux/WSL e dois
clientes Windows: **51 verificações, saída 0**. Evidência local:
`Logs/LinuxSmoke/20260921-224225-102/result.json` e logs da mesma pasta.

O mesmo conjunto de builds verificados passou depois na regressão
`Testar-Ferrugem.bat -Smoke -Fps`: **27 verificações, saída 0**. Evidência:
`Logs/LinuxSmoke/20260921-225105-368/result.json`.

O aceite automático cobre as regras e os caminhos descritos em "Limites de
validação do combate", incluindo inputs reais de disparo/recarga e replicação do
cenário de dano injetado no servidor. Não representa aprovação manual de mira,
perseguição normal ou lançamento pela tecla G. Esse playtest permanece pendente
neste registro.

O servidor Unity ainda reporta quatro alocações persistentes ao encerrar
(`Leak Detected`), aviso já conhecido e ainda em investigação. Esses resultados
não declaram prontidão para produção, operação prolongada ou 100 jogadores.

### Inspeção visual parcial do marco 2B

Na sessão manual `Logs/LinuxSmoke/20260921-225230-064`, a inspeção da imagem
confirmou HUD legível, arma provisória visível sem material magenta, infectados
visíveis em movimento e barris no cenário. O primeiro clique capturou o cursor
mantendo seis munições. O log normal do servidor registrou disparos, morte PvP
com `infection=0` e respawn, sem exceção identificada até essa inspeção.

A interação automatizada com a janela foi interrompida ao detectar atividade do
usuário, que continuou com a sessão aberta. Isso é verificação visual parcial:
ainda não confirma o playtest completo de mira, balanceamento, antecipação/ataque
normal do zombie, infecção por perseguição normal ou uso manual da carga.
Esses itens continuam pendentes de confirmação do usuário.

## Refinamento aprovado — versão 0.3.0

O recorte seguinte acrescenta mira de ferro, agachamento, salto, aceleração/parada,
rampa/escada, mãos e recarga procedurais, balanço da arma e áudio original de
validação. Ver [escopo e critérios do refinamento](REFINAMENTO-FPS.md). A ausência
de pulo, rampas, mãos e áudio descrita nos marcos anteriores corresponde àquelas
versões. A build final 0.3.0 passou em 60 verificações Motor e 53 Combat com simulador ativo (50 ms e perda de 2% por direção), e 27 FPS sem simulador. Os caminhos dos resultados, falhas anteriores e limites estão no documento de refinamento. A inspeção visual parcial confirmou HUD, mãos/revólver e transições de morte/renascimento. Infectados atingindo o ponto de nascimento impediram concluir o tiro manual; mira, recarga, postura remota, travessia dos obstáculos e escuta continuam pendentes.

As facções têm nomes e cores aprovados: Os Sem Cova (vermelho), Mato Sem Cachorro
(verde) e Última Gota (azul). A planta conserva posições e medidas; as equipes ainda
não foram implementadas na simulação multiplayer.
