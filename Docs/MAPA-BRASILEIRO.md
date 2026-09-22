# Mapa brasileiro — Santa Brasa, Vale do Sal

Direção estabelecida pelo usuário em 21/09/2026. O Ferrugem se passa em um Brasil
pós-apocalíptico fictício e terá apresentação ao jogador em **português do Brasil**.
Este é um briefing de criação. O usuário esclareceu que cada base deve ficar a 3 km do centro e autorizou ampliar o mapa. O alvo de desenho adotado é 8 × 8 km (64 km²); isso não altera o campo técnico de testes já implementado.

## Decisões confirmadas

- Arte própria e original. **Não comprar pacotes Synty**; os cinco links indicados
  servem exclusivamente como referência visual. Não extrair ou reproduzir seus
  modelos, texturas, personagens ou cenas.
- O civil CC0 já integrado pode continuar como recurso temporário de validação.
  Isso não o transforma em arte própria nem aprova sua permanência na versão final.
- Cidade central com aproximadamente **1 km de diâmetro**, cercada por uma área
  externa de mata e montanhas.
- Três acampamentos simples de sobreviventes, feitos de materiais reaproveitados,
  distribuídos de forma equilibrada ao redor da cidade.
- Ligações por asfalto gasto e estradas de terra, com acesso ao centro. Medir tempo
  real de percurso e cobertura, além da distância geométrica entre pontos.
- Veículos, roupas e armas seguem a identidade civil, gasta e remendada. Nada de
  transformar os acampamentos em bases militares modernas.

A [planta conceitual](Mapas/README.md) apresenta estas medidas visualmente.

## Geometria adotada para o desenho

O esclarecimento do usuário substitui a referência inicial de 8 km²: são **3 km
de cada base ao centro**, e o mapa pode crescer. Adotamos como alvo conceitual um
terreno de **8 × 8 km, ou 64 km²**, com centro na origem e limites em ±4.000 m.
São dimensões de projeto; o cenário Unity executável continua com 40 × 40 m.

| Elemento | Medida de projeto |
| --- | --- |
| Cidade central | 1.000 m de diâmetro; raio de 500 m. |
| Centro de cada acampamento ao centro da cidade | 3.000 m em linha reta. |
| Separação angular das bases | 120 graus. |
| Distância entre dois centros de base | Aproximadamente 5.196 m. |
| Centro da base até a borda urbana mais próxima | 2.500 m em linha reta. |
| Raio de cada acampamento no desenho | 150 m, provisório para estudar entradas e espaço de serviço. |
| Área de captura inicial | Proposta de 250–300 m de diâmetro dentro da cidade; não é toda a área urbana. |

Posições propostas em coordenadas X/Z, em metros:

| Base | X | Z |
| --- | ---: | ---: |
| Mato Sem Cachorro | 0 | 3.000 |
| Os Sem Cova | -2.598,076 | -1.500 |
| Última Gota | 2.598,076 | -1.500 |

A faixa externa de mata e montanhas cabe entre cidade, acampamentos e limite do
terreno. O traçado exato das estradas ainda será desenhado; curvas e relevo tornam
a distância percorrida maior que a linha reta. O desenho deverá distinguir limite
de terreno, área jogável, perímetro protegido e caminhos efetivamente transitáveis.

### Tempo de viagem e acesso gratuito ao combate

Para os 2.500 m entre centro da base e borda urbana, sem paradas, curvas ou obstáculos:

| Hipótese | Tempo aproximado |
| --- | ---: |
| Caminhada a 3,5 m/s | 11,9 minutos. |
| Corrida a 6 m/s sem estamina | 6,9 minutos. |
| Veículo a 40 km/h constantes | 3,75 minutos. |

Essas contas são hipóteses, não medições de uma rota pronta. A distância exige
**transporte civil gratuito disponível na base** e, posteriormente, postos
avançados, para que participar do combate não dependa de uma longa caminhada nem
de conseguir dinheiro para o primeiro veículo. Tempos reais, espera pelo transporte
e segurança do desembarque precisam entrar no balanceamento.

## Lugar e história

Santa Brasa continua sendo o antigo entreposto agrícola do Vale do Sal. Antes da
Febre Cinzenta, a cidade recebia caminhoneiros, trabalhadores sazonais, comerciantes
e famílias vindas de diferentes partes do Brasil. A cidade é fictícia e ainda
não está situada em um estado real. Sua geografia deve formar um lugar coerente.

As cinco regiões entram pela história das pessoas e das trocas. Não precisamos
encaixar cinco biomas diferentes em torno de uma cidade, nem associar cada grupo
a uma região. As três comunidades podem ter integrantes de todas as origens.
Regionalidade não determina atributos de combate, valor moral ou comportamento.

### Propostas de presença das cinco regiões

As ideias abaixo são propostas de ficção e direção artística, não uma descrição
histórica ou arquitetônica pesquisada de localidades reais.

| Origem das histórias | História proposta | Referência visual proposta |
| --- | --- | --- |
| Norte | Família que chegou por antigas rotas fluviais e rodoviárias, trazendo ferramentas de pesca e registros de entregas. | Construções de madeira e varandas junto à água, aplicadas onde o relevo e o uso local fizerem sentido. |
| Nordeste | Famílias ligadas a reparos, costura e comércio, com objetos pessoais e placas feitas por moradores. | Fachadas com platibanda, cobogós e estruturas de feira no mercado. |
| Centro-Oeste | Memória de fretes agrícolas, manutenção de máquinas e temporadas de trabalho no interior. | Armazéns agrícolas, silos, galpões e estradas de terra associados ao entreposto. |
| Sudeste | Moradores vindos de bairros industriais e cidades de passagem, com pequenos negócios e vínculos familiares. | Sobrados de comércio e pequenos galpões de atividade fabril. |
| Sul | Famílias deslocadas com ferramentas de marcenaria e lembranças de suas cidades de origem. | Casas de madeira, telhados inclinados e uma antiga serraria. |

