# Plano de execução por marcos

Ordem orientada aos riscos de rede, destruição e diversão. O encerramento de cada fase exige evidências, não apenas uma lista de funcionalidades implementadas. Referências devem ser abertas na versão fixada antes de copiar padrões de código.

## Fase 0 — Descoberta e decisões

Pesquisa inicial concluída em 21/09/2026: referências de gameplay, pacotes Unity, servidor Linux e Steam. Ainda falta validar compatibilidade dentro de um projeto novo.

Entregas: registrar versão exata do Editor e pacotes, plataforma Windows/Linux, limites de destruição e decisão provisória de rede. Criar um projeto mínimo de avaliação sem adaptar os projetos já presentes nesta pasta.

| Padrão permitido como ponto de partida | Referência verificada | Uso planejado |
|---|---|---|
| `ClientServerBootstrap`, `WorldSystemFilter` | [Mundos cliente/servidor, Netcode 1.4](https://docs.unity3d.com/Packages/com.unity.netcode@1.4/manual/client-server-worlds.html) | Separar simulação e apresentação |
| `ClientServerTickRate.SimulationTickRate`, `NetworkTickRate` | Mesma página | Configurar simulação e snapshots |
| `PredictedSimulationSystemGroup` | Mesma página | Organizar simulação prevista |
| `GhostRelevancy`, `GhostRelevancySet` | [Otimizações, Netcode 1.4](https://docs.unity3d.com/Packages/com.unity.netcode@1.4/manual/optimizations.html) | Filtrar estado por cliente |
| `StandaloneBuildSubtarget.Server`, `BuildPlayerOptions.subtarget`, `UNITY_SERVER` | [Dedicated Server build, Unity 6.3](https://docs.unity.com/en-us/engine/6000.3/manual/platform-specific/dedicated-server/build) | Produzir build Linux de servidor |
| NetCube, HelloNetcode e Asteroids | [Exemplos oficiais](https://github.com/Unity-Technologies/EntityComponentSystemSamples/blob/master/NetcodeSamples/README.md) | Estudar conexão, inputs e previsão |

Verificação: compilar o exemplo correspondente ao pacote escolhido e registrar sua licença/versão. A documentação 1.4 acima não comprova compatibilidade automática com outro pacote. Os exemplos consultados declaram Unity 6.2/DOTS 1.4; portar apenas após ler as diferenças. Não inventar métodos, parâmetros ou transportar exemplos de NGO para Entities.

## Fase 1 — Fundação reproduzível

Implementar: projeto URP novo, repositório Git, LFS para binários relevantes, `.gitignore` Unity, configurações serializadas, cena mínima, builds Windows e Linux. Usar o padrão de bootstrap oficial e conectar dois clientes ao servidor independente.

Referências: tabela da fase 0; [requisitos Dedicated Server](https://docs.unity.com/en-us/engine/6000.3/manual/platform-specific/dedicated-server/get-started/requirements).

Aceite: checkout limpo abre e compila; cliente e servidor registram mesma versão de protocolo; conexão, desconexão e nova conexão funcionam; servidor executa sem GPU. Guardas: nada de credenciais no Git, dependência de Editor no servidor ou pasta Library versionada.

## Fase 2 — Prova dos riscos combinados

Implementar: personagem previsto, tiro validado, um veículo terrestre, um zombie, árvore com estado, parede modular e uma célula de terreno que muda colisão. Reutilizar padrões dos exemplos oficiais e documentar o código próprio de gameplay.

Executar em três marcos, conforme [Fase 2](../FASE-2.md): **2A**, personagem FPS e
corpo remoto entre dois clientes; **2B**, arma civil e zombie lento, ataque fatal,
criação de um infectado por morte causada por zombie e eliminação apenas por tiro
na cabeça ou explosão; **2C**, veículo civil e destruição. Terceira pessoa é
permitida somente em veículos. O marco 2A não inclui combate ou infectados.
Usar assets gratuitos de proporções humanas e estética sóbria, com procedência em
[Assets do protótipo](../ASSETS-PROTOTIPO.md), antes de investir em arte própria.

Referências: [previsão e mundos](https://docs.unity3d.com/Packages/com.unity.netcode@1.4/manual/client-server-worlds.html), [snapshots](https://docs.unity3d.com/Packages/com.unity.netcode@1.10/manual/ghost-snapshots.html), [NavMesh Surface](https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavMeshSurface.html), arquitetura deste pacote.

Aceite: dois clientes veem o mesmo dano/colisão; novo cliente encontra os objetos já destruídos; carro e zombie atravessam corretamente o cenário modificado; comportamento utilizável com 100 ms RTT e perda de 1%; perfil de CPU registrado. Guardas: sem autoridade do cliente sobre dano, sem RPC único como armazenamento da destruição e sem assumir física idêntica entre máquinas.

Decisão de saída: manter stack, reduzir escopo de destruição ou testar alternativa de rede antes de ampliar conteúdo. Esta fase pode invalidar estimativas de tempo posteriores.

## Fase 3 — KOTH de 12 jogadores

Implementar: três equipes, entrada balanceada, acampamentos, kit básico, área de captura, HUD, respawn, vitória e reinício de partida. Copiar o padrão de lista de jogadores do exemplo oficial e escrever as regras originais descritas no design.

Referência: [NetcodeSamples — PlayerList](https://github.com/Unity-Technologies/EntityComponentSystemSamples/blob/master/NetcodeSamples/README.md) e regras de captura no documento 01.

Aceite: sessão com quatro pessoas por equipe; empate pausa pontos; morte e saída removem presença; partida termina e reinicia sem transportar recursos antigos. Testes automatizados para contagem de elegíveis, vitória e limite de equipe. Guardas: posição/dano/placar do cliente nunca determinam resultado.

## Fase 4 — Ciclo de logística completo

Implementar: um ponto de coleta, carga física, capacidade de transporte, entrega, reciclagem, créditos, compra de caminhonete, construção e abastecimento de posto. Dados de receita versionados; comandos com validação e deduplicação.

Referências: padrões de snapshots já validados e tabelas de economia do documento 01. Não há API Unity que implemente essa economia pronta; escrever regras próprias, mantendo nomes internos claramente distintos de APIs do pacote.

Aceite: jogador percorre acampamento → mercado → acampamento → posto e outro jogador consegue usar o posto. Repetir entrega ou reconectar não duplica itens. Equipe sem estoque continua jogável com kit básico. Guardas: coleta não vira fonte ilimitada de crédito, reciclagem não devolve mais do que custou comprar.

## Fase 5 — Recorte jogável de 12–30 pessoas

Implementar: mapa compacto, direções visuais das três comunidades, mercado e rotas, 15–30 infectados ativos, áudio de feedback, ajustes de veículo e destruição; menus essenciais e controles remapeáveis. Incluir pings de equipe antes de depender de voz.

Referências: arquitetura e [NavMesh Surface](https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavMeshSurface.html). Ler a documentação dos pacotes de input/renderização efetivamente selecionados antes de implementar integração.

Aceite: partidas completas com pessoas reais; medir tempo até primeiro encontro, duração, viagens, contribuição por função e abandono. Jogadores conseguem explicar por que vale a pena defender e por que vale a pena transportar. Guardas: não ampliar mapa para encobrir problemas de combate; não produzir dezenas de assets antes dessa validação.

## Fase 6 — Escala e operação: 30 → 60 → 100

Implementar: relevância espacial, prioridades, limites de entidades, instrumentação e clientes automatizados de carga; deploy Linux repetível, reconexão e autenticação. Em cada patamar, repetir cenário misto e centro lotado.

Referências: [Ghost relevancy e importância](https://docs.unity3d.com/Packages/com.unity.netcode@1.4/manual/optimizations.html), [thin clients](https://docs.unity3d.com/Packages/com.unity.netcode@1.4/manual/client-server-worlds.html), requisitos de hospedagem do documento 04. Confirmar APIs de thin clients na versão instalada.

Aceite: orçamento de tick do documento 02; memória estabilizada após aquecimento; duas horas sem crash nem duplicação; medição de rede sob perda; 100 clientes externos com ações reais, seguida de playtest humano. Guardas: bots locais e conexões ociosas não servem como certificação de capacidade.

## Fase 7 — Conteúdo e aéreos civis

Implementar: demais estabelecimentos, variedade limitada de armas/veículos, um aéreo civil, mapa ampliado apenas se necessário, áudio e arte final selecionada. Revalidar navegação, interesse de rede, transporte aéreo e destruição com novo conteúdo.

Referências: documentação dos componentes de veículo selecionados na fase 2; padrões de rede fixados; guia de direção do documento 01. Escolher asset externo exige prova de licença, manutenção e compatibilidade.

Aceite: aéreo tem papel útil sem dominar todas as rotas; orçamento mantém-se com visão de longa distância; todas as equipes têm acesso equivalente. Guardas: não tratar pacote de veículo físico como solução de multiplayer pronta.

## Fase 8 — Steam e verificação de lançamento

Implementar: integração de identidade, build distribuível, página coerente, Steam Playtest, suporte, denúncia/moderação, inventário de direitos e declaração de IA. Consultar fontes do documento 05 no momento da submissão.

Aceite: instalação limpa pela Steam, autenticação falha de forma segura, duas partidas seguidas, reconexão, controle remapeável e operação sem intervenção do desenvolvedor. Conferir código contra as APIs da versão fixada; revisar autoridade indevida no cliente, RPCs usados como estado e dependências de Editor. Rodar testes de captura/transações e regressão de carga, além de revisão humana do gameplay e das declarações da loja.

Guardas: não anunciar capacidade ou desempenho não demonstrados. Revisão técnica interna não substitui aprovação da Valve. Early Access só com produto já jogável e limites claramente apresentados.

## Horizonte e recursos humanos

Estimativas de esforço para uma pessoa experiente em Unity/C#, trabalhando em tempo integral: fases 0–2, aproximadamente 4–8 semanas; fases 3–5, mais 8–16 semanas; escala, conteúdo, operação e publicação podem exigir mais 6–12 meses ou muito mais. São faixas de planejamento, não compromisso de lançamento. Experiência inicial, trabalho parcial e aprendizado de DOTS podem ampliar muito esses prazos.

Planejar o produto completo como trabalho de pelo menos muitos meses, possivelmente anos se solo. A assistência de IA ajuda a produzir e revisar, mas não substitui teste com jogadores, integração, direção e operação. Calibrar calendário após as duas primeiras semanas de trabalho real.

Competências necessárias: gameplay/C#, rede e profiling, arte/animação, level design, áudio e QA/operação. Podem ser acumuladas no protótipo; revisar necessidade de colaboradores após a prova técnica. Custos a orçar separadamente: tempo de trabalho, serviços de IA, arte/áudio/licenças, hospedagem, tráfego, backup, testes e publicação.
