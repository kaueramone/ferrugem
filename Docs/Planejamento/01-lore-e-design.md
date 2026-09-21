# Lore e design v0.2

Tudo nesta seção é criação proposta para o projeto, salvo as referências identificadas no README. Valores são parâmetros iniciais de playtest.

## Premissa

No Brasil fictício do Ferrugem, onze anos depois da Febre Cinzenta, as grandes cidades deixaram de responder. No Vale do Sal, três comunidades sobreviveram mantendo coisas simples em funcionamento: poços, motores, oficinas e pequenas lavouras. O mundo ainda tem combustível, armas de caça, rádios e estradas. O que acabou foi a cadeia capaz de substituir tudo isso.

No centro do vale fica Santa Brasa, antiga cidade de distribuição agrícola. Seu mercado municipal, depósitos, posto rodoviário e delegacia concentraram os últimos carregamentos durante a evacuação. Os infectados continuam circulando ali, atraídos por motores, geradores e disparos.

Uma estação elevatória no centro controla a água do vale. Quem mantém a região ocupada consegue operar as bombas e assegurar uma cota de abastecimento para a sua comunidade. Os mantimentos sustentam a expedição; o controle territorial decide quem passa pela próxima estiagem.

As três comunidades romperam um antigo acordo depois que um comboio de água desapareceu. Cada uma acusa as outras. Nenhuma tem recursos para invadir e ocupar os acampamentos rivais, mas todas podem disputar Santa Brasa. As partidas representam expedições diferentes; a reconstrução do mapa entre partidas é uma convenção de jogo, não um mundo que se regenera durante o combate.

Frase de apresentação: **“Segure o centro. Traga o que sobrou. Mantenha sua gente viva.”**

## Três comunidades

| Comunidade | Origem e motivação | Identidade visual |
|---|---|---|
| Cooperativa da Várzea | Agricultores e famílias que protegem sementes, água e tratores. Querem administrar o abastecimento por cotas. | Verde desbotado, lona, roupas de trabalho; símbolo de sulcos |
| União das Oficinas | Mecânicos, eletricistas e operários que mantêm geradores e bombas. Defendem que quem conserta deve comandar. | Ocre, metal reaproveitado, coletes de oficina; símbolo de chave |
| Caravana do Asfalto | Motoristas e refugiados que vivem de transporte e trocas. Querem reabrir as rotas sem monopólio. | Azul gasto, placas e faixas de caminhão; símbolo de estrada |

Mesmas capacidades de combate no início. Facções se distinguem por história, silhueta e áudio, sem bônus que produzam uma escolha dominante. Cores acompanhadas de ícones para identificação acessível. Não usar uniformes modernos ou copiar personagens, marcas e veículos de outras obras.

## Ciclo de uma vida

1. Nascer no acampamento, escolher kit gratuito ou comprar equipamento com créditos da partida.
2. Sair a pé, pegar carona ou comprar veículo civil. Transporte coletivo gratuito evita deixar jogadores sem acesso ao combate.
3. No centro, defender a zona para pontuar ou carregar recursos dos estabelecimentos.
4. Transportar a carga até a estação de entrega do acampamento. A carga limita mobilidade e ocupa capacidade no veículo.
5. A entrega gera crédito de contribuição e insumos da equipe. A oficina converte recursos por receitas abstratas e tempo de processamento.
6. Levar suprimentos processados ao centro para construir ou abastecer postos avançados.
7. Repetir enquanto a equipe tenta sustentar presença e logística.

Ao morrer, retornar à seleção de kit após um atraso curto. Carga transportada fica recuperável no mundo; créditos já recebidos não são perdidos. Armas e inventário abandonados têm prazo de limpeza. O veículo permanece até ser recuperado, destruído ou removido pelas regras de abandono.

## Regras de partida