Essas referências visuais são propostas de pesquisa e composição, não elementos
exclusivos ou representações completas de cada região. Sua incorporação precisa
respeitar o clima, os materiais disponíveis e a história local de Santa Brasa.
Não serão cinco bairros turísticos temáticos colocados lado a lado.
Esses sinais podem se misturar em todas as áreas. Sotaques, músicas, referências de
arquitetura local e objetos culturais específicos exigirão pesquisa e revisão
quando forem produzidos. Evitar caricaturas, falas folclorizadas e uma lista de
símbolos turísticos. Mostrar pessoas vivendo, trabalhando e improvisando.

## Cidade central

O esqueleto funcional inclui mercado municipal, posto de combustível, depósitos,
delegacia, oficinas e a infraestrutura de água que motiva o controle do centro.
Os estabelecimentos precisam ser reconhecíveis por forma, cor e placas em PT-BR.
Exemplos de nomes fictícios de trabalho: **Mercado de Santa Brasa**, **Posto Sétima
Curva** e **Depósito Três Vigas**. Marcas, logotipos e anúncios serão criados para
o universo do jogo.

O desenho deve alternar vias de chegada, ruas locais, quintais e passagens a pé.
Cada equipe terá escolhas comparáveis de exposição, cobertura e acesso a posições
elevadas. Nenhuma fotografia ou cena comercial será reproduzida como planta.
O centro urbano de 1 km não define automaticamente o tamanho do volume de captura:
a área que pontua e os locais de coleta serão configurados e testados à parte.

## Três acampamentos humildes

Mato Sem Cachorro (verde), Os Sem Cova (vermelho) e Última Gota (azul) têm nomes e cores aprovados, com histórias propostas
compatíveis com a lore. Suas diferenças aparecem na organização e nos materiais,
com as mesmas oportunidades iniciais de combate e transporte.

Um acampamento começa com abrigo de lona ou telha reaproveitada, mesa de equipamento,
pequena oficina, armazenamento, reservatório de água, rádio e área para estacionar
veículos civis. Cercas remendadas, carrocerias fora de uso e estruturas de madeira
podem formar limites legíveis. O desgaste deve mostrar manutenção e uso, sem
poluição visual que esconda saídas ou interações.

As saídas protegidas, distâncias úteis, visibilidade de inimigos e possibilidade
de cerco precisam ser avaliadas igualmente. Distribuição angular regular é uma
hipótese inicial; equilíbrio será medido pelas rotas percorridas e pelos encontros.

## Arte e linguagem

Low poly sério, proporções humanas, silhuetas próprias e materiais sóbrios. Como propostas para o bloco urbano: casas térreas com beirais, reboco gasto e tijolo aparente; comércio com portas de metal; pontos de ônibus simples; caixas de água elevadas, muros remendados, telha cerâmica, postes, fiação e letreiros de oficinas e mercearias com nomes originais pintados à mão. Postes e fiação entram primeiro como ambientação; eventual destruição exige uma etapa própria. Praças, quintais e vegetação de sombra devem servir à circulação e à cobertura; espécies e particularidades locais serão escolhidas depois de pesquisa. O
vocabulário visual parte de construção civil, comércio, trabalho rural e objetos
do cotidiano brasileiro. Variar fachadas e interiores por função e história dos
moradores, sem transformar regiões brasileiras em conjuntos rígidos de aparência.

Menus, HUD, objetivos, mensagens de erro ao jogador, nomes dos locais, itens e
placas devem usar PT-BR. Identificadores de código e logs técnicos podem continuar
em inglês quando necessário para as ferramentas; isso não deve aparecer como
texto obrigatório para jogar. Vozes e conteúdo localizado também precisam ser
originais ou possuir procedência e licença registradas.

## Ordem de produção

1. Registrar o desenho com a geometria acima e definir rotas transitáveis.
2. Validar primeiro um quarteirão de 200–300 m, uma base, uma estrada e um veículo civil. Só depois expandir o bloco para as três bases e o terreno completo.
3. Medir deslocamento, linhas de visão, cobertura e custo de simulação.
4. Produzir um conjunto original mínimo: um módulo de construção, uma árvore,
   um veículo civil, um infectado e uma arma civil; expandir depois de validado.
5. Preparar edifícios para estados íntegro, danificado e destruído, com colisão
   correspondente e estado replicado; uma malha visual sozinha não fornece isso.

O mapa de produção, a navegação dos infectados e a destruição permanecem trabalhos
futuros. O cenário atual de 40 × 40 m continua útil para verificar rede e controles.

## Escala técnica e localização pendentes

64 km² é o alvo conceitual do mapa, não um resultado de desempenho nem prova de
capacidade para 100 jogadores. Terreno dividido em setores, carregamento por
proximidade, níveis de detalhe e filtragem de entidades por interesse são
planejamentos a validar. Não estão implementados pelo simples fato de o desenho
usar essas dimensões.

A revisão de PT-BR deve incluir os termos atuais do protótipo, como substituir
"respawn" por "renascimento" nas mensagens ao jogador, além de revisar menus,
itens, placas e futuras vozes. Identificadores internos, logs de diagnóstico e
nomes originais nos registros de autoria de terceiros não precisam ser traduzidos.
