# FERRUGEM: O ÚLTIMO ENTREPOSTO

Documento fundador v0.1 — 21/09/2026. Nome provisório; disponibilidade comercial ainda não pesquisada.

FPS multiplayer PvPvE em Unity, com até 100 jogadores, três comunidades de sobreviventes, equipamento civil improvisado, zombies e destruição que altera o combate. Defender o centro garante a vitória; transportar recursos mantém a equipe capaz de lutar.

Este pacote é uma proposta de design e execução. Números de balanceamento, capacidade de hardware e prazos são hipóteses para testar, não resultados medidos. Ainda não foi criado um projeto Unity nem executado um teste de jogo.

## Documentos

1. [Lore e design](01-lore-e-design.md): mundo, facções, ciclo, economia, regras, mapa e identidade.
2. [Arquitetura](02-arquitetura.md): servidor, rede, destruição, zombies, persistência e metas de desempenho.
3. [Plano de execução](03-plano-de-execucao.md): fases, dependências e critérios para avançar.
4. [Máquina e hospedagem](04-maquina-e-hospedagem.md): inventário observado, ferramentas e dimensionamento experimental.
5. [Steam e participação de IA](05-steam-e-ia.md): publicação, testes, direitos e registro de uso de IA.

## Decisões propostas

- Windows PC primeiro; servidor dedicado Linux. Perspectiva em primeira pessoa.
- Unity 6.3 LTS + URP; validar Netcode for Entities em uma prova técnica curta antes de produzir conteúdo.
- Partidas independentes: mapa, recursos, postos e destruição reiniciam. Progressão persistente inicialmente cosmética.
- Até 100 jogadores: distribuição 34/33/33 com entrada na menor equipe. Testes equilibrados também com 99.
- Primeiro marco: 12 jogadores, três equipes, uma zona, uma arma, uma caminhonete, um ponto de coleta, um posto e poucos zombies.
- Destruição por módulos, árvores com estados e crateras físicas limitadas a áreas preparadas. Demonstrar os três tipos cedo.
- Aéreos civis fazem parte da visão, entrando após veículos terrestres e rede estarem estáveis.
- IA participa do planejamento e desenvolvimento, com revisão humana e registro do conteúdo efetivamente utilizado.

## Referências e limites da pesquisa

O KOTH de Arma fornece a referência de disputa territorial entre três equipes e progressão. WARDOGS fornece a referência de grande escala, logística, veículos, fortificação e destruição. Essas descrições públicas orientam o gameplay; não revelam a arquitetura interna desses jogos. Nossa arquitetura abaixo é uma proposta independente. O percurso estabelecimento → acampamento → reciclagem → posto avançado é o diferencial deste projeto. Fontes: [Arma KOTH](https://armakoth.com/Official), [WARDOGS na Steam](https://store.steampowered.com/app/1867240/WARDOGS/).

## Próximo trabalho concreto

Executar a fase 0 e a fase 1 do plano: criar repositório e projeto novos, fixar versões, gerar cliente Windows e servidor Linux e demonstrar dois clientes conectados. A escolha definitiva da rede depende dessa prova. Não comprar pacotes de veículos/destruição antes de verificar compatibilidade.

Para ajustar calendário e operação, faltam tamanho da equipe, experiência com Unity/C#, horas semanais, orçamento recorrente e região dos primeiros jogadores. A proposta assume início com uma pessoa e apoio de IA; nenhum gasto ou publicação foi realizado.