- Até 100 jogadores; máximo de 34 por equipe, entrada preferencial na menor e diferença máxima de um quando possível. Grupo grande aguarda vagas em vez de quebrar balanceamento. Não transferir jogadores vivos compulsoriamente.
- Uma região central de disputa, fixa no primeiro mapa. Apenas sobreviventes vivos, ativos e no solo dentro do volume de captura contam. Passageiros terrestres elegíveis contam individualmente; aéreos e jogadores incapacitados não contam.
- A equipe com maior presença elegível ganha 1 ponto a cada 5 segundos. Empate no maior número pausa a pontuação; zombies não pontuam.
- Vitória em 300 pontos; limite inicial de 40 minutos. No limite, maior pontuação vence; empate gera prorrogação de até 5 minutos, encerrada na primeira vantagem. Persistindo igualdade, empate.
- Eliminações dão contribuição pessoal, sem pontos diretos de vitória. Coleta não encerra a partida por si: deve viabilizar a disputa do centro.
- Acampamentos protegidos fora da zona. Proibir construção, tiro para fora e exploração da invulnerabilidade dentro do perímetro; revisar saídas para impedir cerco inevitável.
- Postos: máximo inicial de dois por equipe, fora do núcleo de captura e dos perímetros de acampamento. Respawn consome suprimentos, tem intervalo e fica bloqueado com inimigo próximo. A equipe sempre pode voltar ao acampamento.
- Partida competitiva inicia com população mínima por equipe; abaixo disso, aquecimento em área reduzida sem progressão. Evitar um mapa vazio que dependa de 100 pessoas para funcionar.

## Economia de escassez

Separar três contadores: pontos de vitória, créditos pessoais da partida e estoque coletivo físico. O jogador não deve confundir comprar um kit com gastar o material reservado a postos.

| Local | Recurso recuperado | Utilidade abstrata |
|---|---|---|
| Mercado | Caixas de provisões e utensílios | Suprimento de respawn, recuperação e sucata |
| Posto | Combustível e peças | Autonomia, geradores e reparos |
| Depósito | Material de construção e ferramentas | Postos, barreiras e reparos |
| Delegacia | Caixas de equipamento e munição | Reposição limitada de munição e armas civis |
| Oficina | Componentes aproveitáveis | Reparação e peças de veículo |

Receitas são valores de jogo, sem simulação de fabricação real de munição. A conversão é limitada por estoque, fila e tempo. Um kit básico é sempre gratuito. Oferecer compras antecipadas apenas de kits de valor semelhante; progressão futura deve ampliar escolhas, evitando vantagem acumulada irreversível.

Exemplo para testar: uma caixa entregue gera 20 créditos ao responsável e 10 unidades de insumo à equipe; um posto custa 80 sucatas + 20 suprimentos, e cada respawn custa 1 suprimento. Valores serão revistos com telemetria, não tratados como equilíbrio final.

Recursos reaparecem em lotes anunciados e com intervalo. Depósitos, carga e entregas têm IDs únicos. Pagar recompensa apenas na transição de estado validada; pegar, largar e entregar repetidamente a mesma carga não gera dinheiro. Recursos reciclados valem menos que o custo de compra para impedir arbitragem infinita. Recompensas de transporte exigem deslocamento útil até a zona e têm intervalo.

## Armas e veículos

A pé, o jogo é exclusivamente FPS. Terceira pessoa fica disponível apenas enquanto
o jogador estiver em um veículo. Personagens mantêm proporções humanas e aparência
civil sóbria: low poly não significa cabeças enormes, cores de brinquedo ou tom cômico.

Armas propostas: espingarda de caça, carabina de ferrolho, rifle civil de pequeno calibre, revólver, pistola antiga, arma artesanal fictícia e ferramentas de contato. Começar com uma carabina simples. Equipamento é gasto e limitado, mas a resposta dos comandos deve ser consistente; falhas aleatórias que decidem duelos ficam fora do primeiro protótipo.

Terrestres: bicicleta, moto utilitária, automóvel, caminhonete, van, caminhão e trator. A blindagem improvisada troca velocidade, consumo e visibilidade por proteção. Nenhum tanque ou plataforma militar.

