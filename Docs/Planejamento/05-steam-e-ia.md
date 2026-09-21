# Publicação na Steam e participação de IA

Fontes oficiais consultadas em 21/09/2026. Revalidar políticas no momento do cadastro e da submissão. Este documento prepara a publicação; não abre conta, paga taxa nem publica conteúdo.

## Caminho administrativo

1. Definir responsável legal e nome comercial. A Steam permite cadastro de indivíduo, conforme seu enquadramento; preparar identificação, informações fiscais e conta bancária correspondentes.
2. Concluir onboarding, verificação e contratos Steamworks. Obrigações fiscais no país do titular dependem de sua situação e não foram determinadas nesta pesquisa.
3. Pagar Steam Direct: US$ 100 ou equivalente por aplicativo, sujeito aos tributos aplicáveis. Não é reembolsável; pode ser recuperada no pagamento após US$ 1.000 de receita bruta ajustada elegível.
4. Planejar o prazo mínimo de 30 dias após o pagamento para os primeiros títulos e pelo menos duas semanas de página pública Coming Soon. As janelas podem se sobrepor.

Fontes: [Onboarding Steamworks](https://partner.steamgames.com/doc/gettingstarted/onboarding), [Steam Direct Fee](https://partner.steamgames.com/doc/gettingstarted/appfee).

## Preparação do produto

- Nome pesquisado antes de marca/arte definitiva; direitos documentados sobre código, imagens, modelos, fontes, música, voz e assets externos.
- Build Windows distribuída por SteamPipe, depots e opções de inicialização corretos; instalar em máquina limpa e testar cliente junto a servidor disponível.
- Página com descrição, idiomas, capturas de gameplay, materiais gráficos e requisitos medidos. Apresentar claramente online obrigatório, disponibilidade/regiões dos servidores e estado do desenvolvimento.
- Questionário de conteúdo: violência e demais elementos efetivamente presentes; responder questões de classificação regional. Não presumir classificação etária com base em low poly.
- Se houver coleta de dados, autenticação, denúncias ou serviços externos, documentar seu uso e preparar os avisos e termos aplicáveis.

Página e build passam por revisões separadas. A Valve informa normalmente 3–5 dias úteis por revisão e recomenda enviar com pelo menos sete dias úteis de antecedência. Aprovação não é automática; reservar tempo para correções. [Review Process](https://partner.steamgames.com/doc/store/review_process).

Integração Steamworks não fornece automaticamente netcode, anti-cheat completo ou hospedagem. Identidade e autenticação devem ser integradas à nossa validação do servidor. Consultar a documentação e licença do wrapper C# escolhido antes de instalá-lo.

## Playtest antes de vender

Usar Steam Playtest para organizar sessões com população suficiente: primeiro 12–30, depois 60 e 100. É um recurso gratuito com AppID separado vinculado ao produto; não cobrar acesso nem monetizar o Playtest. [Steam Playtest](https://partner.steamgames.com/doc/features/playtest).

Se decidirmos distribuir binários de servidor para a comunidade, configurar um aplicativo TOOL com seus depots e distribuição SteamCMD. É opcional e não fornece máquinas para hospedar. [Distributing Your Dedicated Game Server](https://partner.steamgames.com/doc/sdk/uploading/distributing_gs).

Antes de lançamento público, exigir partidas completas estáveis, instalação/atualização/reconexão, logs operacionais, canal de suporte, meios de denúncia e capacidade demonstrada. Early Access precisa comunicar o conteúdo realmente jogável e o trabalho restante; não usar a página como promessa de que todos os sistemas serão entregues.

## Declaração de IA

Texto proposto para README, créditos e comunicação pública, ajustado ao uso real:

> Este projeto é desenvolvido com assistência de inteligência artificial em planejamento, documentação e programação, sob direção, revisão e responsabilidade humana. Conteúdos narrativos e outros materiais produzidos com assistência de IA são registrados e identificados conforme sua utilização no jogo.

No estado atual, a assistência está demonstrada no planejamento e na proposta de lore; programação ainda é atividade prevista. Ajustar o texto quando houver implementação, sem atribuir ferramentas ou modalidades ainda não usadas.

A Steam distingue conteúdo pré-gerado e conteúdo gerado durante a execução. Seu questionário foca conteúdo consumido pelo jogador, como arte, som, narrativa e localização, e não ganhos de eficiência nas ferramentas. Se a lore deste pacote entrar no jogo, registrar o uso como narrativa assistida. Geração em tempo real exige também informar salvaguardas; não está prevista nesta versão. Comportamento programado dos zombies não é IA generativa. [Content Survey — seção 3](https://partner.steamgames.com/doc/gettingstarted/contentsurvey).

## Registro de procedência

Manter um registro por conteúdo incluído na build:

| Campo | O que registrar |
|---|---|
| ID e arquivo | Local e versão do conteúdo |
| Categoria | Código, narrativa, imagem, modelo, áudio, tradução |
| Origem | Autoria própria, fornecedor/licença ou ferramenta de IA |
| Data e ferramenta | Identificação disponível do serviço/modelo e data |
| Uso da IA | Rascunho, geração, transformação ou revisão |
| Revisão humana | Responsável e alterações realizadas |
| Direitos | Termos/licença aplicáveis e prova de aquisição quando houver |
| Destino | Referência interna ou conteúdo entregue na build |

Registro inicial: `lore-v0.1`, proposta narrativa do documento 01, criada nesta colaboração com IA em 21/09/2026, ainda aguardando revisão e seleção humana; não incluída em nenhuma build. Não presumir que todo resultado gerado pode ser utilizado comercialmente sem conferir seus termos e origem.

## Checklist de submissão

- [ ] Conta, identidade, banco, informações fiscais e taxa resolvidos.
- [ ] Nome e direitos dos conteúdos revisados.
- [ ] Loja e requisitos representam a build testada.
- [ ] Questionário de conteúdo e uso de IA preenchidos conforme o produto.
- [ ] Build instala e conecta a servidor funcional.
- [ ] Capacidade anunciada foi demonstrada em carga e playtest.
- [ ] Suporte e operação preparados.
- [ ] Revisões aprovadas e períodos obrigatórios cumpridos.
- [ ] Aprovação humana final para publicar e anunciar data.