Aéreos previstos: ultraleve de transporte leve e helicóptero civil de manutenção difícil. Começar com um único modelo após a validação terrestre; exigir combustível, área de pouso e limite por equipe. Sem mísseis ou aeronaves de combate. A visibilidade aérea deve entrar nos testes de rede, porque expõe mais objetos do mapa.

## Zombies e destruição no mesmo sistema

Infectados são lentos, inspirados no ritmo de The Walking Dead, e funcionam como obstáculos e distrações durante a disputa entre jogadores. Respondem a som e visão, abandonam perseguições sem estímulo e se concentram nos pontos de recurso. Tiros e motores aumentam o risco de uma rota, mas não fazem nascer inimigos arbitrariamente na frente do jogador.

Um ataque corpo a corpo válido do zombie mata o jogador imediatamente. O servidor confirma alcance e instante do ataque; mero contato entre colisores não equivale a acerto. A morte causada por zombie gera exatamente um novo infectado no lugar do jogador morto. O atacante continua existindo e o jogador retorna pelo respawn humano. Esse evento deve ser deduplicado; não há período de incubação ou controle do infectado pelo jogador. Mortes PvP não geram zombies no recorte inicial.

Zombies só morrem com tiro na cabeça ou explosão; tiros no corpo não os matam. Começar com poucos, ataques perceptíveis e perseguição limitada para que sejam interferência ambiental, apesar da letalidade. Velocidade, população e alcance exigem playtest especialmente em portas e interiores.

Armas leves quebram vidros, portas e coberturas frágeis. Árvores caem com impacto apropriado, ferramentas e dano acumulado. Veículos pesados e eventos explosivos de jogo podem comprometer estruturas preparadas. Edifícios reforçados não desabam com poucos tiros de pistola.

Crateras físicas aparecem apenas onde o terreno admite deformação. Estruturas críticas, acampamentos e geometrias essenciais têm limites claros. A destruição abre flancos e muda cobertura sem eliminar toda possibilidade de disputar o objetivo.

## Primeiro mapa e primeiro recorte

A proposta inicial de validação de 600 × 600 m, com centro de 150–200 m, era um bloco de protótipo e não define o mapa final. A antiga sugestão de expansão para 1,5–2 km de lado foi substituída pela direção atual do usuário: cidade central de aproximadamente 1 km de diâmetro, três acampamentos e faixa externa de mata/montanhas. O usuário esclareceu 3 km de cada base ao centro e autorizou ampliar o mapa: o alvo de desenho adotado é 8 × 8 km (64 km²), bases a 120 graus e cidade de 1 km de diâmetro. A referência inicial de 8 km² deixou de limitar essa proposta. Consulte o [briefing brasileiro](../MAPA-BRASILEIRO.md). O cenário técnico implementado de 40 × 40 m continua separado desse alvo.

Primeiro recorte jogável: 12 pessoas, carabina, caminhonete, mercado, reciclagem, um modelo de posto, 15–30 zombies ativos e um quarteirão destrutível. Um prédio modular, uma árvore derrubável e uma seção de solo com cratera física bastam para provar os riscos.

Depois: variedade civil, cinco tipos de estabelecimento, 30/60/100 jogadores, mais infectados por orçamento, um aéreo e mapa ampliado. Estética low poly com silhuetas legíveis, iluminação simples e ruído visual controlado. Não definir requisitos finais de loja antes de medir esse conteúdo.

## Direção brasileira e arte própria

Toda a apresentação ao jogador será em português do Brasil. Santa Brasa e o Vale
do Sal são fictícios; moradores e rotas de comércio trazem histórias das cinco
regiões brasileiras, sem vincular região a facção ou comprimir cinco biomas no
mesmo mapa. As comunidades vivem em acampamentos simples de materiais
reaproveitados, ligados por asfalto gasto e estradas de terra.

A arte de produção será original. Os pacotes Synty indicados são apenas referências
e não serão comprados; o civil CC0 atual permanece provisório. Geografia, nomes,
paleta e critérios de composição estão no [briefing do mapa](../MAPA-BRASILEIRO.md).
